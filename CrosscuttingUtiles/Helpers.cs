using System;
using System.IO;
using System.Linq;

namespace CrosscuttingUtiles
{
    public class Helpers
    {

        public static string GetUserName()
        {
            //First get user claims    
            var claims = System.Security.Claims.ClaimsPrincipal.Current.Identities.First().Claims.ToList();

            //Filter specific claim    
            var UserName = claims?.FirstOrDefault(x => x.Type.Equals("UserName", StringComparison.OrdinalIgnoreCase))?.Value;

            return UserName != null ? UserName.ToUpper() : "Sin USuario";
        }

        public static string GetUserNameAndRole()
        {
            //First get user claims    
            var claims = System.Security.Claims.ClaimsPrincipal.Current.Identities.First().Claims.ToList();

            //Filter specific claim    
            var UserName = claims?.FirstOrDefault(x => x.Type.Equals("UserName", StringComparison.OrdinalIgnoreCase))?.Value;
            var UserCargoName = claims?.FirstOrDefault(x => x.Type.Equals("UserCargoName", StringComparison.OrdinalIgnoreCase))?.Value;

            var fullUserName = $"{(UserName != null ? UserName : "-")}{(string.IsNullOrEmpty(UserCargoName) ? string.Empty : " | ")}{(string.IsNullOrEmpty(UserCargoName) ? string.Empty : UserCargoName)}";
            return fullUserName.ToLower();
        }


        /// <summary>
        /// Replacing all non-ASCII characters, except right angle character in C#
        /// </summary>
        public static Func<string, string> fnc_NormaliceIsNullOrEmpty = (GetTextFromSomewhere) =>
        {

            try
            {
                if (string.IsNullOrEmpty(GetTextFromSomewhere.Trim())) return "Sin Dato";

                //var _StackTrace = Regex.Replace(_GetTextFromSomewhere, @"[^0-9A-Za-z ,]", String.Empty);          // Solo letras y numeros
                var _StackTrace1 = System.Text.RegularExpressions.Regex.Replace(GetTextFromSomewhere, @"[^\u0000-\u007F]",
                            String.Empty);
                //var _StackTrace2 = Regex.Replace(_GetTextFromSomewhere, @"[^\u0061-\u0301]", String.Empty);       // remove all spaces
                //var _StackTrace1 = _GetTextFromSomewhere.Replace(@"\", @"/");
                //var _StackTrace2 = Regex.Replace(_StackTrace1, @"\r\n?|\n", String.Empty);
                //var _StackTrace3 = Regex.Replace(_StackTrace2, @"""", String.Empty);                              // Replace '"'
                // var _StackTrace3 = Regex.Replace(_StackTrace2, @"[^\u0000-\u007F]", String.Empty);
                //var _StackTrace3 = Regex.Replace(_StackTrace2, @"[^\u0061-\u0301]", String.Empty);                // remove all spaces

                return _StackTrace1;
            }
            catch
            {
                return "Sin Dato";
            }
        };

        /// <summary>
        /// Does .NET provide an easy way convert bytes to KB, MB, GB, etc.?
        /// </summary>
        public static Func<decimal, string> fnc_FormatFileSize = (KeySizes) =>
        {
            try
            {
                long bytes = (long)KeySizes;
                //decimal megabyteSize = ( size / 1048576);
                var unit = 1024;
                if (bytes < unit) { return $"{bytes} B"; }

                var exp = (int)(Math.Log(bytes) / Math.Log(unit));
                var megabyteSize = $"{bytes / Math.Pow(unit, exp):F2} {("KMGTPE")[exp - 1]}B";
                return megabyteSize;
            }
            catch
            {
                return "";
            }
        };

        /// <summary>
        /// Get folder size (for example usage)
        /// </summary>
        public static long GetFolderSize(string path, string ext, bool AllDir)
        {
            var option = AllDir ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            return new DirectoryInfo(path).EnumerateFiles("*" + ext, option).Sum(file => file.Length);
        }

    }
}