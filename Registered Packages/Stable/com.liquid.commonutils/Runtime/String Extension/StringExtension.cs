using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace Liquid.CommonUtils
{
    /// <summary>
    /// Extension of the System.String.
    /// </summary>
    public static class StringExtension
    {
        public static string ToMD5(this string str)
        {
            if (str == null)
                return null;

            MD5 md5 = MD5.Create();
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            StringBuilder md5StrBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; ++i)
            {
                md5StrBuilder.Append(data[i].ToString("x2"));
            }
            return md5StrBuilder.ToString();
        }

        /// <summary>
        /// For the string file path, replace the char '\' in all strings with the char '/'.
        /// </summary>
        /// <param name="pathStr">Original string.</param>
        /// <returns>Modified string.</returns>
        public static string ToUnityPath(this string pathStr)
		{
			if (pathStr == null)
				return null;

			return pathStr.Replace('\\', '/');
        }

		public static string ToFileName(this string filePathStr)
		{
			if (filePathStr == null)
				return null;

			Regex rgx = new Regex(@"([^\\/]*)$");
			Match matches = rgx.Match(filePathStr);

			string fileName = string.Empty;
			if (matches.Success)
				fileName = matches.Value;
			return fileName;
		}

		public static string ToFileNameWithoutSuffix(this string filePathStr)
		{
			if (filePathStr == null)
				return null;

			string fileName = filePathStr.ToFileName();
			if (fileName == null)
				return null;

			Regex rgx = new Regex(@"\.[^\\/\.]+$");
			string result = rgx.Replace(fileName, string.Empty);
			return result;
		}

		/// <summary>
		/// Determine whether the given string is a legal URI.
		/// </summary>
		/// <param name="uri">Original string.</param>
		/// <returns>
		/// Returns true if the given string is a legal URI,
		/// otherwise returns false
		/// </returns>
		public static bool IsLegalURI(this string uri)
		{
			if (uri == null)
				return false;

			return uri.Contains("://");
        }

        /// <summary>
        /// Determine whether the given string is a legal HTTP URI.
        /// </summary>
        /// <param name="uri">Original string.</param>
        /// <returns>
        /// Returns true if the given string is a legal HTTP URI,
        /// otherwise returns false
        /// </returns>
        public static bool IsLegalHTTPURI(this string uri)
		{
			if (uri == null)
				return false;

		    string lowerURI = uri.ToLower();
            return lowerURI.StartsWith("http://") || lowerURI.StartsWith("https://");
        }
    }
}