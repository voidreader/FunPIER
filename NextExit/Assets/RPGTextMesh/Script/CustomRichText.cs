using UnityEngine;
using System.Collections;

namespace RPG.Text
{
    public class CustomRichText
    {
        /// <summary>
        /// 변수 텍스트를 찾아서 변수값, 변수에 포함된 텍스트를 리턴합니다.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="findText"></param>
        /// <param name="refIndex"></param>
        /// <param name="outText"></param>
        /// <param name="outValue"></param>
        /// <returns></returns>
        public static bool findTextIndex(string text, string findText, ref int refIndex, out string outText, out string outValue)
        {
            outValue = "";
            outText = "";
            int index = refIndex;
            int length = text.Length;
            // 시작 점을 찾는다.
            // find start point
            if (text[index] == '<')
            {
                index++;
                if (index + findText.Length > length)
                    return false;
                string sub = text.Substring(index, findText.Length);
                if (sub.Equals(findText))
                {
                    index += findText.Length;
                    int endIndex = 0;
                    for (int i = index; i < length; i++)
                    {
                        if (text[i] == '>')
                        {
                            endIndex = i;
                            break;
                        }
                    }
                    if (endIndex == 0)
                        return false;
                    if (text[index] == '=')
                    {
                        index++;
                        outValue = text.Substring(index, endIndex - index);
                    }
                    index = endIndex + 1;
                }
                else
                    return false;

                string endText = "</" + findText + ">";
                int finalIndex = index;
                bool findEndText = false;
                // 끝을 찾는다.
                // find end point
                while (finalIndex < length)
                {
                    if (finalIndex + endText.Length > length)
                        return false;
                    string sub2 = text.Substring(finalIndex, endText.Length);
                    if (sub2.Equals(endText))
                    {
                        findEndText = true;
                        break;
                    }
                    finalIndex++;
                }

                if (findEndText)
                {
                    outText = text.Substring(index, finalIndex - index);
                    refIndex = finalIndex + endText.Length - 1;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// hex -> int
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static int HexToDecimal(char ch)
        {
            switch (ch)
            {
                case '0': return 0x0;
                case '1': return 0x1;
                case '2': return 0x2;
                case '3': return 0x3;
                case '4': return 0x4;
                case '5': return 0x5;
                case '6': return 0x6;
                case '7': return 0x7;
                case '8': return 0x8;
                case '9': return 0x9;
                case 'a':
                case 'A': return 0xA;
                case 'b':
                case 'B': return 0xB;
                case 'c':
                case 'C': return 0xC;
                case 'd':
                case 'D': return 0xD;
                case 'e':
                case 'E': return 0xE;
                case 'f':
                case 'F': return 0xF;
            }
            return 0xF;
        }

        /// <summary>
        /// RGB 컬러 파싱.
        /// Parsing RGB Color
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="gs"></param>
        /// <param name="bs"></param>
        /// <returns></returns>
        public static Color ParseColor(string rs, string gs, string bs)
        {
            int r = (HexToDecimal(rs[0]) << 4) | HexToDecimal(rs[1]);
            int g = (HexToDecimal(gs[0]) << 4) | HexToDecimal(gs[1]);
            int b = (HexToDecimal(bs[0]) << 4) | HexToDecimal(bs[1]);
            float f = 1f / 255f;
            return new Color(f * r, f * g, f * b);
        }

        /// <summary>
        /// RGBA 컬러 파싱.
        /// Parsing RGBA Color
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="gs"></param>
        /// <param name="bs"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static Color ParseColor(string rs, string gs, string bs, string alpha)
        {
            int r = (HexToDecimal(rs[0]) << 4) | HexToDecimal(rs[1]);
            int g = (HexToDecimal(gs[0]) << 4) | HexToDecimal(gs[1]);
            int b = (HexToDecimal(bs[0]) << 4) | HexToDecimal(bs[1]);
            int a = (HexToDecimal(alpha[0]) << 4) | HexToDecimal(alpha[1]);
            float f = 1f / 255f;
            return new Color(f * r, f * g, f * b, f * a);
        }

        public static Color ConvertColor(string text)
        {
            if (!string.IsNullOrEmpty(text) && text.StartsWith("#"))
            {
                string rs, gs, bs, alpha;
                switch (text.Length)
                {
                    case 4: // #RGB
                        rs = text.Substring(1, 1);
                        rs += rs;
                        gs = text.Substring(2, 1);
                        gs += gs;
                        bs = text.Substring(3, 1);
                        bs += bs;
                        return ParseColor(rs, gs, bs);
                    case 5: // #RGBA
                        rs = text.Substring(1, 1);
                        rs += rs;
                        gs = text.Substring(2, 1);
                        gs += gs;
                        bs = text.Substring(3, 1);
                        bs += bs;
                        alpha = text.Substring(4, 1);
                        alpha += alpha;
                        return ParseColor(rs, gs, bs, alpha);
                    case 7: // #RRGGBB
                        rs = text.Substring(1, 2);
                        gs = text.Substring(3, 2);
                        bs = text.Substring(5, 2);
                        return ParseColor(rs, gs, bs);
                    case 9: // #RRGGBBAA
                        rs = text.Substring(1, 2);
                        gs = text.Substring(3, 2);
                        bs = text.Substring(5, 2);
                        alpha = text.Substring(7, 2);
                        return ParseColor(rs, gs, bs, alpha);
                }
            }
            return new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
    }
}