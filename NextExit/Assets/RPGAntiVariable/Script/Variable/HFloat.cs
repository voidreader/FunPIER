using UnityEngine;
using System;
using System.Collections;

namespace RPG.AntiVariable
{
    public class HFloat
    {
        AntiVariable H;

        public HFloat(float a)
        {
            H = new AntiVariable();
            H.setValue(a);
        }

        public override string ToString()
        {
            return H.floatValue().ToString();
        }
        public string ToString(System.IFormatProvider provider)
        {
            return H.floatValue().ToString(provider);
        }
        public string ToString(string format)
        {
            return H.floatValue().ToString(format);
        }
        public string ToString(string format, System.IFormatProvider provider)
        {
            return H.floatValue().ToString(format, provider);
        }
        
        public float CompareTo(HFloat a)
        {
            return H.floatValue().CompareTo(a.H.floatValue());
        }        
        
        #region implicit

        public static implicit operator float(HFloat a)
        {
            return a.H.floatValue();
        }

        public static implicit operator HFloat(float a)
        {
            return new HFloat(a);
        }
        #endregion

    }
}