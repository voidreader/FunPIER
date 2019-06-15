
namespace RPG.AntiVariable
{
    public struct HInt32
    {
        AntiVariable H;

        public HInt32(int a)
        {
            H = new AntiVariable();
            H.setValue(a);
        }

        public override string ToString()
        {
            return H.intValue().ToString();
        }
        public string ToString(System.IFormatProvider provider)
        {
            return H.intValue().ToString(provider);
        }
        public string ToString(string format)
        {
            return H.intValue().ToString(format);
        }
        public string ToString(string format, System.IFormatProvider provider)
        {
            return H.intValue().ToString(format, provider);
        }

        public int CompareTo(HInt32 a)
        {
            return H.intValue().CompareTo(a.H.intValue());
        }
        
        #region implicit

        public static implicit operator int(HInt32 a)
        {
            return a.H.intValue();
        }

        public static implicit operator HInt32(int a)
        {
            return new HInt32(a);
        }
        #endregion

        #region operator overload        
        public static HInt32 operator ++(HInt32 a)
        {
            return new HInt32(a.H.intValue() + 1);
        }

        public static HInt32 operator --(HInt32 a)
        {
            return new HInt32(a.H.intValue() - 1);
        }
        
        #endregion

    }
}