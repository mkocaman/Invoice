using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ESF_kz
{
	static class ValidatingManager
	{
		internal static bool enumIsDefined<T>(string str) where T:Enum
		{
			return Enum.IsDefined(typeof(T), str);
		}

		internal static bool ValidateFloatTextBox(TextBox tb, ErrorProvider ep)
		{
			bool result = false;
			Regex regex = new Regex(@"^(\d*)|(\d*[.]\d*)$");
			bool isEmpty = tb.Text == "";
			bool isCorrectFormat = regex.IsMatch(tb.Text);
			if(isCorrectFormat && !isEmpty)
			{
				tb.Text = float.Parse(tb.Text).ToString();
			}

			if (isEmpty)
			{
				ep.SetError(tb, "Empty value");
			}
			else if (!isCorrectFormat)
			{
				ep.SetError(tb, "Wrong format(must be float)");
			}
			else
			{
				ep.Clear();
				result = true;
			}
			return result;
		}

		internal static bool ValidateIntegerTextBox(TextBox tb, ErrorProvider ep)
		{
			bool result = false;
			Regex regex = new Regex(@"^\d*$");
			bool isEmpty = tb.Text == "";
			bool isCorrectFormat = regex.IsMatch(tb.Text);
			if (isCorrectFormat)
			{
				tb.Text = int.Parse(tb.Text).ToString();
			}
			if (isEmpty)
			{
				ep.SetError(tb, "Empty value");
			}
			else if (!isCorrectFormat)
			{
				ep.SetError(tb, "Wrong format(must be integer)");
			}
			else
			{
				ep.Clear();
				result = true;
			}
			return result;
		}
	}
}
