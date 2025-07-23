using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingShared.Helpers
{
    public class StringHelper
    {
        public static string? NormalizeString(string? input)
        {
            if (input == null)
            {
                return null;
            }
            // Convert to lowercase
            string lowerCase = input.ToLower();

            // Remove diacritics
            string normalized = lowerCase.Normalize(NormalizationForm.FormD);
            string withoutDiacritics = new string(normalized
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray());

            // Remove space
            string result = withoutDiacritics.Replace(" ", "");
            return result;
        }
    }
}
