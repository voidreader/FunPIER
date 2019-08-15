using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SA.Android.App;
using SA.Android.Content;
using SA.Android.Content.Pm;

using SA.Android.Social;
using SA.Android.Utilities;

using SA.Foundation.Utility;



public class AN_SocialExample : MonoBehaviour {

#pragma warning disable 649
    [SerializeField] Button m_shareText = null;
    [SerializeField] Button m_shareImage = null;
    [SerializeField] Button m_shareImages = null;

    [SerializeField] Button m_fbImage = null;
    [SerializeField] Button m_fbImages = null;

    [SerializeField] Button m_instaImage = null;
    [SerializeField] Button m_instaImages = null;

    [SerializeField] Button m_emil = null;

    [SerializeField] Button m_twitter = null;
    [SerializeField] Button m_whatsup = null;

    [SerializeField] Button m_instllCheck = null;
    [SerializeField] Button m_startFacebook = null;

    [SerializeField] Button m_composeSMS = null;

#pragma warning restore 649

    private void Awake() {


        m_shareText.onClick.AddListener(() => {
            var composer = new AN_ShareComposer();
            composer.SetTitle("Android Native Share Example");
            composer.SetText("Hello world");

            composer.Share();
        });

        m_shareImage.onClick.AddListener(() => {
            SA_ScreenUtil.TakeScreenshot((screenshot) => {
                /*
                var composer = new AN_ShareComposer();
                composer.SetTitle("Android Native Share Example");
                composer.SetText("Hello world");
                composer.AddImage(screenshot);

                composer.Share();*/

                ShowSharingMenuAndroid(screenshot);
            });
        });


        m_shareImages.onClick.AddListener(() => {
            SA_ScreenUtil.TakeScreenshot((screenshot) => {
                var composer = new AN_ShareComposer();
                composer.SetTitle("Android Native Share Example");
                composer.SetText("Hello world");
                composer.AddImage(screenshot);
                composer.AddImage(screenshot);

                composer.Share(() => {
                    Debug.Log("Sharing flow is finished, User has retured to the app");
                });
            });
        });

        m_fbImage.onClick.AddListener(() => {
            SA_ScreenUtil.TakeScreenshot((screenshot) => {
                var composer = new AN_FacebookSharing();
                composer.AddImage(screenshot);

                composer.Share();
            });
        });


        m_fbImages.onClick.AddListener(() => {
            SA_ScreenUtil.TakeScreenshot((screenshot) => {
                var composer = new AN_FacebookSharing();
                composer.AddImage(screenshot);
                composer.AddImage(screenshot);

                composer.Share(() => {
                    Debug.Log("Sharing flow is finished, User has retured to the app");
                });
            });
        });


        m_instaImage.onClick.AddListener(() => {
            SA_ScreenUtil.TakeScreenshot((screenshot) => {
                var composer = new AN_InstagramSharing();
                composer.AddImage(screenshot);

                composer.Share();
            });
        });


        m_instaImages.onClick.AddListener(() => {
            SA_ScreenUtil.TakeScreenshot((screenshot) => {
                var composer = new AN_InstagramSharing();
                composer.AddImage(screenshot);
                composer.AddImage(screenshot);

                composer.Share();
            });
        });

        m_emil.onClick.AddListener(() => {
            SA_ScreenUtil.TakeScreenshot((screenshot) => {
                var composer = new AN_EmailComposer();
               
                composer.SetText("Testing the emailssharing\n example");
                composer.SetSubject("Hello worls");

                composer.AddRecipient("ceo@stansassets.com");
                composer.AddRecipient("support@stansassets.com");

                composer.AddImage(screenshot);
               // composer.AddImage(screenshot);

                composer.Share(() => {
                    Debug.Log("Sharing flow is finished, User has retured to the app");
                });
            });
        });

        

        m_twitter.onClick.AddListener(() => {
            SA_ScreenUtil.TakeScreenshot((screenshot) => {
                var composer = new AN_TwitterSharing();
                composer.AddImage(screenshot);
                composer.SetText("My twitt");

                composer.Share();
            });
        });


        m_whatsup.onClick.AddListener(() => {
            SA_ScreenUtil.TakeScreenshot((screenshot) => {
                var composer = new AN_WhatsappSharing();
                composer.AddImage(screenshot);
                composer.AddImage(screenshot);
                composer.SetText("My message");

                composer.Share();
            });
        });


        m_instllCheck.onClick.AddListener(() => {
            AN_Logger.Log(AN_FacebookSharing.IsAppInstalled);
            

            var pm = AN_MainActivity.Instance.GetPackageManager();
            var info = pm.GetPackageInfo("random.app.name", AN_PackageManager.GET_ACTIVITIES);

            if(info == null) {
                AN_Logger.Log("not installed");
            } else {
                AN_Logger.Log("app installed");
            }
           

        });


        m_startFacebook.onClick.AddListener(() => {
            var pm = AN_MainActivity.Instance.GetPackageManager();
            AN_Intent startAppIntent = pm.GetLaunchIntentForPackage("com.facebook.katana");
            if(startAppIntent == null) {
                AN_Logger.Log("App with Id: com.facebook.katana not found on device");
                return;
            }
            startAppIntent.AddCategory(AN_Intent.CATEGORY_LAUNCHER);

            AN_MainActivity.Instance.StartActivity(startAppIntent);
        });


        m_composeSMS.onClick.AddListener(() => {

            string phoneNumber = "+380956808684";
            AN_Intent smsIntent = new AN_Intent(AN_Intent.ACTION_VIEW, new Uri("sms:" + phoneNumber));
            smsIntent.PutExtra("sms_body", "Hello, how are you?");
            smsIntent.PutExtra(AN_Intent.EXTRA_TEXT, "Hello, how are you ?");
            AN_MainActivity.Instance.StartActivity(smsIntent);
  
        });
    }


