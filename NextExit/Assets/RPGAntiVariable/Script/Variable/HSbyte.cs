
namespace RPG.AntiVariable
{
    public struct HSbyte
    {
        AntiVariable H;

        public HSbyte(sbyte a)
        {
            H = new AntiVariable();
            H.setValue(a);
        }

        public override string ToString()
        {
            return H.sbyteValue().ToString();
        }
        public string ToString(System.IFormatProvider provider)
        {
            return H.sbyteValue().ToString(provider);
        }
        public string ToString(string format)
        {
            return H.sbyteValue().ToString(format);
        }
        public string ToString(string format, System.IFormatProvider provider)
        {
            return H.sbyteValue().ToString(format, provider);
        }
        
        public int CompareTo(HSbyte a)
        {
            return H.sbyteValue().CompareTo(a.H.sbyteValue());
        }
        
        #region implicit

        public static implicit operator sbyte(HSbyte a)
        {
            return a.H.sbyteValue();
        }

        public static implicit operator HSbyte(sbyte a)
        {
            return new HSbyte(a);
        }
        #endregion

    }
}