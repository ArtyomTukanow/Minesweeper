using UnityEngine;
using View;

namespace Core
{
    public class Initer : MonoBehaviour
    {
        public BasePrefabs basePrefabs;
        private static Initer Self;

        private void Awake()
        {
            if (Self)
                Destroy(gameObject);

            Self = this;

#if UNITY_EDITOR
            Caching.ClearCache();
#endif
            QualitySettings.SetQualityLevel(0);

#if UNITY_WEBGL
			Application.runInBackground = true;
            Application.targetFrameRate = -1;
#else
            Application.targetFrameRate = 60;
#endif

#if UNITY_WSA || UNITY_WEBGL
			QualitySettings.vSyncCount = 1;
#else
            QualitySettings.vSyncCount = 0;
#endif
            QualitySettings.antiAliasing = 0;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            QualitySettings.shadows = ShadowQuality.Disable;
            QualitySettings.shadowCascades = 0;
            QualitySettings.shadowDistance = 0;

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void Start()
        {
            if (Game.Instance == null)
                Game.Create();

            Destroy(gameObject);
        }
    }
}