    private void Mail2() {
        SA_ScreenUtil.TakeScreenshot((screenshot) => {
            AN_Intent sendIntent = new AN_Intent();
            sendIntent.SetAction(AN_Intent.ACTION_SEND_MULTIPLE);
            sendIntent.PutExtra(AN_Intent.EXTRA_TEXT, "This is my text to send.");
            sendIntent.SetType(AN_MIMEDataType.Image);

            sendIntent.PutExtra(AN_Intent.EXTRA_EMAIL, "lacost.st@gmail.com", "stan@roomful.co");
            sendIntent.PutExtra(AN_Intent.EXTRA_TEXT, "EXTRA_TEXT2");
            sendIntent.PutExtra(AN_Intent.EXTRA_SUBJECT, "EXTRA_SUBJECT2s");
            var list = new List<Texture2D>() { screenshot, screenshot };
            sendIntent.PutExtra(AN_Intent.EXTRA_STREAM, list.ToArray());

            AN_Intent chooser = AN_Intent.CreateChooser(sendIntent, "My chooser title");
            AN_ProxyActivity proxy = new AN_ProxyActivity();
            proxy.StartActivityForResult(chooser, (result) => {
                Debug.Log("Unity: Activity Result: " + result.ResultCode.ToString());
                proxy.Finish();
            });
        });
          
    }


    private void Mail() {


        AN_Intent sendIntent = new AN_Intent();
        sendIntent.SetAction(AN_Intent.ACTION_SEND);
        sendIntent.PutExtra(AN_Intent.EXTRA_TEXT, "This is my text to send.");
        sendIntent.SetType("text/plain");
        sendIntent.SetAction(AN_MIMEDataType.Image);

        sendIntent.PutExtra(AN_Intent.EXTRA_EMAIL, "lacost.st@gmail.com", "stan@roomful.co");
        sendIntent.PutExtra(AN_Intent.EXTRA_TEXT, "EXTRA_TEXT2");
        sendIntent.PutExtra(AN_Intent.EXTRA_SUBJECT, "EXTRA_SUBJECT2s");


        AN_Intent chooser = AN_Intent.CreateChooser(sendIntent, "My chooser title");
        AN_ProxyActivity proxy = new AN_ProxyActivity();
        proxy.StartActivityForResult(chooser, (result) => {
            Debug.Log("Unity: Activity Result: " + result.ResultCode.ToString());
            proxy.Finish();
        });
    }


