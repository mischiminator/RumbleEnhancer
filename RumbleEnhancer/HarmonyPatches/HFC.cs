using Harmony;
using System;
using UnityEngine.XR;

namespace Rumbleenhancer.HarmonyPatches
{

    [HarmonyPatch(typeof(HapticFeedbackController), "HitNote", new Type[] { typeof(XRNode) })]
    class HFC
    {
        static bool Prefix()
        {
            return false;
        }
    }
}
