using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AstraBlog.Helpers
{
    public static class StringHelper
    {

        // The default route of MVC! By Anna -- (title of blog)
        public static string BlogSlug(string title)
        {
           //remove all accents and make string lower case.
            string output = RemoveAccents(title).ToLower();

            //remove special characters
            output = Regex.Replace(output, @"[^A-Za-z0-9\s-]", "");

            //remove all additional spaces in favor of just one
            output = Regex.Replace(output, @"\s+", " ").Trim();

            //replace all spaces with hyphen

            output = Regex.Replace(output, @"\s", "-");

            return output;
        }

        private static string RemoveAccents(string title)
        {
            if(string.IsNullOrWhiteSpace(title))
            {
                return title;
            }

            //convert for Unicode
            title = title.Normalize(NormalizationForm.FormD);

            // format the unicode / ascii

            char[] chars = title.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();

            // convert and return the new title
            return new string(chars).Normalize(NormalizationForm.FormC);
        }
    }
}