    public static void StartFilteredChooserActivity(Texture2D screenshot) {


        AN_Intent shareIntent = new AN_Intent();
        shareIntent.SetAction(AN_Intent.ACTION_SEND_MULTIPLE);
        shareIntent.SetType(AN_MIMEDataType.Image);

        shareIntent.PutExtra(AN_Intent.EXTRA_TEXT, "This is my text to send.");

        shareIntent.PutExtra(AN_Intent.EXTRA_EMAIL, "lacost.st@gmail.com", "stan@roomful.co");
        shareIntent.PutExtra(AN_Intent.EXTRA_TEXT, "EXTRA_TEXT2");
        shareIntent.PutExtra(AN_Intent.EXTRA_SUBJECT, "EXTRA_SUBJECT2s");
        var list = new List<Texture2D>() { screenshot, screenshot };
        shareIntent.PutExtra(AN_Intent.EXTRA_STREAM, list.ToArray());

        List<string> filters = new List<string>();
        filters.Add("mail");


        var pm = AN_MainActivity.Instance.GetPackageManager();

        List<AN_ResolveInfo> resolveInfoList = pm.QueryIntentActivities(shareIntent, 0);
       
        if (resolveInfoList.Count == 0) {
            Debug.Log("No apps found");
            return;
        }

        
        List<AN_Intent> intentShareList = new List<AN_Intent>();

        
        foreach (AN_ResolveInfo resInfo in resolveInfoList) {
            string packageName = resInfo.ActivityInfo.PackageName;
            string name = resInfo.ActivityInfo.Name;

            foreach (string filterPattern in filters) {

                AN_Logger.Log(resInfo.ActivityInfo.PackageName);

                if (resInfo.ActivityInfo.PackageName.ToLower().Contains(filterPattern) || resInfo.ActivityInfo.Name.ToLower().Contains(filterPattern) ) {
                    AN_Intent intent = new AN_Intent(shareIntent);
                    intent.SetPackage(packageName);
                   // intent.setComponent(new ComponentName(packageName, name));
                    intentShareList.Add(intent);
                    AN_Logger.Log("packageName: " + packageName);

                    shareIntent.SetPackage(packageName);
                }
            }
        }


        if (intentShareList.Count == 0) {
            AN_Logger.Log("CAN'T FIND PACKAGES FOR FILTER: " + filters);
        } else {
            AN_Logger.Log( "SHARE WITH FILTER: " + filters);
            AN_Intent chooserIntent = AN_Intent.CreateChooser(shareIntent, "Hello title", intentShareList.ToArray());
            AN_MainActivity.Instance.StartActivity(chooserIntent);
        }

       
        
    }


    void ShowSharingMenuAndroid(Texture2D imageToShare) {

        var Title  =  "Test Title";
        var AppLink = "Test AppLink";

        var PrioritizedApps = new List<string> { "whatsapp", "facebook" };

        AN_Intent shareIntent = new AN_Intent();
        shareIntent.SetAction(AN_Intent.ACTION_SEND);

        shareIntent.AddFlags(AN_Intent.FLAG_GRANT_READ_URI_PERMISSION);
        shareIntent.SetType(AN_MIMEDataType.Image);

        string sharingMessageText = "Sharing message text.";

        sharingMessageText = sharingMessageText + "\n\n" + AppLink;

        shareIntent.PutExtra(AN_Intent.EXTRA_TEXT, sharingMessageText);
        shareIntent.PutExtra(AN_Intent.EXTRA_STREAM, imageToShare);

        var pm = AN_MainActivity.Instance.GetPackageManager();
        List<AN_ResolveInfo> resolveInfoList = pm.QueryIntentActivities(shareIntent, 0);

        if (resolveInfoList.Count == 0) {
            AN_Logger.Log("Could not find any app for sharing.");

            return;
        }

        List<AN_Intent> intentShareList = new List<AN_Intent>();

        // Try to find apps from priority list to put them on the first positions
        foreach (string appName in PrioritizedApps) {
            foreach (AN_ResolveInfo resInfo in resolveInfoList) {
                string packageName = resInfo.ActivityInfo.PackageName;
                string name = resInfo.ActivityInfo.Name;

                if (resInfo.ActivityInfo.PackageName.ToLower().Contains(appName) ||
                    resInfo.ActivityInfo.Name.ToLower().Contains(appName)) {
                    AN_Intent intent = new AN_Intent(shareIntent);
                    intent.SetPackage(packageName);
                    intentShareList.Add(intent);

                    resolveInfoList.Remove(resInfo);

                    break;
                }
            }
        }

        // Put all the others apps after prioritized ones
        for (int resInfoIndex = 0; resInfoIndex < resolveInfoList.Count; resInfoIndex++) {
            AN_ResolveInfo resInfo = resolveInfoList[resInfoIndex];

            string packageName = resInfo.ActivityInfo.PackageName;
            string name = resInfo.ActivityInfo.Name;

            if (resInfoIndex != resolveInfoList.Count - 1) {
                AN_Intent intent = new AN_Intent(shareIntent);
                intent.SetPackage(packageName);
                intentShareList.Add(intent);
            } else {
                shareIntent.SetPackage(packageName);
            }
        }

        if (intentShareList.Count == 0) {
            AN_Logger.Log("Could not find any app for sharing.");
           
        } else {

            AN_Logger.Log("proxy.StartActivityForResult");

            AN_Intent chooserIntent = AN_Intent.CreateChooser(shareIntent, Title, intentShareList.ToArray());
           /* AN_ProxyActivity proxy = new AN_ProxyActivity();
            proxy.StartActivityForResult(chooserIntent, (result) => {
                Debug.Log("Unity: Activity Result: " + result.ResultCode.ToString());
                proxy.Finish();
            });*/
            AN_MainActivity.Instance.StartActivity(chooserIntent);
        }
    }
}
