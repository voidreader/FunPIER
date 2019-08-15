using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.App;
using SA.Android.Content;
using SA.Android.Content.Pm;
using SA.Android.Manifest;
using SA.Android.Utilities;

using SA.Foundation.Async;

namespace SA.Android.Social
{
    public abstract class AN_SocialShareBuilder 
    {
        protected string m_text = string.Empty;
        protected List<Texture2D> m_images = new List<Texture2D>();
        private AN_Intent m_shareIntent;
        private string m_packageName = string.Empty;

        protected abstract AN_Intent MakeSharingIntent();
        private AN_ProxyActivity m_proxyActivity;


        //--------------------------------------
        // Public Methods
        //--------------------------------------


        public void Share(Action callback = null) {

            if(Application.isEditor) {

                SA_Coroutine.WaitForSeconds(1, () => {
                    if (callback != null) { callback.Invoke(); }
                });
                
                return;
            }
            
            AN_PermissionsUtility.TryToResolvePermission(new []{AMM_ManifestPermission.WRITE_EXTERNAL_STORAGE, AMM_ManifestPermission.READ_EXTERNAL_STORAGE}, (granted) =>
            {
                if (!granted)
                {
                    AN_Logger.LogError("User haven't granted required sharing permissions: " +
                                       "WRITE_EXTERNAL_STORAGE, READ_EXTERNAL_STORAGE. " +
                                       "Sharing may not be fully completed.");
                }
                ShowSharingDialog(callback);
            });

        }

        private void ShowSharingDialog(Action callback = null)
        {
            GenerateShareIntent();
            var intentToShare = MakeSharingIntent();

            //let's see if we have a package name set
            if(!string.IsNullOrEmpty(m_packageName)) {
                //Let's check if app with this package actually exists
                var pm = AN_MainActivity.Instance.GetPackageManager();
                var info = pm.GetPackageInfo(m_packageName, AN_PackageManager.GET_ACTIVITIES);
                if(info == null) {
                    //there is no requested app installed.
                    if (callback != null) {
                        callback.Invoke();
                    }

                    return;
                }
            }

            m_proxyActivity = new AN_ProxyActivity();
            m_proxyActivity.StartActivityForResult(intentToShare, (result) => {
                if (callback != null) {
                    callback.Invoke();
                }
            });
        }


        //--------------------------------------
        // Protected Methods
        //--------------------------------------

        protected void SetPackage(String packageName) {
            m_packageName = packageName;
            ShareIntent.SetPackage(m_packageName);
        }



        protected virtual void GenerateShareIntent() {
            if (!string.IsNullOrEmpty(m_text)) {
                AppendText();
            }

            if (m_images.Count > 0) {
                AppendImages();
            }

            ShareIntent.AddFlags(AN_Intent.FLAG_ACTIVITY_NEW_DOCUMENT);
        }


        protected AN_Intent GenerateChooserIntent(string title, params string[] filters) {

            if(filters.Length == 0) {
                return AN_Intent.CreateChooser(ShareIntent, title);
            }

            //Simple intent to get list of the apps that can support the send action
            var testIntent = new AN_Intent();
            testIntent.SetAction(AN_Intent.ACTION_SEND);
            testIntent.SetType("text/plain");


            var pm = AN_MainActivity.Instance.GetPackageManager();
            var resolveInfoList = pm.QueryIntentActivities(testIntent);

            var intentShareList = new List<AN_Intent>();

            foreach (var resInfo in resolveInfoList) {
                var packageName = resInfo.ActivityInfo.PackageName;
                foreach (var filterPattern in filters) {
                    if (resInfo.ActivityInfo.PackageName.ToLower().Contains(filterPattern) || resInfo.ActivityInfo.Name.ToLower().Contains(filterPattern)) {
                        //TODO do we need full data or only package name
                        var intent = new AN_Intent(ShareIntent);
                        intent.SetPackage(packageName);
                        intentShareList.Add(intent);
                        break;
                    }
                }
            }

            if (intentShareList.Count == 0) {
                //we can't find packages for a provided filters, so we will use standard chooser
                string filterList = "";
                foreach(var f in filters) {
                    filterList += f + ";"; 
                }
                AN_Logger.Log("Wasn't able to find packages for filters: " + filterList);
                return AN_Intent.CreateChooser(ShareIntent, title);
            } else {
                AN_Logger.Log("Chooser created with options count: " + intentShareList.Count);
                
                //if we have only 1 option there is no point to create hole chooser UI. Let's just use this option
                if (intentShareList.Count ==  1) {
                    return intentShareList[0];
                } 

                return AN_Intent.CreateChooser(ShareIntent, title, intentShareList.ToArray());
            }
        }


        //--------------------------------------
        // Private Methods
        //--------------------------------------


        private void AppendText() {
            ShareIntent.SetAction(AN_Intent.ACTION_SEND);
            ShareIntent.PutExtra(AN_Intent.EXTRA_TEXT, m_text);
            ShareIntent.SetType(AN_MIMEDataType.Text);
        }


        private void AppendImages() {

            if (m_images.Count > 1) {
                ShareIntent.SetAction(AN_Intent.ACTION_SEND_MULTIPLE);
                ShareIntent.PutExtra(AN_Intent.EXTRA_STREAM, m_images.ToArray());
            } else {
                ShareIntent.SetAction(AN_Intent.ACTION_SEND);
                ShareIntent.PutExtra(AN_Intent.EXTRA_STREAM, m_images[0]);
            }

            ShareIntent.AddFlags(AN_Intent.FLAG_GRANT_READ_URI_PERMISSION);
            ShareIntent.SetType(AN_MIMEDataType.Image);
        }


        //--------------------------------------
        // Get / Set
        //--------------------------------------

        protected AN_Intent ShareIntent {
            get { return m_shareIntent ?? (m_shareIntent = new AN_Intent()); }
        }

    }
}