using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Editor;
using SA.Android.Vending.Billing;


namespace SA.Android
{

    public class AN_VendingFeaturesUI : AN_ServiceSettingsUI
    {


        GUIContent ProductIdDLabel = new GUIContent("Product Id[?]:", "A unique identifier that will be used for reporting. It can be composed of letters and numbers.");
        GUIContent DisplayNameLabel = new GUIContent("Display Name[?]:", "This is the name of the Google Product that will be seen by customers (if this is their primary language). For automatically renewable subscriptions, don’t include a duration in the display name. The display name can’t be longer than 75 characters.");
        GUIContent DescriptionLabel = new GUIContent("Description[?]:", "This is the description of the In-App Purchase that will be used by App Review during the review process. If indicated in your code, this description may also be seen by customers. For automatically renewable subscriptions, do not include a duration in the description. The description cannot be longer than 255 bytes.");
        GUIContent ConsumableLabel = new GUIContent("Consumable[?]:", "Indicates if product can be consumed from the invetory after purchase");

        GUIContent ProductType = new GUIContent("ProductType[?]:", "Defines the produyct type");
        GUIContent PriceLabel = new GUIContent("Price[?]:", "The price of the product");


        public override void OnAwake() {
            base.OnAwake();

            AddFeatureUrl("Console Setup", "https://unionassets.com/android-native-pro/developer-console-setup-673");
            AddFeatureUrl("Connecting", "https://unionassets.com/android-native-pro/connecting-to-the-service-674");
            AddFeatureUrl("Define Products", "https://unionassets.com/android-native-pro/connecting-to-the-service-674#define%C2%A0in-app-products-via-editor-ui");
            AddFeatureUrl("Products Availability", "https://unionassets.com/android-native-pro/connecting-to-the-service-674#products-availability");
            AddFeatureUrl("Purchase Flow", "https://unionassets.com/android-native-pro/purchase-flow-675");
            AddFeatureUrl("Subscriptions", "https://unionassets.com/android-native-pro/subscriptions-826");
            AddFeatureUrl("Upgrade a Subscription", "https://unionassets.com/android-native-pro/subscriptions-826#upgrade-or-downgrade-a-subscription");
            AddFeatureUrl("User Inventory", "https://unionassets.com/android-native-pro/purchase-flow-675#Query-cached");
            AddFeatureUrl("Transactions Validation", "https://unionassets.com/android-native-pro/transactions-validation-676");
            AddFeatureUrl("Licensing", "https://unionassets.com/android-native-pro/licensing-677");
      }


        public override string Title {
            get {
                return "Vending";
            }
        }

        public override string Description {
            get {
                return "A service provided by Google Play that lets you sell digital content from inside an Android app or “in-app.”";
            }
        }


        protected override Texture2D Icon {
            get {
                return AN_Skin.GetIcon("android_vending.png");
            }
        }

        public override SA_iAPIResolver Resolver {
            get {
                return AN_Preprocessor.GetResolver<AN_VendingResolver>();
            }
        }

        protected override void OnServiceUI() {
            using (new SA_WindowBlockWithSpace(new GUIContent("Your license key"))) {
                EditorGUILayout.HelpBox("RSA public key will be used for app licensing. &  in-app billing purchases",
                                        MessageType.Info);

                EditorGUILayout.LabelField("Base64-encoded RSA public key");

                AN_Settings.Instance.RSAPublicKey = EditorGUILayout.TextArea(AN_Settings.Instance.RSAPublicKey, SA_PluginSettingsWindowStyles.TextArea, GUILayout.Height(95));
                AN_Settings.Instance.RSAPublicKey.Trim();
            }

            using (new SA_WindowBlockWithSpace(new GUIContent("Licensing "))) {
                EditorGUILayout.HelpBox("Licensing allows you to prevent unauthorized distribution of your App.", MessageType.Info);
                AN_Settings.Instance.Licensing = SA_EditorGUILayout.ToggleFiled("API Status", AN_Settings.Instance.Licensing, SA_StyledToggle.ToggleType.EnabledDisabled);
            }

            using (new SA_WindowBlockWithSpace(new GUIContent("In-App Products List"))) {

                if(AN_Settings.Instance.InAppProducts.Count == 0) {
                    EditorGUILayout.HelpBox("Use this menu to specify in-app products available for your App.", MessageType.Info);
                }

                SA_EditorGUILayout.ReorderablList(AN_Settings.Instance.InAppProducts, GetProductDisplayName, DrawProductContent, () => {

                    var product = new AN_Product("your.product.sku", AN_ProductType.inapp);
                    product.Title = "New Product";
                    AN_Settings.Instance.InAppProducts.Add(product);
                });
            }

        }

        private string GetProductDisplayName(AN_Product product) {
            return product.Title + "           " + product.Price + "$";
        }

        private void DrawProductContent(AN_Product product) {

            product.ProductId = SA_EditorGUILayout.TextField(ProductIdDLabel, product.ProductId);
            product.Title = SA_EditorGUILayout.TextField(DisplayNameLabel, product.Title);
            product.Type =  (AN_ProductType) SA_EditorGUILayout.EnumPopup(ProductType, product.Type);
            product.IsConsumable = SA_EditorGUILayout.ToggleFiled(ConsumableLabel, product.IsConsumable, SA_StyledToggle.ToggleType.YesNo);

            product.Price = SA_EditorGUILayout.TextField(PriceLabel, product.Price);
            

            EditorGUILayout.LabelField(DescriptionLabel);
            using(new SA_GuiBeginHorizontal()) {
                product.Description = EditorGUILayout.TextArea(product.Description,  GUILayout.Height(60), GUILayout.MinWidth(190));
                EditorGUILayout.Space();
                product.Icon = (Texture2D)EditorGUILayout.ObjectField("", product.Icon, typeof(Texture2D), false, GUILayout.Width(75));
            }
        }
    }
}