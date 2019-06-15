using UnityEngine;

namespace RPG.AntiVariable
{
    public class AntiVariable
    {
        object m_obj;
        sbyte m_rand;

        public AntiVariable()
        {
            m_rand = 0;
            m_obj = 0;
        }

        public void makeRandom()
        {
            int bRand = m_rand;
            while (bRand == m_rand)
                m_rand = (sbyte)AntiManager.random(1, 9);
        }

        #region set
        public void setValue(byte v)
        {
            makeRandom();
            if (byte.MaxValue <= v + m_rand)
                m_rand -= sbyte.MaxValue;
            m_obj = v + m_rand;
        }

        public void setValue(sbyte v)
        {
            makeRandom();
            if (sbyte.MaxValue <= v + m_rand)
                m_rand -= sbyte.MaxValue;
            m_obj = v + m_rand;
        }

        public void setValue(short v)
        {
            makeRandom();
            if (short.MaxValue <= v + m_rand)
                m_rand -= sbyte.MaxValue;
            m_obj = v + m_rand;
        }

        public void setValue(int v)
        {
            makeRandom();
            if (int.MaxValue <= v + m_rand)
                m_rand -= sbyte.MaxValue;
            m_obj = v + m_rand;
        }

        public void setValue(float v)
        {
            makeRandom();
            if (float.MaxValue <= v + m_rand)
                m_rand -= sbyte.MaxValue;
            m_obj = v + m_rand;
        }

        public void setValue(long v)
        {
            makeRandom();
            if (long.MaxValue <= v + m_rand)
                m_rand -= sbyte.MaxValue;
            m_obj = v + m_rand;
        }

        public void setValue(double v)
        {
            makeRandom();
            if (double.MaxValue <= v + m_rand)
                m_rand -= sbyte.MaxValue;
            m_obj = v + m_rand;
        }
        #endregion

        #region get
        public byte byteValue()
        {
            return (byte)intValue();
        }

        public sbyte sbyteValue()
        {
            return (sbyte)intValue();
        }

        public short shortValue()
        {
            return (short)intValue();
        }

        public int intValue()
        {
            return (int)m_obj - m_rand;
        }
        
        public float floatValue()
        {
            return (float)m_obj - m_rand;
        }
        
        public long longValue()
        {
            if (m_obj == null)
                return 0;
            return (long)m_obj - m_rand;
        }

        public double doubleValue()
        {
            return (double)m_obj - m_rand;
        }

        #endregion


    }
}