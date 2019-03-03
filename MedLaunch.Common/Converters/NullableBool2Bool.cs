namespace MedLaunch.Common.Converters
{
    public class NullableBool2Bool
    {
        public static bool Convert(bool? value)
        {
            bool b = false;
            if (value == true) { b = true; }
            return b;
        }
    }
}
