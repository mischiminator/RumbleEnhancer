using CustomUI.Settings;
using IPA;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;

namespace Rumbleenhancer

{
    public class Plugin : IBeatSaberPlugin
    {
        public static object asyncRumbleLock = new object();

        internal static bool save = false;
        internal static bool settingsattached = false;
        Settings settings;

        public void Init(IPALogger logger)
        {
            Logger.log = logger;
            try
            {
                settings = new Settings();
            }
            catch (Exception e)
            {
                logger.Info(e.Message);
            }
        }

        public void OnApplicationStart()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            SceneManager.sceneLoaded += OnSceneLoaded;

            Logger.log.Info("Plugin Version 1.4.3 loaded!");
        }

        public void OnActiveSceneChanged(Scene fromScene, Scene toScene)
        {
            if (toScene.name != "GameCore")
            {
                return;
            }

            GameObject RumbleObject = new GameObject("Rumble Enhancer");
            RumbleObject.AddComponent(typeof(RumbleEnhancer));
            Logger.log.Info("GameObject attached!");
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            if (save)
            {
                Logger.log.Info("Saving Settings");
                settings.SaveSettings();
                save = false;
            }
            //Logger.log.Info("\"" + scene.name + "\" loaded");
            if (scene.name == "MenuCore" && !settingsattached)
            {
                SubMenu settingsSubmenu = SettingsUI.CreateSubMenu("Rumble Enhancer");

                IntViewController rumbleStrength = settingsSubmenu.AddInt("Rumble Strength", 0, 10, 1);
                rumbleStrength.GetValue += delegate { return settings.RumbleStrengthSettings(); };
                rumbleStrength.SetValue += delegate (int value) { settings.RumbleStrength = value; };

                IntViewController rumbleTime = settingsSubmenu.AddInt("Rumble Length\t\t(in ms)", 0, 250, 5);
                rumbleTime.GetValue += delegate { return settings.RumbleTimeMS; };
                rumbleTime.SetValue += delegate (int value) { settings.RumbleTimeMS = value; };

                IntViewController rumblePause = settingsSubmenu.AddInt("Rumble Interval\t(in ms)", 5, 250, 1);
                rumblePause.GetValue += delegate { return settings.TimeBetweenRumblePulsesMS; };
                rumblePause.SetValue += delegate (int value) { settings.TimeBetweenRumblePulsesMS = value; };

                settingsattached = true;
                Logger.log.Info("Settings attached!");
            }

        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Logger.log.Info("Plugin unloaded!");

        }

        public void OnFixedUpdate() { }
        public void OnUpdate() { }
        public void OnSceneUnloaded(Scene scene)
        {
            //Logger.log.Info("\"" + scene.name + "\" unloaded");
            if (scene.name == "MenuCore" && save)
            {
                save = false;
                //settings.SaveSettings();
            }
        }

    }
}
