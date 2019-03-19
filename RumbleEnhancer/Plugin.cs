using IllusionPlugin;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine;

namespace RumbleEnhancer
{
	public class Plugin : IPlugin
	{
		public const string PluginName = "RumbleEnhancer";

		public string Name => PluginName;
		public string Version => "1.1.0";

		private readonly string[] GameplaySceneNames = { "Menu", "GameCore" };

		public void OnApplicationStart()
		{
			SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
			SceneManager.sceneLoaded += SceneManager_sceneLoaded;
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

		private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1) { }

		public void OnLevelWasLoaded(int level) { }

		public void OnLevelWasInitialized(int level) { }

		public void OnUpdate() { }

		public void OnFixedUpdate() { }

		public static int RumbleTimeMS
		{
			get
			{
				int Time = ModPrefs.GetInt(PluginName, "RumbleTimeMS", 250, true);
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
