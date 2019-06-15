
namespace RPG.AntiVariable
{
    public struct HByte
    {
        AntiVariable H;

        public HByte(byte a)
        {
            H = new AntiVariable();
            H.setValue(a);
        }

        public override string ToString()
        {
            return H.byteValue().ToString();
        }
        public string ToString(System.IFormatProvider provider)
        {
            return H.byteValue().ToString(provider);
        }
        public string ToString(string format)
        {
            return H.byteValue().ToString(format);
        }
        public string ToString(string format, System.IFormatProvider provider)
        {
            return H.byteValue().ToString(format, provider);
        }
        
        public int CompareTo(HByte a)
        {
            return H.byteValue().CompareTo(a.H.byteValue());
        }
        
        #region implicit

        public static implicit operator byte(HByte a)
        {
            return a.H.byteValue();
        }

        public static implicit operator HByte(byte a)
        {
            return new HByte(a);
        }
        #endregion

    }
}