using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using AOT;

namespace AudienceNetwork
{
    public sealed class NativeBannerAd : NativeAdBase, IDisposable
    {
        public NativeBannerAd(string placementId) : base(placementId)
        {
            nativeAdType = NativeAdType.NativeBannerAd;
            uniqueId = NativeBannerAdBridge.Instance.Create(placementId, this);
        }

        public int RegisterGameObjectsForInteraction(RectTransform iconViewRectTransform, RectTransform ctaRectTransform,
                Camera camera = null)
        {
            return BaseRegisterGameObjectsForInteraction(null, ctaRectTransform, iconViewRectTransform, camera);
        }

        internal override NativeAdBridge NativeAdBridgeInstance()
        {
            return NativeBannerAdBridge.Instance;
        }
    }

    internal class NativeBannerAdBridge : NativeAdBridge
    {
        public static new NativeAdBridge Instance;

        static NativeBannerAdBridge()
        {
            Instance = NativeBannerAdBridge.CreateInstance();
        }

        private static NativeAdBridge CreateInstance()
        {
            if (Application.platform != RuntimePlatform.OSXEditor)
            {
#if UNITY_IOS
                return new NativeBannerAdBridgeIOS();
#elif UNITY_ANDROID
                return new NativeAdBridgeAndroid();
#endif
            }
            return new NativeAdBridge();
        }
    }

#if UNITY_IOS
    internal class NativeBannerAdBridgeIOS : NativeBannerAdBridge
    {
        private static Dictionary<int, NativeAdContainer> nativeBannerAds = new Dictionary<int, NativeAdContainer> ();

        private static NativeAdContainer nativeBannerAdContainerForNativeBannerAdId(int uniqueId)
        {
            NativeAdContainer nativeBannerAd = null;
            bool success = NativeBannerAdBridgeIOS.nativeBannerAds.TryGetValue(uniqueId, out nativeBannerAd);
            if (success) {
                return nativeBannerAd;
            } else {
                return null;
            }
        }

        [DllImport("__Internal")]
        private static extern int FBNativeBannerAdBridgeCreate(string placementId);

        [DllImport("__Internal")]
        private static extern int FBNativeBannerAdBridgeLoad(int uniqueId);

        [DllImport("__Internal")]
        private static extern int FBNativeBannerAdBridgeLoadWithBidPayload(int uniqueId, string bidPayload);

        [DllImport("__Internal")]
        private static extern int FBNativeBannerAdBridgeRegisterViewsForInteraction(int uniqueId,
                int iconViewX, int iconViewY, int iconViewWidth, int iconViewHeight,
                int ctaViewX, int ctaViewY, int ctaViewWidth, int ctaViewHeight);

        [DllImport("__Internal")]
        private static extern bool FBNativeBannerAdBridgeIsValid(int uniqueId);

        [DllImport("__Internal")]
        private static extern string FBNativeBannerAdBridgeGetAdvertiserName(int uniqueId);

        [DllImport("__Internal")]
        private static extern string FBNativeBannerAdBridgeGetHeadline(int uniqueId);

        [DllImport("__Internal")]
        private static extern string FBNativeBannerAdBridgeGetLinkDescription(int uniqueId);

        [DllImport("__Internal")]
        private static extern string FBNativeBannerAdBridgeGetSponsoredTranslation(int uniqueId);

        [DllImport("__Internal")]
        private static extern string FBNativeBannerAdBridgeGetAdTranslation(int uniqueId);

        [DllImport("__Internal")]
        private static extern string FBNativeBannerAdBridgeGetPromotedTranslation(int uniqueId);

        [DllImport("__Internal")]
        private static extern string FBNativeBannerAdBridgeGetBody(int uniqueId);

        [DllImport("__Internal")]
        private static extern string FBNativeBannerAdBridgeGetCallToAction(int uniqueId);

        [DllImport("__Internal")]
        private static extern string FBNativeBannerAdBridgeGetSocialContext(int uniqueId);

