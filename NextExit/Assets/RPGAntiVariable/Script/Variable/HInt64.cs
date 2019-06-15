using System.Runtime.InteropServices;

namespace RPG.AntiVariable
{
    [ComVisible(true)]
    public struct HInt64
    {
        AntiVariable H;

        public HInt64(long a)
        {
            H = new AntiVariable();
            H.setValue(a);
        }

        public override string ToString()
        {
            return H.longValue().ToString();
        }
        public string ToString(System.IFormatProvider provider)
        {
            return H.longValue().ToString(provider);
        }
        public string ToString(string format)
        {
            return H.longValue().ToString(format);
        }
        public string ToString(string format, System.IFormatProvider provider)
        {
            return H.longValue().ToString(format, provider);
        }
        public long CompareTo(HInt64 a)
        {
            return H.longValue().CompareTo(a.H.longValue());
        }
        
        #region implicit
        public static implicit operator long(HInt64 a)
        {
            return a.H.longValue();
        }

        public static implicit operator HInt64(long a)
        {
            return new HInt64(a);
        }
        #endregion

        #region operator overload        
        public static HInt64 operator ++(HInt64 a)
        {
            return new HInt64(a.H.longValue() + 1);
        }

        public static HInt64 operator --(HInt64 a)
        {
            return new HInt64(a.H.longValue() - 1);
        }
        
        #endregion

    }
}