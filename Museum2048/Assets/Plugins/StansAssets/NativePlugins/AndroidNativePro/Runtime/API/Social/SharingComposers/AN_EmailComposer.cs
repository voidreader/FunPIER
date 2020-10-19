using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.Android.App;
using SA.Android.Content;
using SA.Android.Utilities;

namespace SA.Android.Social
{
    public class AN_EmailComposer : AN_SocialFullShareBuilder
    {
        string m_subject = string.Empty;
        readonly List<string> m_recipients = new List<string>();

        protected override AN_Intent MakeSharingIntent()
        {
            var chooser = GenerateChooserIntent(string.Empty, "mail");
            return chooser;
        }

        public void SetSubject(string subject)
        {
            m_subject = subject;
        }

        public void AddRecipient(string recipientEmail)
        {
            m_recipients.Add(recipientEmail);
        }

        protected override void GenerateShareIntent()
        {
            base.GenerateShareIntent();

            ShareIntent.PutExtra(AN_Intent.EXTRA_EMAIL, m_recipients.ToArray());
            ShareIntent.PutExtra(AN_Intent.EXTRA_SUBJECT, m_subject);
        }
    }
}
