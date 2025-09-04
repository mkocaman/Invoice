using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ESF_kz.Forms
{
	public partial class panelESFpartF : AbstractUCESFpanel
	{
		public panelESFpartF()
		{
			InitializeComponent();
		}

		private void tbPartF_deliveryDocNum_TextChanged(object sender, EventArgs e)
		{
			tbPartF_deliveryDocNum_Validation();
		}

		private void tbPartF_deliveryDocNum_Validation()
		{
			Regex regex = new Regex(@".{0,18}");
			bool flag = regex.IsMatch(tbPartF_deliveryDocNum.Text);
			if (!flag)
			{
				epPartF_deliveryDocNum.SetError(tbPartF_deliveryDocNum, "wrong format");
			}
			else
			{
				epPartF_deliveryDocNum.Clear();
			}
		}

		internal DateTime getInvoiceDeliveryDocDate()
		{
			return tbPartF_deliveryDocDate.Text == ""? new DateTime() : dtpPartF_deliveryDocDate.Value;
		}

		internal bool setInvoiceDeliveryDocDate(DateTime date)
		{
			try
			{
				dtpPartF_deliveryDocDate.Value = date;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getInvoiceDeliveryDocNum()
		{
			return tbPartF_deliveryDocNum.Text;
		}

		internal bool setInvoiceDeliveryDocNum(string num)
		{
			try
			{
				tbPartF_deliveryDocNum.Text = num;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private void dtpPartF_deliveryDocDate_ValueChanged(object sender, EventArgs e)
		{
			tbPartF_deliveryDocDate.Text = dtpPartF_deliveryDocDate.Value.ToString("dd.MM.yyyy");
		}
	}
}
