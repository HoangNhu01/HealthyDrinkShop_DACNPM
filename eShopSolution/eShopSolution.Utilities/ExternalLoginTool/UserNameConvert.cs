using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace eShopSolution.Utilities.ExternalLoginTool
{
    public class UserNameConvert
    {
        public static string ConvertUnicode(string input)
        {
            string normalized = input.Normalize(NormalizationForm.FormD); // Normalize the string
            var stringBuilder = new StringBuilder();

            foreach (char c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    // Remove any diacritical marks (non-spacing marks)
                    stringBuilder.Append(c);
                }
            }

            string converted = stringBuilder.ToString();
            string result = Regex.Replace(converted, @"\s+", "");
            return result;
        }
    }
}
