using System.Text.RegularExpressions;
using Sprite;

namespace System
{
    /// <summary>
    /// 针对String字符串类型的扩展发
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 指定的字符串是否为null或<see cref="string.Empty" />
        /// </summary>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 指定的字符串是否是null、空还是仅由空格字符组成。
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// 按给定的分隔符将当前字符串拆分成为数组
        /// </summary>
        /// <param name="str"></param>
        /// <param name="separator">分隔符</param>
        public static string[] Split(this string str, string separator)
        {
            return str.Split(new[] {separator}, StringSplitOptions.RemoveEmptyEntries);
        }


        /// <summary>
        /// 截取左字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string Left(this string str, int lenght)
        {
            Check.NotNull(str, nameof(str));

            if (str.Length < lenght)
            {
                throw new ArgumentException("lenght argument cannot be bigger than given string length!");
            }

            return str.AsSpan().Slice(0, lenght).ToString();
        }

        public static string Right(this string str, int lenght)
        {
            Check.NotNull(str, nameof(str));

            if (str.Length < lenght)
            {
                throw new ArgumentException("lenght argument can not be bigger than given string's length!");
            }
            
            return str.AsSpan().Slice(str.Length - lenght, lenght).ToString();
        }


        public static string RemovePrefix(this string str, params string[] prefixes)
        {
            return str.RemovePrefix(StringComparison.Ordinal, prefixes);
        }

        public static string RemovePrefix(this string str, StringComparison comparisonType, params string[] prefixes)
        {
            if (str.IsNullOrEmpty())
            {
                return str;
            }

            if (prefixes is null || prefixes.Length <= 0)
            {
                return str;
            }

            foreach (var prefix in prefixes)
            {
                if (str.StartsWith(prefix, comparisonType))
                {
                    return str.Right(str.Length - prefix.Length);
                }
            }

            return str;
        }


        public static string RemoveSuffix(this string str, params string[] suffixes)
        {
            return str.RemoveSuffix(StringComparison.Ordinal, suffixes);
        }

        /// <summary>
        /// 移除后缀
        /// </summary>
        /// <param name="str"></param>
        /// <param name="comparisonType"></param>
        /// <param name="suffixes"></param>
        /// <returns></returns>
        public static string RemoveSuffix(this string str, StringComparison comparisonType, params string[] suffixes)
        {
            if (str.IsNullOrEmpty())
            {
                return null;
            }

            if (suffixes is null || suffixes.Length <= 0)
            {
                return str;
            }

            foreach (var suffix in suffixes)
            {
                if (str.EndsWith(suffix, comparisonType))
                {
                    return str.Left(str.Length - suffix.Length);
                }
            }

            return str;
        }

        public static string ToCamelCase(this string str, bool useCurrentCulture = false)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            if (str.Length == 1)
            {
                return useCurrentCulture ? str.ToLower() : str.ToLowerInvariant();
            }

            return (useCurrentCulture ? char.ToLower(str[0]) : char.ToLowerInvariant(str[0])) + str.AsSpan()[1..].ToString();
        }


        public static string GetCamelCaseFirstWord(this string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (str.Length == 1)
            {
                return str;
            }

            var res = Regex.Split(str, @"(?=\p{Lu}\p{Ll})|(?<=\p{Ll})(?=\p{Lu})");

            if (res.Length < 1)
            {
                return str;
            }

            return res[0];
        }

        public static string GetPascalCaseFirstWord(this string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (str.Length == 1)
            {
                return str;
            }

            var res = Regex.Split(str, @"(?=\p{Lu}\p{Ll})|(?<=\p{Ll})(?=\p{Lu})");

            if (res.Length < 2)
            {
                return str;
            }

            return res[1];
        }

        public static string GetPascalOrCamelCaseFirstWord(this string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (str.Length == 1)
            {
                return str;
            }

            var span = str.AsSpan();
            if (span[0]>=65&&str[0]<=90)
            {
                return GetPascalCaseFirstWord(str);
            }

            return GetCamelCaseFirstWord(str);
        }
    }
}