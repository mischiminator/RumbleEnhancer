namespace Rumbleenhancer
{
    internal class PluginConfig
    {
        bool     regenerateConfig            = true;
        public bool RegenerateConfig { get => regenerateConfig; set { regenerateConfig = value;} }

        string   plugin_name                 = "RumbleEnhancer";
        public string Plugin_Name { get => plugin_name; set { plugin_name = value;} }

        int      rumbleStrength              = 1;
        public int RumbleStrength { get => rumbleStrength; set { rumbleStrength = value;} }

        int      rumbleTimeMS                = 25;
        public int RumbleTimeMS { get => rumbleTimeMS; set { rumbleTimeMS = value;} }

        int      timeBetweenRumblePulsesMS   = 5;
        public int TimeBetweenRumblePulsesMS { get => timeBetweenRumblePulsesMS; set { timeBetweenRumblePulsesMS = value;} }

    }
}
