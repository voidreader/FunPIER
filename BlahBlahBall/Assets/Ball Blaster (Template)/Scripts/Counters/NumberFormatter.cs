using System.Globalization;

namespace BallBlaster
{

    public static class NumberFormatter
    {

        // Format nubmer in such way 23,3k (23345)
        public static string ToKMB(float number)
        {
            if (number > 999999999 || number < -999999999)
            {
                return number.ToString("0,,,.###B", CultureInfo.InvariantCulture);
            }
            else if (number > 999999 || number < -999999)
            {
                return number.ToString("0,,.##M", CultureInfo.InvariantCulture);
            }
            else if (number > 999 || number < -999)
            {
                return number.ToString("0,.#K", CultureInfo.InvariantCulture);
            }
            else
            {
                return ((int)number).ToString();
            }
        }

    }

}