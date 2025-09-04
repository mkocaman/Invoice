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
	public partial class panelESFpartE : AbstractUCESFpanel
	{
		public panelESFpartE()
		{
			InitializeComponent();
		}

		private void rbPartE_hasContractTrue_CheckedChanged(object sender, EventArgs e)
		{
			rbPartE_hasContractFalse.Checked = !rbPartE_hasContractTrue.Checked;
			tbPartE_contractNum.Enabled = rbPartE_hasContractTrue.Checked;
			dtpPartE_contractDate.Enabled = rbPartE_hasContractTrue.Checked;
		}

		private void rbPartE_hasContractFalse_CheckedChanged(object sender, EventArgs e)
		{
			rbPartE_hasContractTrue.Checked = !rbPartE_hasContractFalse.Checked;
		}

		private void tbPartE_contractNum_TextChanged(object sender, EventArgs e)
		{
			tbPartE_contractNum_Validation();
		}

		private void tbPartE_contractNum_Validation()
		{
			Regex regex = new Regex(@"^.{0,18}$");
			bool flag = regex.IsMatch(tbPartE_contractNum.Text);
			if (!flag)
			{
				epPartE_contractNum.SetError(tbPartE_contractNum, "Wrong format");
			}
			else
			{
				epPartE_contractNum.Clear();
			}
		}

		private void tbPartE_term_TextChanged(object sender, EventArgs e)
		{
			tbPartE_term_Validation();
		}

		private void tbPartE_term_Validation()
		{
			Regex regex = new Regex(@"^.{0,98}$");
			bool flag = regex.IsMatch(tbPartE_term.Text);
			if (!flag)
			{
				epPartE_term.SetError(tbPartE_term, "Wrong format");
			}
			else
			{
				epPartE_term.Clear();
			}
		}

		private void tbPartE_transportTypeCode_TextChanged(object sender, EventArgs e)
		{

		}

		private void tbPartE_warrant_TextChanged(object sender, EventArgs e)
		{
			tbPartE_warrant_Validation();
		}

		private void tbPartE_warrant_Validation()
		{
			Regex regex = new Regex(@".{0,18}");
		}

		private void tbPartE_deliveryConditionCode_TextChanged(object sender, EventArgs e)
		{
			tbPartE_deliveryConditionCode_Validation();
		}

		private void tbPartE_deliveryConditionCode_Validation()
		{
			Regex regex = new Regex(@"^.{0,3}$");
			bool flag = regex.IsMatch(tbPartE_deliveryConditionCode.Text);
			if (!flag)
			{
				epPartE_deliveryConditionCode.SetError(tbPartE_deliveryConditionCode, "Wrong format");
			}
			else
			{
				epPartE_deliveryConditionCode.Clear();
			}
		}

		private void tbPartE_destination_TextChanged(object sender, EventArgs e)
		{
			tbPartE_destination_Validation();
		}

		private void tbPartE_destination_Validation()
		{
			Regex regex = new Regex(@"^.{0,98}$");
			bool flag = regex.IsMatch(tbPartE_destination.Text);
			if (!flag)
			{
				epPartE_destination.SetError(tbPartE_destination, "Wrong format");
			}
			else
			{
				epPartE_destination.Clear();
			}
		}

		internal DateTime getDeliveryTermContractDate()
		{
			return dtpPartE_contractDate.Enabled ? dtpPartE_contractDate.Value : new DateTime();
		}

		internal bool setDeliveryTermContractDate(DateTime date)
		{
			try
			{
				dtpPartE_contractDate.Value = date;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getDeliveryTermContractNum()
		{
			return tbPartE_contractNum.Text;
		}
		internal bool setDeliveryTermContractNum(string num)
		{
			try
			{
				tbPartE_contractNum.Text = num;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getDeliveryTermConditiomCode()
		{
			return tbPartE_deliveryConditionCode.Text;
		}

		internal bool setDeliveryTermConditiomCode(string code)
		{
			try
			{
				tbPartE_deliveryConditionCode.Text= code;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getDeliveryTermDestination()
		{
			return tbPartE_destination.Text;
		}

		internal bool setDeliveryTermDestination(string destination)
		{
			try
			{
				tbPartE_destination.Text = destination;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal bool getDeliveryTermHasContract()
		{
			return rbPartE_hasContractTrue.Checked;
		}

		internal bool setDeliveryTermHasContract(bool hasContract)
		{
			try
			{
				rbPartE_hasContractTrue.Checked = hasContract;
				return true;
			}
			catch (Exception)
			{
				return false;		
			}
		}

		internal string getDeliveryTermTerm()
		{
			return tbPartE_term.Text;
		}

		internal bool setDeliveryTermTerm(string term)
		{
			try
			{
				tbPartE_term.Text = term;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getDeliveryTermTransportTypeCode()
		{
			return tbPartE_transportTypeCode.Text;
		}

		internal bool setDeliveryTermTransportTypeCode(string code)
		{
			try
			{
				tbPartE_transportTypeCode.Text = code;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getDeliveryTermWarrant()
		{
			return tbPartE_warrant.Text;
		}

		internal bool setDeliveryTermWarrant(string warrant)
		{
			try
			{
				tbPartE_warrant.Text = warrant;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal DateTime getDeliveryTermWarrantDate()
		{
			return tbPartE_warrantDate.Text == ""? new DateTime() : dtpPartE_warrantDate.Value;
		}

		internal bool setDeliveryTermWarrantDate(DateTime date)
		{
			try
			{
				dtpPartE_warrantDate.Value = date;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private void dtpPartE_contractDate_ValueChanged(object sender, EventArgs e)
		{
			tbPartE_contractDate.Text = dtpPartE_contractDate.Value.ToString("dd.MM.yyyy");
		}

		private void dtpPartE_warrantDate_ValueChanged(object sender, EventArgs e)
		{
			tbPartE_warrantDate.Text = dtpPartE_warrantDate.Value.ToString("dd.MM.yyyy");
		}
	}
}
