using CustomUI.Settings;
using Harmony;
using IPA;
using IPALogger = IPA.Logging.Logger;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

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

            var harmony = HarmonyInstance.Create("com.kariko.BeatSaber.RumbleEnhancer");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

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
            if (toScene.name == "GameCore")
            {
                GameObject RumbleObject = new GameObject("Rumble Enhancer");
                RumbleObject.AddComponent(typeof(RumbleEnhancer));
            }
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            if (save)
            {
                Logger.log.Info("Saving Settings");
                settings.SaveSettings();
                save = false;
            }

            if (scene.name == "MenuCore" && !settingsattached)
            {
                SubMenu settingsSubmenu = SettingsUI.CreateSubMenu("Rumble Enhancer");

                string hint = "Duration of Rumble Effect";

                IntViewController rumbleTime = settingsSubmenu.AddInt("Rumble Length\t\t(in ms)", hint, 0, 250, 5);
                rumbleTime.GetValue += delegate { return settings.RumbleTimeMS; };
                rumbleTime.SetValue += delegate (int value) { settings.RumbleTimeMS = value; };

                hint = "The Pause between single pulses,\n the lower this is the stronger the rumble will feel";

                IntViewController rumblePause = settingsSubmenu.AddInt("Rumble Interval\t(in ms)", hint, 0, 30, 1);
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
            if (scene.name == "MenuCore" && save)
            {
                save = false;
                settings.SaveSettings();
            }
        }

    }
}
