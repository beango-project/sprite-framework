using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace Sprite.AspNetCore.Mvc.AntiForgery
{
    /// <summary>
    /// 防伪选项
    /// </summary>
    public class SpriteAntiforgeryOptions
    {
        private Predicate<Type> _autoValidateFilter;

        private HashSet<string> _autoValidateIgnoredHttpMethods;

        private CookieBuilder _cookieBuilder;

        private string _formFieldName = "__RequestVerificationToken";

        public SpriteAntiforgeryOptions()
        {
            _autoValidateFilter = Type => true;
            _autoValidateIgnoredHttpMethods = new HashSet<string>
            {
                "GET", "HEAD", "TRACE", "OPTIONS"
            };
            _cookieBuilder = new CookieBuilder()
            {
                Name = CookieName,
                HttpOnly = false,
                IsEssential = true,
                SameSite = SameSiteMode.Strict,
                SecurePolicy = CookieSecurePolicy.None,
            };
        }

        //RequestVerificationToken
        /// <summary>
        /// 头名称
        /// </summary>
        public string? HeaderName { get; set; } = "X-CSRF-TOKEN";

        /// <summary>
        /// Cookie名称
        /// </summary>
        public string? CookieName { get; set; } = "CSRF-TOKEN";

        /// <summary>
        /// Cookie身份验证名称，Default:Identity.Application.
        /// </summary>
        public string? CookieAuthenticationName { get; set; } = "Identity.Application";

        /// <summary>
        /// Cookie
        /// </summary>
        public CookieBuilder Cookie
        {
            get => _cookieBuilder;
            set => _cookieBuilder = Check.NotNull(value, nameof(value));
        }

        /// <summary>
        /// 自动验证忽略的Http方法
        /// Default methods: "GET", "HEAD", "TRACE", "OPTIONS".
        /// </summary>
        [NotNull]
        public HashSet<string> AutoValidateIgnoredHttpMethods
        {
            get => _autoValidateIgnoredHttpMethods;
            set => _autoValidateIgnoredHttpMethods = Check.NotNull(value, nameof(value));
        }

        public string FormFieldName
        {
            get => _formFieldName;
            set => _formFieldName = Check.NotNull(value, nameof(value));
        }

        /// <summary>
        /// Default value: true.
        /// </summary>
        public bool AutoValidate { get; set; } = true;

        /// <summary>
        /// A predicate to filter types to auto-validate.
        /// Return true to select the type to validate.
        /// Default: returns true for all given types.
        /// </summary>
        [NotNull]
        public Predicate<Type> AutoValidateFilter
        {
            get => _autoValidateFilter;
            set => _autoValidateFilter = Check.NotNull(value, nameof(value));
        }
    }
}