        [DllImport("__Internal")]
        private static extern string FBNativeBannerAdBridgeGetAdChoicesImageURL(int uniqueId);

        [DllImport("__Internal")]
        private static extern string FBNativeBannerAdBridgeGetAdChoicesText(int uniqueId);

        [DllImport("__Internal")]
        private static extern string FBNativeBannerAdBridgeGetAdChoicesLinkURL(int uniqueId);

        [DllImport("__Internal")]
        private static extern void FBNativeBannerAdBridgeSetExtraHints(int uniqueId, string extraHints);

        [DllImport("__Internal")]
        private static extern void FBNativeBannerAdBridgeRelease(int uniqueId);

        [DllImport("__Internal")]
        private static extern void FBNativeBannerAdBridgeOnLoad(int uniqueId, FBNativeAdBridgeExternalCallback callback);

        [DllImport("__Internal")]
        private static extern void FBNativeBannerAdBridgeOnImpression(int uniqueId, FBNativeAdBridgeExternalCallback callback);

        [DllImport("__Internal")]
        private static extern void FBNativeBannerAdBridgeOnClick(int uniqueId, FBNativeAdBridgeExternalCallback callback);

        [DllImport("__Internal")]
        private static extern void FBNativeBannerAdBridgeOnError(int uniqueId, FBNativeAdBridgeErrorExternalCallback callback);

        [DllImport("__Internal")]
        private static extern void FBNativeBannerAdBridgeOnFinishedClick(int uniqueId, FBNativeAdBridgeExternalCallback callback);

        [DllImport("__Internal")]
        private static extern void FBNativeBannerAdBridgeOnMediaDownloaded(int uniqueId, FBNativeAdBridgeExternalCallback callback);

        public override int Create(string placementId, NativeAdBase nativeBannerAd)
        {
            int uniqueId = NativeBannerAdBridgeIOS.FBNativeBannerAdBridgeCreate(placementId);
            NativeBannerAdBridgeIOS.nativeBannerAds.Add(uniqueId, new NativeAdContainer(nativeBannerAd));
            NativeBannerAdBridgeIOS.FBNativeBannerAdBridgeOnLoad(uniqueId, nativeBannerAdDidLoadBridgeCallback);
            NativeBannerAdBridgeIOS.FBNativeBannerAdBridgeOnImpression(uniqueId, nativeBannerAdWillLogImpressionmpressionBridgeCallback);
            NativeBannerAdBridgeIOS.FBNativeBannerAdBridgeOnClick(uniqueId, nativeBannerAdDidClickBridgeCallback);
            NativeBannerAdBridgeIOS.FBNativeBannerAdBridgeOnError(uniqueId, nativeBannerAdDidFailWithErrorBridgeCallback);
            NativeBannerAdBridgeIOS.FBNativeBannerAdBridgeOnFinishedClick(uniqueId, nativeBannerAdDidFinishHandlingClickBridgeCallback);
            NativeBannerAdBridgeIOS.FBNativeBannerAdBridgeOnMediaDownloaded(uniqueId, nativeBannerAdDidDownloadMediaBridgeCallback);

            return uniqueId;
        }

        public override int Load(int uniqueId)
        {
            return NativeBannerAdBridgeIOS.FBNativeBannerAdBridgeLoad(uniqueId);
        }

        public override int Load(int uniqueId, string bidPayload)
        {
            return NativeBannerAdBridgeIOS.FBNativeBannerAdBridgeLoadWithBidPayload(uniqueId, bidPayload);
        }

        public override int RegisterGameObjectsForInteraction(int uniqueId, Rect mediaViewRect, Rect iconViewRect, Rect ctaViewRect)
        {
            return NativeBannerAdBridgeIOS.FBNativeBannerAdBridgeRegisterViewsForInteraction(uniqueId,
                    (int)iconViewRect.x, (int)iconViewRect.y, (int)iconViewRect.width, (int)iconViewRect.height,
                    (int)ctaViewRect.x, (int)ctaViewRect.y, (int)ctaViewRect.width, (int)ctaViewRect.height);
        }

