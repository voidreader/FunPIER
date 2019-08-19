using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SA.Android.Contacts;

public class AN_ContactsEample : MonoBehaviour
{


    [SerializeField] Button m_retirve = null;
    [SerializeField] AN_ContactsEampleView m_contactView = null;


    public void Awake() {

        m_retirve.onClick.AddListener(() => {
            AN_ContactsContract.Retrieve((result) => {
                if(result.IsFailed) {
                    Debug.Log("Filed:  " + result.Error.Message);
                    return;
                }

                Debug.Log("Loaded: " + result.Contacts.Count + " Contacts.");
                foreach(var contact in result.Contacts) {
                    var view = Instantiate(m_contactView.gameObject).GetComponent<AN_ContactsEampleView>();
                    view.transform.SetParent(m_contactView.transform.parent);
                    view.gameObject.SetActive(true);
                    view.SetContactInfo(contact);
                }
            });
        });
    }
}
