
using UnityEngine;
using System.Collections;

using SA.Android.Utilities;
using SA.Android.Contacts;

using SA.Foundation.Tests;

namespace SA.Android.Tests.Contacts
{
    public class AN_ContactsContract_Test : SA_BaseTest
    {

        public override void Test() {

            AN_ContactsContract.Retrieve((result) => {
                if (result.IsSucceeded) {
                    Debug.Log("Loaded: " + result.Contacts.Count + " Contacts.");
                    foreach (var contact in result.Contacts) {

                        AN_Logger.Log("contact.Id: " + contact.Id);
                        AN_Logger.Log("contact.Name: " + contact.Name);
                        AN_Logger.Log("contact.Note: " + contact.Note);
                        AN_Logger.Log("contact.Organization: " + contact.Organization);
                        AN_Logger.Log("contact.Phone: " + contact.Phone);
                        AN_Logger.Log("contact.Photo: " + contact.Photo);
                    }
                }
                SetAPIResult(result);
            }); 
        }
    }
}
