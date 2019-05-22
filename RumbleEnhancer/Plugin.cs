using CustomUI.Settings;
using IPA;
using IPA.Config;
using IPA.Utilities;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;

namespace Rumbleenhancer

{
    public class Plugin : IBeatSaberPlugin
    {
        public static object asyncRumbleLock = new object();

        internal static Ref<PluginConfig> config;
        internal static IConfigProvider configProvider;
        
        public void Init(IPALogger logger, [Config.Prefer("json")] IConfigProvider cfgProvider)
        {
            Logger.log = logger;
            configProvider = cfgProvider;

            config = configProvider.MakeLink<PluginConfig>((p, v) =>
            {
                if (v.Value == null || v.Value.RegenerateConfig)
                    p.Store(v.Value = new PluginConfig() { RegenerateConfig = false });
                config = v;
            });
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
            if (scene.name == "MenuCore")
            {
                SubMenu settingsSubmenu = SettingsUI.CreateSubMenu("Rumble Enhancer");

                IntViewController rumbleStrength = settingsSubmenu.AddInt("Rumble Strength", 0, 10, 1);
                rumbleStrength.GetValue += delegate { return config.Value.RumbleStrength; };
                rumbleStrength.SetValue += delegate (int value) { config.Value.RumbleStrength = value ; };

                IntViewController rumbleTime = settingsSubmenu.AddInt("Rumble Length\t\t(in ms)", 0, 250, 5);
                rumbleTime.GetValue += delegate { return config.Value.RumbleTimeMS; };
                rumbleTime.SetValue += delegate (int value) { config.Value.RumbleTimeMS = value; };


                IntViewController rumblePause = settingsSubmenu.AddInt("Rumble Interval\t(in ms)", 0, 250, 1);
                rumblePause.GetValue += delegate { return config.Value.TimeBetweenRumblePulsesMS; };
                rumblePause.SetValue += delegate (int value) { config.Value.TimeBetweenRumblePulsesMS = value; };

                Logger.log.Info("Settings attached!");
            }
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Logger.log.Info("Plugin unloaded!");

        }

        public void OnFixedUpdate(){}
        public void OnUpdate(){}
        public void OnSceneUnloaded(Scene scene){}

        public static int RumbleTimeMS
        {
            get
            {
                int Time = config.Value.RumbleTimeMS;
                return Mathf.Clamp(Time, 0, Time);
            }
        }

        public static int TimeBetweenRumblePulsesMS
        {
            get
            {
                int Time = config.Value.TimeBetweenRumblePulsesMS;
                return Mathf.Clamp(Time, 5, Time);
            }
        }

        public static float RumbleStrength
        {
            get
            {
                float Strength = config.Value.RumbleStrength * 0.1f;
                return Mathf.Clamp(Strength, 0.0f, 1.0f);
            }
        }

    }
}
