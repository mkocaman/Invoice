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
	public partial class panelESFpartJ : AbstractUCESFpanel
	{
		public panelESFpartJ()
		{
			InitializeComponent();
		}



		private void tbPartJ_customerAgentTin_TextChanged(object sender, EventArgs e)
		{
			tbPartJ_customerAgentTin_Validating();
		}

		private void tbPartJ_customerAgentTin_Validating()
		{
			ESF_form esfform = (ESF_form)this.TopLevelControl;
			panelESFpartC partC = esfform.getPannel<panelESFpartC>();
			panelESFpartCtab panelCtab = partC.getTab();

			Regex regex = new Regex(@"^.{12}$");
			bool flag = regex.IsMatch(tbPartJ_customerAgentTin.Text);
			if(!flag)
			{
				epPartJ_customerAgentTin.SetError(tbPartJ_customerAgentTin, "Neverniy format");
				panelCtab.chbxPartC_isPrincipal_setState(false);
			}
			else
			{
				epPartJ_customerAgentTin.Clear();
				panelCtab.chbxPartC_isPrincipal_setState(true);
			}
		}

		internal string getCustomerAgentAddress()
		{
			return tbPartJ_customerAgentAddress.Text;
		}

		internal bool setCustomerAgentAddress(string address)
		{
			try
			{
				tbPartJ_customerAgentAddress.Text = address;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal DateTime getCustomerAgentDocDate()
		{
			return  tbPartJ_customerAgentDocDate.Text == ""? new DateTime(): dtpPartJ_customerAgentDocDate.Value;
		}

		internal bool setCustomerAgentDocDate(DateTime date)
		{
			try
			{
				dtpPartJ_customerAgentDocDate.Value = date;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getCustomerAgentDocNum()
		{
			return tbPartJ_customerAgentDocNum.Text;
		}

		internal bool setCustomerAgentDocNum(string num)
		{
			try
			{
				tbPartJ_customerAgentDocNum.Text = num;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getCustomerAgentName()
		{
			return tbPartJ_customerAgentName.Text;
		}

		internal bool setCustomerAgentName(string name)
		{
			try
			{
				tbPartJ_customerAgentName.Text = name;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getCustomerAgentTin()
		{
			return tbPartJ_customerAgentTin.Text;
		}

		internal bool setCustomerAgentTin(string tin)
		{
			try
			{
				tbPartJ_customerAgentTin.Text = tin;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private void dtpPartJ_customerAgentDocDate_ValueChanged(object sender, EventArgs e)
		{
			tbPartJ_customerAgentDocDate.Text = dtpPartJ_customerAgentDocDate.Value.ToString("dd.MM.yyyy");
		}
	}
}
