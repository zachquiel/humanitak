using System;
using System.Linq;

namespace SmartAdminMvc.Extensions {
    public static partial class Extensions {
        public static string FirstCharToUpper(this string input) {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("ARGH!");
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }
}