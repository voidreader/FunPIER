using UnityEngine;
using UnityEditor;
using SA.Foundation.Editor;
using SA.Android.Vending.BillingClient;

namespace SA.Android.Editor
{
    class AN_VendingFeaturesUI : AN_ServiceSettingsUI
    {
        readonly GUIContent ProductIdDLabel = new GUIContent("Product Id[?]:", "A unique identifier that will be used for reporting. It can be composed of letters and numbers.");
        readonly GUIContent DisplayNameLabel = new GUIContent("Display Name[?]:", "This is the name of the Google Product that will be seen by customers (if this is their primary language). For automatically renewable subscriptions, don’t include a duration in the display name. The display name can’t be longer than 75 characters.");
        readonly GUIContent DescriptionLabel = new GUIContent("Description[?]:", "This is the description of the In-App Purchase that will be used by App Review during the review process. If indicated in your code, this description may also be seen by customers. For automatically renewable subscriptions, do not include a duration in the description. The description cannot be longer than 255 bytes.");
        readonly GUIContent ConsumableLabel = new GUIContent("Consumable[?]:", "Indicates if product can be consumed after purchase");
        readonly GUIContent ProductType = new GUIContent("ProductType[?]:", "Defines the product type");
        readonly GUIContent PriceLabel = new GUIContent("Price[?]:", "The price of the product");

        public override void OnAwake()
        {
            base.OnAwake();
            AddFeatureUrl("Console Setup", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Developer-Console-Setup");
            AddFeatureUrl("Connecting", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Connecting-to-The-Service");
            AddFeatureUrl("Define Products", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Connecting-to-The-Service");
            AddFeatureUrl("Products Availability", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Connecting-to-The-Service");
            AddFeatureUrl("Purchase Flow", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Purchase-Flow");
            AddFeatureUrl("Subscriptions", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Subscriptions");
            AddFeatureUrl("Upgrade a Subscription", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Subscriptions#upgrade-or-downgrade-a-subscription");
            AddFeatureUrl("User Inventory", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Purchase-Flow#user-inventory");
            AddFeatureUrl("Transactions Validation", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Transactions-Validation");
            AddFeatureUrl("Licensing", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Licensing");
        }

        public override string Title => "Vending";

        public override string Description => "A service provided by Google Play that lets you sell digital content from inside an Android app or “in-app.”";

        protected override Texture2D Icon => AN_Skin.GetIcon("android_vending.png");

        public override SA_iAPIResolver Resolver => AN_Preprocessor.GetResolver<AN_VendingResolver>();

        protected override void OnServiceUI()
        {
            if (!AN_Settings.Instance.Licensing && !AN_Settings.Instance.GooglePlayBilling)
                using (new SA_WindowBlockWithSpace(new GUIContent("Notice ")))
                {
                    EditorGUILayout.HelpBox("Service is enabled, but both Licensing &  Google Play Billing features " +
                        "are disabled. Please enable at least of the feature or turn the service off.",
                        MessageType.Error);
                }

            using (new SA_WindowBlockWithSpace(new GUIContent("Licensing ")))
            {
                EditorGUILayout.HelpBox("Licensing allows you to prevent unauthorized distribution of your App.", MessageType.Info);
                AN_Settings.Instance.Licensing = SA_EditorGUILayout.ToggleFiled("API Status", AN_Settings.Instance.Licensing, SA_StyledToggle.ToggleType.EnabledDisabled);

                if (AN_Settings.Instance.Licensing)
                {
                    EditorGUILayout.HelpBox("RSA public key will be used for app licensing. &  in-app billing purchases",
                        MessageType.Info);

                    EditorGUILayout.LabelField("Base64-encoded RSA public key");

                    AN_Settings.Instance.RSAPublicKey = EditorGUILayout.TextArea(AN_Settings.Instance.RSAPublicKey, SA_PluginSettingsWindowStyles.TextArea, GUILayout.Height(95));
                    AN_Settings.Instance.RSAPublicKey = AN_Settings.Instance.RSAPublicKey.Trim();
                }
            }

            using (new SA_WindowBlockWithSpace(new GUIContent("Google Play Billing ")))
            {
                EditorGUILayout.HelpBox("Google Play Billing is a service that lets you sell digital content on Android.", MessageType.Info);
                AN_Settings.Instance.GooglePlayBilling = SA_EditorGUILayout.ToggleFiled("API Status", AN_Settings.Instance.GooglePlayBilling, SA_StyledToggle.ToggleType.EnabledDisabled);

                if (AN_Settings.Instance.GooglePlayBilling)
                {
                    if (AN_Settings.Instance.InAppProducts.Count == 0)
                        EditorGUILayout.HelpBox("Use this menu to specify in-app products available for your App.", MessageType.Info);

                    SA_EditorGUILayout.ReorderablList(AN_Settings.Instance.InAppProducts, GetProductDisplayName, DrawProductContent, () =>
                    {
                        AN_Settings.Instance.AddInAppProduct("your.product.sku", AN_BillingClient.SkuType.inapp);
                    });
                }
            }
        }

        string GetProductDisplayName(AN_SkuDetails product)
        {
            return product.Title + "           " + product.Price + "$";
        }

        void DrawProductContent(AN_SkuDetails product)
        {
            product.Sku = SA_EditorGUILayout.TextField(ProductIdDLabel, product.Sku);
            product.Title = SA_EditorGUILayout.TextField(DisplayNameLabel, product.Title);
            product.Type = (AN_BillingClient.SkuType)SA_EditorGUILayout.EnumPopup(ProductType, product.Type);
            product.IsConsumable = SA_EditorGUILayout.ToggleFiled(ConsumableLabel, product.IsConsumable, SA_StyledToggle.ToggleType.YesNo);
            product.Price = SA_EditorGUILayout.TextField(PriceLabel, product.Price);

            EditorGUILayout.LabelField(DescriptionLabel);
            using (new SA_GuiBeginHorizontal())
            {
                product.Description = EditorGUILayout.TextArea(product.Description, GUILayout.Height(60), GUILayout.MinWidth(190));
                EditorGUILayout.Space();
                product.Icon = (Texture2D)EditorGUILayout.ObjectField("", product.Icon, typeof(Texture2D), false, GUILayout.Width(75));
            }
        }
    }
}