        public override bool IsValid(int uniqueId)
        {
            return NativeBannerAdBridgeIOS.FBNativeBannerAdBridgeIsValid(uniqueId);
        }

        public override string GetAdvertiserName(int uniqueId)
        {
            return NativeBannerAdBridgeIOS.FBNativeBannerAdBridgeGetAdvertiserName(uniqueId);
        }

        public override string GetHeadline(int uniqueId)
        {
            return NativeBannerAdBridgeIOS.FBNativeBannerAdBridgeGetHeadline(uniqueId);
        }

        public override string GetLinkDescription(int uniqueId)
        {
            return NativeBannerAdBridgeIOS.FBNativeBannerAdBridgeGetLinkDescription(uniqueId);
        }

        public override string GetSponsoredTranslation(int uniqueId)
        {
            return NativeBannerAdBridgeIOS.FBNativeBannerAdBridgeGetSponsoredTranslation(uniqueId);
        }

        public override string GetAdTranslation(int uniqueId)
        {
            return NativeBannerAdBridgeIOS.FBNativeBannerAdBridgeGetAdTranslation(uniqueId);
        }

        public override string GetPromotedTranslation(int uniqueId)
        {
            return NativeBannerAdBridgeIOS.FBNativeBannerAdBridgeGetPromotedTranslation(uniqueId);
        }

        public override string GetBody(int uniqueId)
        {
            return NativeBannerAdBridgeIOS.FBNativeBannerAdBridgeGetBody(uniqueId);
        }

        public override string GetCallToAction(int uniqueId)
        {
            return NativeBannerAdBridgeIOS.FBNativeBannerAdBridgeGetCallToAction(uniqueId);
        }

        public override string GetSocialContext(int uniqueId)
        {
            return NativeBannerAdBridgeIOS.FBNativeBannerAdBridgeGetSocialContext(uniqueId);
        }

        public override string GetAdChoicesImageURL(int uniqueId)
        {
            return NativeBannerAdBridgeIOS.FBNativeBannerAdBridgeGetAdChoicesImageURL(uniqueId);
        }

        public override string GetAdChoicesText(int uniqueId)
        {
            return NativeBannerAdBridgeIOS.FBNativeBannerAdBridgeGetAdChoicesText(uniqueId);
        }

        public override string GetAdChoicesLinkURL(int uniqueId)
        {
            return NativeBannerAdBridgeIOS.FBNativeBannerAdBridgeGetAdChoicesLinkURL(uniqueId);
        }

        public override void SetExtraHints(int uniqueId, ExtraHints extraHints)
        {
            NativeBannerAdBridgeIOS.FBNativeBannerAdBridgeSetExtraHints(uniqueId, JsonUtility.ToJson(extraHints));
        }

        public override void Release(int uniqueId)
        {
            NativeBannerAdBridgeIOS.nativeBannerAds.Remove(uniqueId);
            NativeBannerAdBridgeIOS.FBNativeBannerAdBridgeRelease(uniqueId);
        }

        // Sets up internal managed callbacks

        public override void OnLoad(int uniqueId,
                                    FBNativeAdBridgeCallback callback)
        {
            NativeAdContainer container = NativeBannerAdBridgeIOS.nativeBannerAdContainerForNativeBannerAdId(uniqueId);
            if (container) {
                container.onLoad = (delegate() {
                    container.nativeAd.LoadAdFromData();
                });
            }
        }

        public override void OnImpression(int uniqueId,
                                          FBNativeAdBridgeCallback callback)
        {
            NativeAdContainer container = NativeBannerAdBridgeIOS.nativeBannerAdContainerForNativeBannerAdId(uniqueId);
            if (container) {
                container.onImpression = callback;
            }
        }

