using System;
using System.Text.RegularExpressions;

namespace MedLaunch.Common
{
    public static class RegexOps
    {
        public static string DumpHRefs(string inputString)
        {
            Match m;
            string HRefPattern = "href\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))";
            m = Regex.Match(inputString, HRefPattern,
                                RegexOptions.IgnoreCase | RegexOptions.Compiled,
                                TimeSpan.FromSeconds(1));
            string result = "";
            while (m.Success)
            {
                result = m.Groups[1].ToString();
            }

            return result;


        }
    }
}
