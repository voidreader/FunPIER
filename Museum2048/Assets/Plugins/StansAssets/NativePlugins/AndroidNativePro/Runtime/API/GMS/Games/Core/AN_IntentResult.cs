using SA.Android.Content;
using SA.Android.GMS.Common;

namespace SA.Android.GMS.Games
{
    /// <summary>
    /// GMS Intent request result
    /// </summary>
    public class AN_IntentResult : AN_LinkedObjectResult<AN_Intent>
    {
        public AN_IntentResult(AN_Intent intent)
            : base()
        {
            m_linkedObject = intent;
        }

        /// <summary>
        /// GMS Generated Intet
        /// </summary>
        public AN_Intent Intent => m_linkedObject;
    }
}
