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
        internal static Ref<PluginConfig> config;
        internal static IConfigProvider configProvider;

        private readonly string[] GameplaySceneNames = { "MenuCore", "GameCore" };


        public void Init(IPALogger logger, [Config.Prefer("json")] IConfigProvider cfgProvider)
        {
            Logger.log = logger;
            configProvider = cfgProvider;

            config = cfgProvider.MakeLink<PluginConfig>((p, v) =>
            {
                if (v.Value == null || v.Value.RegenerateConfig)
                    p.Store(v.Value = new PluginConfig() { RegenerateConfig = false });
                config = v;
            });
        }

        public void OnApplicationStart()
        {
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;

            Logger.log.Info("Plugin Version 1.4.0 loaded!");
        }

        private void SceneManagerOnActiveSceneChanged(Scene fromScene, Scene toScene)
        {
            if (!GameplaySceneNames.Contains(toScene.name))
            {
                return;
            }

            GameObject RumbleObject = new GameObject("Rumble Enhancer");
            RumbleObject.AddComponent(typeof(RumbleEnhancer));
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name == "MenuCore")
            {
                SubMenu settingsSubmenu = SettingsUI.CreateSubMenu("Rumble Enhancer");

                IntViewController rumbleStrength = settingsSubmenu.AddInt("Rumble Strength", 0, 10, 1);
                rumbleStrength.GetValue += delegate { return ModPrefs.GetInt(config.Value.plugin_name, "RumbleStrength", 1, true); };
                rumbleStrength.SetValue += delegate (int value) { ModPrefs.SetInt(config.Value.plugin_name, "RumbleStrength", value); };

                IntViewController rumbleTime = settingsSubmenu.AddInt("Rumble Length\t\t(in ms)", 0, 250, 5);
                rumbleTime.GetValue += delegate { return ModPrefs.GetInt(config.Value.plugin_name, "RumbleTimeMS", 25, true); };
                rumbleTime.SetValue += delegate (int value) { ModPrefs.SetInt(config.Value.plugin_name, "RumbleTimeMS", value); };


                IntViewController rumblePause = settingsSubmenu.AddInt("Rumble Interval\t(in ms)", 0, 250, 1);
                rumblePause.GetValue += delegate { return ModPrefs.GetInt(config.Value.plugin_name, "TimeBetweenRumblePulsesMS", 5, true); };
                rumblePause.SetValue += delegate (int value) { ModPrefs.SetInt(config.Value.plugin_name, "TimeBetweenRumblePulsesMS", value); };

            }
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
            Logger.log.Info("Plugin unloaded!");

        }

        public void OnFixedUpdate(){}
        public void OnUpdate(){}
        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene){}
        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode){}
        public void OnSceneUnloaded(Scene scene){}

        public static int RumbleTimeMS
        {
            get
            {
                int Time = ModPrefs.GetInt(config.Value.plugin_name, "RumbleTimeMS", 25, true);
                return Mathf.Clamp(Time, 0, Time);
            }
        }

        public static int TimeBetweenRumblePulsesMS
        {
            get
            {
                int Time = ModPrefs.GetInt(config.Value.plugin_name, "TimeBetweenRumblePulsesMS", 5, true);
                return Mathf.Clamp(Time, 5, Time);
            }
        }

        public static float RumbleStrength
        {
            get
            {
                float Strength = ModPrefs.GetFloat(config.Value.plugin_name, "RumbleStrength", 1.0f, true);
                return Mathf.Clamp(Strength, 0.0f, 1.0f);
            }
        }

    }
}
