using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.XR;

namespace Rumbleenhancer
{
    class RumblingHand
    {
        private XRNode RumbleHand;
        private float RumbleStrength;
        private int RumbleTimeMS;
        private int TimeBetweenRumblePulsesMS;

        private bool RumbleActive;
        private int PassedRumbleTimeMS;

        private object LockObject;

        public RumblingHand(XRNode RumbleHand, float RumbleStrength, int RumbleTimeMS, int TimeBetweenRumblePulsesMS)
        {
            this.RumbleHand = RumbleHand;
            this.RumbleStrength = RumbleStrength;
            this.RumbleTimeMS = RumbleTimeMS;
            this.TimeBetweenRumblePulsesMS = TimeBetweenRumblePulsesMS;

            LockObject = new object();

            StopRumble();
        }

        public void ResetTime()
        {
            PassedRumbleTimeMS = 0;
        }

        public void StopRumble()
        {
            RumbleActive = false;
            ResetTime();
        }

        public void Rumble()
        {
            if (RumbleTimeMS == 0 || RumbleStrength == 0)
            {
                return;
            }

            lock (LockObject)
            {
                if (RumbleActive)
                {
                    ResetTime();
                    return;
                }
            }

            RumbleActive = true;

            Task.Run(() =>
            {
                while (true)
                {
                    lock (LockObject)
                    {
                        if (!RumbleActive || PassedRumbleTimeMS >= RumbleTimeMS)
                        {
                            StopRumble();
                            break;
                        }
                    }

                    VRPlatformHelper.instance.TriggerHapticPulse(RumbleHand, RumbleStrength);
                    Thread.Sleep(TimeBetweenRumblePulsesMS);

                    PassedRumbleTimeMS += TimeBetweenRumblePulsesMS;
                }
            });
        }
    }
}
