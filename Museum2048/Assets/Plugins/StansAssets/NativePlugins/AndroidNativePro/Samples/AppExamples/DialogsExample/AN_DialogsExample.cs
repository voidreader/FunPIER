using System;
using UnityEngine;
using UnityEngine.UI;
using SA.Android.App;
using SA.Android.Content;
using StansAssets.Foundation.Async;

public class AN_DialogsExample : MonoBehaviour
{
    [SerializeField]
    Button m_progressButton = null;
    [SerializeField]
    Button m_messageButton = null;
    [SerializeField]
    Button m_dialogButton = null;
    [SerializeField]
    Button m_rateButton = null;
    [SerializeField]
    Button m_calendarButton = null;

    void Start()
    {
        m_progressButton.onClick.AddListener(() =>
        {
            //Show the preloader
            AN_Preloader.LockScreen("Please Wait...");
            
            //and hide it in 2 sec
             CoroutineUtility.WaitForSeconds(2f, () =>
            {
                AN_Preloader.UnlockScreen();
            });
        });

        m_messageButton.onClick.AddListener(() =>
        {
            var message = new AN_AlertDialog(AN_DialogTheme.Material);
            message.Title = "Message";
            message.Message = "Some message text";

            message.SetPositiveButton("Okay", () =>
            {
                Debug.Log("message: ok button was clicked");
            });

            message.Show();
        });

        m_dialogButton.onClick.AddListener(() =>
        {
            var dialog = new AN_AlertDialog(AN_DialogTheme.Light);
            dialog.Title = "Dialog";
            dialog.Message = "Some dialog text";

            dialog.SetPositiveButton("Yes", () =>
            {
                Debug.Log("dialog: Yes button was clicked");
            });

            dialog.SetNegativeButton("No", () =>
            {
                Debug.Log("dialog: No button was clicked");
            });

            dialog.Show();
        });

        m_rateButton.onClick.AddListener(() =>
        {
            var appName = Application.productName;
            var appIdentifier = Application.identifier;

            var dialog = new AN_AlertDialog(AN_DialogTheme.Default);
            dialog.Title = string.Format("Rate {0}!", appName);
            dialog.Message = string.Format("If you enjoy using {0}, please take a moment to rate it.Thanks for your support!", appName);

            dialog.SetPositiveButton("Rate", () =>
            {
                Debug.Log("dialog: Rate button was clicked");

                //This code will take user to your app Play Market page
                var uri = new Uri("market://details?id=" + appIdentifier);
                var viewIntent = new AN_Intent(AN_Intent.ACTION_VIEW, uri);
                AN_MainActivity.Instance.StartActivity(viewIntent);
            });

            dialog.SetNegativeButton("No, thanks", () =>
            {
                Debug.Log("dialog: No, thanks button was clicked");
            });

            dialog.SetNeutralButton("Remind me later", () =>
            {
                Debug.Log("dialog: Remind me later button was clicked");
            });

            dialog.Show();
        });

        m_calendarButton.onClick.AddListener(() =>
        {
            var date = DateTime.Now;
            var year = date.Year;
            var month = date.Month - 1; //Compatibility with Android Calendar..
            var day = date.Day;

            var picker = new AN_DatePickerDialog(year, month, day);
            picker.Show((result) =>
            {
                if (result.IsSucceeded)
                {
                    Debug.Log("date picked result.Year: " + result.Year);

                    //Same  Android Calendar Compatibility
                    Debug.Log("date picked result.Month: " + result.Month + 1);
                    Debug.Log("date picked result.Day: " + result.Day);
                }
                else
                {
                    Debug.Log("Failed to pick a date: " + result.Error.FullMessage);
                }
            });
        });
    }

    void OnApplicationPause(bool pause)
    {
        Debug.Log("OnApplicationPause: " + pause);
    }
}
