using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SA.Android.Contacts;

public class AN_ContactsEampleView : MonoBehaviour {

    [SerializeField] RawImage m_image = null;
    [SerializeField] Text m_name = null;
    [SerializeField] Text m_info = null;



    public void SetContactInfo(AN_ContactInfo contact) {
        m_image.texture = contact.Photo;
        m_name.text = contact.Name;
        m_info.text = "Email: " + contact.Email + " Phone: " + contact.Phone;
    }
}
