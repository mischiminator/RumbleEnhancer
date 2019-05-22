using System;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Rumbleenhancer
{
    class Settings
    {
        private int rumbleStrength = 1;
        private int rumbleTimeMS = 25;
        private int timeBetweenPulsesMS = 5;

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
                    "  RumbleStrength: 10,",
                    "  RumbleTimeMS: 25,",
                    "  TimeBetweenRumblePulsesMS: 5",
                    "}"
                };
                File.WriteAllLines(path, content);
            }
            try
            {
                jt = JToken.Parse(File.ReadAllText(path));
            }catch(Exception e)
            {
                Logger.log.Error(e.Message);
            }
            
            rumbleStrength = Convert.ToInt32(jt["RumbleStrength"].ToString());
            rumbleTimeMS = Convert.ToInt32(jt["RumbleTimeMS"].ToString());
            timeBetweenPulsesMS = Convert.ToInt32(jt["TimeBetweenRumblePulsesMS"].ToString());
            
        }

        public void SaveSettings()
        {
            try
            {
                string[] content =
                    {
                    "{",
                    "  RumbleStrength: "+rumbleStrength+",",
                    "  RumbleTimeMS: "+rumbleTimeMS+",",
                    "  TimeBetweenRumblePulsesMS: "+timeBetweenPulsesMS,
                    "}"
                };
                File.WriteAllLines(path, content);
            } catch(Exception e)
            {
                Logger.log.Error(e.Message);
            }
            Plugin.settingsattached = false;
        }

        public float RumbleStrength
        {
            get
            {
                float ret = rumbleStrength * 0.1f;
                return Mathf.Clamp(ret, 0.0f, 1.0f);
            }
            set
            {
                Logger.log.Info("RS set:" + value);
                rumbleStrength = (int)value;
                Plugin.save = true;
            }
        }

        public int RumbleStrengthSettings()
        {
            return rumbleStrength;
        }

        public int RumbleTimeMS
        {
            get
            {
                return Mathf.Clamp(rumbleTimeMS, 0, rumbleTimeMS);
            } 
            set
            {
                Logger.log.Info("RT set:" + value);
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
                Logger.log.Info("RP set:" + value);
                timeBetweenPulsesMS = value;
                Plugin.save = true;
            }
        }
    }
}
