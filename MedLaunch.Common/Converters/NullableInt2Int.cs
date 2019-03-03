namespace MedLaunch.Common.Converters
{
    public class NullableInt2Int
    {
        public static int Convert(int? value)
        {
            int b = 0;
            if (value == 1) { b = 1; }
            return b;
        }
    }
}
