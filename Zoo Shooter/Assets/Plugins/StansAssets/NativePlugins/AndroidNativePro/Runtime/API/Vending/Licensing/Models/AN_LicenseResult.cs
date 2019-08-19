using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Templates;

namespace SA.Android.Vending.Licensing
{

    [Serializable]
    public class AN_LicenseResult : SA_Result
    {
        [SerializeField] int m_policyCode;

        //Editor use only
        public AN_LicenseResult(int policyCode) : base(){
            m_policyCode = policyCode;
        }


        public AN_LicenseErrorCode ErrorCode  {
            get {
                if(HasError) {
                    return (AN_LicenseErrorCode)Error.Code;
                } else {
                    return AN_LicenseErrorCode.ERROR_UNDEFINED;
                }
            }
        }


        /// <summary>
        /// Current app lisence policy code.
        /// You may see the posible codes inside <see cref="AN_Policy"/>
        /// </summary>
        public int PolicyCode {
            get {
                return m_policyCode;
            }
        }
    }
}