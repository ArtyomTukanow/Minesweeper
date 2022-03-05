using System.Collections;
using Core;
using Google.Play.Review;
using UnityEngine;

namespace Controller
{
    public class RateUsController : MonoBehaviour
    {
#if UNITY_ANDROID
        private ReviewManager reviewManager;
        private PlayReviewInfo playReviewInfo;
#endif

        public void PrepareRateUs()
        {
#if UNITY_ANDROID
            StartCoroutine(PrepareReview());
#endif
        }

#if UNITY_ANDROID
        private IEnumerator PrepareReview()
        {
            reviewManager = new ReviewManager();

            var requestFlowOperation = reviewManager.RequestReviewFlow();

            yield return requestFlowOperation;

            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                Debug.LogError($"Cant load requestInfo: {requestFlowOperation.Error}");
                yield break;
            }

            playReviewInfo = requestFlowOperation.GetResult();
        }
#endif

        public void ShowRateUs()
        {
#if UNITY_ANDROID
            StartCoroutine(StartReview());
#endif
        }

#if UNITY_ANDROID
        private IEnumerator StartReview()
        {
            if (playReviewInfo == null)
                yield return PrepareReview();

            if (playReviewInfo == null)
                yield break;

            var launchFlowOperation = reviewManager.LaunchReviewFlow(playReviewInfo);
			
            yield return launchFlowOperation;

            playReviewInfo = null;

            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                Debug.LogError($"Cant load requestInfo: {launchFlowOperation.Error}");
            }
        }
#endif
    }
}