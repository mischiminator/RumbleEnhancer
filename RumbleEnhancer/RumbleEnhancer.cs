using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR;

namespace Rumbleenhancer
{
    class RumbleEnhancer : MonoBehaviour
    {
        private const float SearchTimeout = 5000.0f;

        private HapticFeedbackController HapticFeedbackController = null;
        private NoteCutEffectSpawner NoteCutEffectSpawner = null;
        private BeatmapObjectSpawnController BeatmapObjectSpawnController = null;

        private Dictionary<Saber.SaberType, RumblingHand> RumblingHands = null;

        private void Awake()
        {
            Settings s = new Settings();
            float RumbleStrength = s.RumbleStrength;
            int RumbleTimeMS = s.RumbleTimeMS;
            int TimeBetweenRumblePulsesMS = s.TimeBetweenRumblePulsesMS;

            RumblingHands = new Dictionary<Saber.SaberType, RumblingHand>
        {
            { Saber.SaberType.SaberA, new RumblingHand(XRNode.LeftHand, RumbleStrength, RumbleTimeMS, TimeBetweenRumblePulsesMS) },
            { Saber.SaberType.SaberB, new RumblingHand(XRNode.RightHand, RumbleStrength, RumbleTimeMS, TimeBetweenRumblePulsesMS) }
        };
        }

        private IEnumerator Start()
        {
            yield return FindComponentInBeatSaber(SearchTimeout, (HapticFeedbackController Result) => HapticFeedbackController = Result);
            yield return FindComponentInBeatSaber(SearchTimeout, (NoteCutEffectSpawner Result) => NoteCutEffectSpawner = Result);

            if (HapticFeedbackController == null)
            {
                Logger.log.Error("Haptic Feedback Controller not found!");
                yield break;
            }

            if (NoteCutEffectSpawner == null)
            {
                Logger.log.Error("Note Cut Spawner not found!");
                yield break;
            }

            FieldInfo field = NoteCutEffectSpawner.GetType().GetField("_beatmapObjectSpawnController", BindingFlags.Instance | BindingFlags.NonPublic);
            BeatmapObjectSpawnController = field.GetValue(NoteCutEffectSpawner) as BeatmapObjectSpawnController;

            if (BeatmapObjectSpawnController == null)
            {
                Logger.log.Error("Beatmap object spawner not found");
                yield break;
            }

            BeatmapObjectSpawnController.noteWasCutEvent += BeatmapObjectSpawnController_noteWasCutEvent;
        }

        private void OnDestroy()
        {
            if (BeatmapObjectSpawnController != null)
            {
                BeatmapObjectSpawnController.noteWasCutEvent -= BeatmapObjectSpawnController_noteWasCutEvent;
            }

            foreach (RumblingHand RumblingHand in RumblingHands.Values)
            {
                RumblingHand.StopRumble();
            }
        }

        private void BeatmapObjectSpawnController_noteWasCutEvent(BeatmapObjectSpawnController NoteSpawnController, NoteController NoteController, NoteCutInfo NoteCutInfo)
        {
            if (HapticFeedbackController == null)
            {
                return;
            }

            RumblingHands[NoteCutInfo.saberType].Rumble();
            return;
        }

        private IEnumerator FindComponentInBeatSaber<T>(float TimeOut, Action<T> OnComplete) where T : UnityEngine.Object
        {
            T Result = null;
            float TimeLeft = TimeOut;

            while (TimeLeft > 0.0f)
            {
                Result = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
                if (Result != null)
                {
                    break;
                }

                TimeLeft -= Time.deltaTime;
                yield return null;
            }

            OnComplete?.Invoke(Result);
        }
    }
}
