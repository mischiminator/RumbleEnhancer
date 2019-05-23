using System;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Rumbleenhancer
{
    class Settings
    {
        private int rumbleStrength = 1;
        private int rumbleTimeMS = 130;
        private int timeBetweenPulsesMS = 13;

        private JToken jt;

        private readonly string path = Path.Combine(Environment.CurrentDirectory, "UserData/Rumble Enhancer.json");
        public Settings()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            if (!File.Exists(path))
            {
                string[] content =
                {
                    "{",
                    "  RumbleTimeMS: 130,",
                    "  TimeBetweenRumblePulsesMS: 13",
                    "}"
                };
                File.WriteAllLines(path, content);
            }

            jt = JToken.Parse(File.ReadAllText(path));

            if (jt["RumbleStrenth"] != null)
                File.Delete(path);

            rumbleTimeMS = Convert.ToInt32(jt["RumbleTimeMS"].ToString());
            timeBetweenPulsesMS = Convert.ToInt32(jt["TimeBetweenRumblePulsesMS"].ToString());

        }

        public void SaveSettings()
        {
            string[] content =
                {
                    "{",
                    "  RumbleTimeMS: "+rumbleTimeMS+",",
                    "  TimeBetweenRumblePulsesMS: "+timeBetweenPulsesMS,
                    "}"
                };
            File.WriteAllLines(path, content);

            Plugin.settingsattached = false;
        }

        public float RumbleStrength
        {
            get
            {
                return rumbleStrength;
            }
            set
            {
                rumbleStrength = (int)value;
                Plugin.save = true;
            }
        }
        
        public int RumbleTimeMS
        {
            get
            {
                return Mathf.Clamp(rumbleTimeMS, 0, rumbleTimeMS);
            }
            set
            {
                rumbleTimeMS = value;
                Plugin.save = true;
            }
        }

        public int TimeBetweenRumblePulsesMS
        {
            get
            {
                return Mathf.Clamp(timeBetweenPulsesMS, 5, timeBetweenPulsesMS);
            }
            set
            {
                timeBetweenPulsesMS = value;
                Plugin.save = true;
            }
        }
    }
}
