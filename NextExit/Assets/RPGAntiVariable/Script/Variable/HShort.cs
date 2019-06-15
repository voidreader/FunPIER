
namespace RPG.AntiVariable
{
    public struct HShort
    {
        AntiVariable H;

        public HShort(short a)
        {
            H = new AntiVariable();
            H.setValue(a);
        }

        public override string ToString()
        {
            return H.shortValue().ToString();
        }
        public string ToString(System.IFormatProvider provider)
        {
            return H.shortValue().ToString(provider);
        }
        public string ToString(string format)
        {
            return H.shortValue().ToString(format);
        }
        public string ToString(string format, System.IFormatProvider provider)
        {
            return H.shortValue().ToString(format, provider);
        }
        
        public int CompareTo(HShort a)
        {
            return H.shortValue().CompareTo(a.H.shortValue());
        }
        
        #region implicit

        public static implicit operator short(HShort a)
        {
            return a.H.shortValue();
        }

        public static implicit operator HShort(short a)
        {
            return new HShort(a);
        }
        #endregion

    }
}