        public override void OnClick(int uniqueId,
                                     FBNativeAdBridgeCallback callback)
        {
            NativeAdContainer container = NativeBannerAdBridgeIOS.nativeBannerAdContainerForNativeBannerAdId(uniqueId);
            if (container) {
                container.onClick = callback;
            }
        }

        public override void OnError(int uniqueId,
                                     FBNativeAdBridgeErrorCallback callback)
        {
            NativeAdContainer container = NativeBannerAdBridgeIOS.nativeBannerAdContainerForNativeBannerAdId(uniqueId);
            if (container) {
                container.onError = callback;
            }
        }

        public override void OnFinishedClick(int uniqueId,
                                             FBNativeAdBridgeCallback callback)
        {
            NativeAdContainer container = NativeBannerAdBridgeIOS.nativeBannerAdContainerForNativeBannerAdId(uniqueId);
            if (container) {
                container.onFinishedClick = callback;
            }
        }

        public override void OnMediaDownloaded(int uniqueId, FBNativeAdBridgeCallback callback)
        {
            NativeAdContainer container = NativeBannerAdBridgeIOS.nativeBannerAdContainerForNativeBannerAdId(uniqueId);
            if (container) {
                container.onMediaDownload = callback;
            }
        }

        // External unmanaged callbacks (must be static)

        [MonoPInvokeCallback(typeof(FBNativeAdBridgeExternalCallback))]
        private static void nativeBannerAdDidLoadBridgeCallback(int uniqueId)
        {
            NativeAdContainer container = NativeBannerAdBridgeIOS.nativeBannerAdContainerForNativeBannerAdId(uniqueId);
            if (container && container.onLoad != null) {
                container.onLoad();
            }
        }

        [MonoPInvokeCallback(typeof(FBNativeAdBridgeExternalCallback))]
        private static void nativeBannerAdWillLogImpressionmpressionBridgeCallback(int uniqueId)
        {
            NativeAdContainer container = NativeBannerAdBridgeIOS.nativeBannerAdContainerForNativeBannerAdId(uniqueId);
            if (container && container.onImpression != null) {
                container.onImpression();
            }
        }

        [MonoPInvokeCallback(typeof(FBNativeAdBridgeErrorExternalCallback))]
        private static void nativeBannerAdDidFailWithErrorBridgeCallback(int uniqueId, string error)
        {
            NativeAdContainer container = NativeBannerAdBridgeIOS.nativeBannerAdContainerForNativeBannerAdId(uniqueId);
            if (container && container.onError != null) {
                container.onError(error);
            }
        }

        [MonoPInvokeCallback(typeof(FBNativeAdBridgeExternalCallback))]
        private static void nativeBannerAdDidClickBridgeCallback(int uniqueId)
        {
            NativeAdContainer container = NativeBannerAdBridgeIOS.nativeBannerAdContainerForNativeBannerAdId(uniqueId);
            if (container && container.onClick != null) {
                container.onClick();
            }
        }

        [MonoPInvokeCallback(typeof(FBNativeAdBridgeExternalCallback))]
        private static void nativeBannerAdDidFinishHandlingClickBridgeCallback(int uniqueId)
        {
            NativeAdContainer container = NativeBannerAdBridgeIOS.nativeBannerAdContainerForNativeBannerAdId(uniqueId);
            if (container && container.onFinishedClick != null) {
                container.onFinishedClick();
            }
        }

        [MonoPInvokeCallback(typeof(FBNativeAdBridgeExternalCallback))]
        private static void nativeBannerAdDidDownloadMediaBridgeCallback(int uniqueId)
        {
            NativeAdContainer container = NativeBannerAdBridgeIOS.nativeBannerAdContainerForNativeBannerAdId(uniqueId);
            if (container && container.onMediaDownload != null) {
                container.onMediaDownload();
            }
        }
    }
#endif
}
