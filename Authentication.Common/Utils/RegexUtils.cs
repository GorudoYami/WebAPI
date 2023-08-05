 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Authentication.Common.Utils;
public static partial class RegexUtils {
	[GeneratedRegex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$")]
	private static partial Regex EmailRegex();

	public static bool IsEmail(this string str) {
		return EmailRegex().IsMatch(str);
	}
}
