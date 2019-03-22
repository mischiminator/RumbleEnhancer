using IllusionPlugin;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine;
using CustomUI;
using CustomUI.GameplaySettings;
using CustomUI.Settings;
using CustomUI.MenuButton;
using CustomUI.Utilities;
using System;

namespace RumbleEnhancer
{
	public class Plugin : IPlugin
	{
		public const string PluginName = "RumbleEnhancer";

		public string Name => PluginName;
		public string Version => "1.2.0";

        bool toggleValue = false;

        private readonly string[] GameplaySceneNames = { "Menu", "GameCore" };

		public void OnApplicationStart()
		{
			SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
			SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            Console.WriteLine("[RumbleEnhancer] Plugin loaded");
		}

		private void SceneManagerOnActiveSceneChanged(Scene fromScene, Scene toScene)
		{
			if (!GameplaySceneNames.Contains(toScene.name))
			{
				return;
			}

			GameObject RumbleObject = new GameObject("Rumble Enhancer");
			RumbleObject.AddComponent<RumbleEnhancer>();
		}

		public void OnApplicationQuit()
		{
			SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
			SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
		}

		private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name == "MenuCore")
            {
                SubMenu settingsSubmenu = SettingsUI.CreateSubMenu("Rumble Enhancer");

                IntViewController rumbleStrength = settingsSubmenu.AddInt("Rumble Strength", 0, 10, 1);
                rumbleStrength.GetValue += delegate { return ModPrefs.GetInt(Name, "RumbleStrength", 1, true); };
                rumbleStrength.SetValue += delegate (int value) { ModPrefs.SetInt(Name, "RumbleStrength", value); };

                IntViewController rumbleTime = settingsSubmenu.AddInt("Rumble Length\t\t(in ms)", 0, 250, 5);
                rumbleTime.GetValue += delegate { return ModPrefs.GetInt(Name, "RumbleTimeMS", 25, true); };
                rumbleTime.SetValue += delegate (int value) { ModPrefs.SetInt(Name, "RumbleTimeMS", value); };


                IntViewController rumblePause = settingsSubmenu.AddInt("Rumble Interval\t(in ms)", 0, 250, 1);
                rumblePause.GetValue += delegate { return ModPrefs.GetInt(Name, "TimeBetweenRumblePulsesMS", 5, true); };
                rumblePause.SetValue += delegate (int value) { ModPrefs.SetInt(Name, "TimeBetweenRumblePulsesMS", value); };

            }
        }

		public void OnLevelWasLoaded(int level) { }

		public void OnLevelWasInitialized(int level) { }

		public void OnUpdate() { }

		public void OnFixedUpdate() { }

		public static int RumbleTimeMS
		{
			get
			{
				int Time = ModPrefs.GetInt(PluginName, "RumbleTimeMS", 25, true);
				return Mathf.Clamp(Time, 0, Time);
			}
		}

		public static int TimeBetweenRumblePulsesMS
		{
			get
			{
				int Time = ModPrefs.GetInt(PluginName, "TimeBetweenRumblePulsesMS", 5, true);
				return Mathf.Clamp(Time, 5, Time);
			}
		}

		public static float RumbleStrength
		{
			get
			{
				float Strength = ModPrefs.GetFloat(PluginName, "RumbleStrength", 1.0f, true);
				return Mathf.Clamp(Strength, 0.0f, 1.0f);
			}
		}
	}
}
