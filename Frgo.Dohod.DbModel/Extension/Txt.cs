using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Frgo.Dohod.DbModel.Extension.Txt
{
    static class Txt
    {
		public static string Between(this string txt, string delim1, string delim2 = "", int nomocur = 1)
		{
			if (string.IsNullOrWhiteSpace(delim2))
				delim2 = delim1;
			string xx = "";
			int j = 0;
			for (int i = 1; i <= nomocur; i++)
			{
				j = txt.IndexOf(delim1, j, StringComparison.OrdinalIgnoreCase);
				if (j < 0 || i == nomocur)
					break;
				j = j + delim1.Length;
			}
			if (j >= 0)
			{
				txt = txt.Substring(j + delim1.Length);
				j = txt.IndexOf(delim2, StringComparison.OrdinalIgnoreCase);
				if (j >= 0)
					xx = txt.Substring(0, j).Trim();
			}
			return xx;
		}
		public static string From(this string txt, string delim, bool orAll = false, int nomocur = 1)
		{
			// если delim = "", то вернет все самово начала
			string xx = "";
			int j = 0;
			for (int i = 1; i <= nomocur; i++)
			{
				j = txt.IndexOf(delim, j, StringComparison.OrdinalIgnoreCase);
				if (j < 0 || i == nomocur)
					break;
				j = j + delim.Length;
			}
			if (j >= 0)
				xx = txt.Substring(j + delim.Length);
			else if (orAll)
				xx = txt;
			return xx.Trim();
		}

		public static string To(this string txt, string delim, bool orAll = false, int nomocur = 1)
		{
			string xx = "";
			int j = 0;
			for (int i = 1; i <= nomocur; i++)
			{
				j = txt.IndexOf(delim, j, StringComparison.OrdinalIgnoreCase);
				if (j < 0 || i == nomocur)
					break;
				j = j + delim.Length;
			}
			if (j >= 0)
				xx = txt.Substring(0, j);
			else if (orAll)
				xx = txt;
			return xx.Trim();
		}

		public static string Substr(this string txt, int n, int m = -1)
		{
			if (txt == null)
				return "";
			int length = txt.Length;
			if (n >= length)
				return "";

			if (m < 0)
				return txt.Substring(n);
			else
				return txt.Substring(n, n + m > length ? length - n : m);
		}
		public static string ToCompareString(this string txt)
		{
			if (string.IsNullOrWhiteSpace(txt))
				return "";
			// Только буквы и цифны
			return  txt.ToCharArray().Where(x => char.IsWhiteSpace(x) || char.IsLetterOrDigit(x)).ToString().ToLower();
		}


	}
}
