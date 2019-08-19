
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SA.Android.Utilities;
using SA.Android.Vending.Licensing;
using SA.Foundation.Tests;
using SA.Foundation.Async;

using SA.Android.App;
using SA.Android.Content;
using SA.Android.Content.Pm;


namespace SA.Android.Tests.Application
{
    public class AN_AlertDialog_Test : SA_BaseTest
    {

        private class AN_AlertDialogConfiguration {
            public AN_DialogTheme DialogTheme;
            public int ButtonsCount;
        }


        public override void Test() {

            List<AN_AlertDialogConfiguration> configs = new List<AN_AlertDialogConfiguration>();
            foreach (AN_DialogTheme theme in (AN_DialogTheme[])Enum.GetValues(typeof(AN_DialogTheme))) {
               

               
                for(int i = 1; i <= 3; i++) {
                    var config = new AN_AlertDialogConfiguration();
                    config.DialogTheme = theme;

                    config.ButtonsCount = i;
                    configs.Add(config);
                }
            }

            SA_Coroutine.Start(TestDialogVariants(configs));
        }


        private IEnumerator TestDialogVariants(List<AN_AlertDialogConfiguration> configs) {

            foreach(var config in configs) {
                var dialog = new AN_AlertDialog(config.DialogTheme);
                dialog.Title = config.DialogTheme.ToString();
                dialog.Message = "Alert Dialog Test with " + config.ButtonsCount + " buttons";

                if(config.ButtonsCount >= 1) {
                    dialog.SetPositiveButton("Positive", () => {
                       
                    });
                }

                if (config.ButtonsCount >= 2) {
                    dialog.SetNegativeButton("Negative", () => {

                    });
                }

                if (config.ButtonsCount >= 3) {
                    dialog.SetNeutralButton("Neutral", () => {

                    });
                }

                dialog.Show();
                yield return new WaitForSeconds(1f);
                dialog.Hide();
            }

            SetResult(SA_TestResult.OK);
            yield return null;
        }

    }
}