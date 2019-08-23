using UnityEngine;
using System;

namespace AudienceNetwork
{
    public sealed class NativeAd : NativeAdBase, IDisposable
    {
        public NativeAd(string placementId) : base(placementId)
        {
            nativeAdType = NativeAdType.NativeAd;
            uniqueId = NativeAdBridgeInstance().Create(placementId, this);
        }

        public int RegisterGameObjectsForInteraction(RectTransform mediaViewRectTransform, RectTransform ctaRectTransform,
                RectTransform iconViewRectTransform = null, Camera camera = null)
        {
            return BaseRegisterGameObjectsForInteraction(mediaViewRectTransform, ctaRectTransform,
                    iconViewRectTransform, camera);
        }
    }
}
