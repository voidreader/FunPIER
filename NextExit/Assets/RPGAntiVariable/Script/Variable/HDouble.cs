using UnityEngine;
using System;
using System.Collections;

namespace RPG.AntiVariable
{
    public class HDouble
    {
        AntiVariable H;

        public HDouble(double a)
        {
            H = new AntiVariable();
            H.setValue(a);
        }

        public override string ToString()
        {
            return H.doubleValue().ToString();
        }
        public string ToString(System.IFormatProvider provider)
        {
            return H.doubleValue().ToString(provider);
        }
        public string ToString(string format)
        {
            return H.doubleValue().ToString(format);
        }
        public string ToString(string format, System.IFormatProvider provider)
        {
            return H.doubleValue().ToString(format, provider);
        }
        
        public double CompareTo(HDouble a)
        {
            return H.doubleValue().CompareTo(a.H.doubleValue());
        }        
        
        #region implicit

        public static implicit operator double(HDouble a)
        {
            return a.H.doubleValue();
        }

        public static implicit operator HDouble(double a)
        {
            return new HDouble(a);
        }
        #endregion

    }
}