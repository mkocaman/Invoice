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
	public partial class panelESFpartD : AbstractUCESFpanel
	{
		public panelESFpartD()
		{
			InitializeComponent();
		}

		private void tbPartD_consignorTin_TextChanged(object sender, EventArgs e)
		{
			tbPartD_consignorTin_Validation();
		}

		private void tbPartD_consignorTin_Validation()
		{
			Regex regex = new Regex(@"^\d{12}$");
			bool flag = regex.IsMatch(tbPartD_consignorTin.Text);
			if (!flag)
			{
				epPartD_consignorTin.SetError(tbPartD_consignorTin, "Otsutstvuet ili nevernogo formata");
			}
			else
			{
				if(!checkFOrExistINDB(tbPartD_consignorTin.Text))
				{
					epPartD_consignorTin.SetError(tbPartD_consignorTin, "ИИН/БИН грузоотправителя не найден в БД ИС ЭСФ");
					return;
				}
				if(isBloking(tbPartD_consignorTin.Text))
				{
					epPartD_consignorTin.SetError(tbPartD_consignorTin, "Грузоотправитель заблокирован");
					return;
				}
				epPartD_consignorTin.Clear();
			}
		}

		private bool isBloking(string bin)
		{
			return false;
		}

		private bool checkFOrExistINDB(string bin)
		{
			return true;
		}

		private void tbPartD_consignorName_TextChanged(object sender, EventArgs e)
		{
			tbPartD_consignorName_Validation();
		}

		private void tbPartD_consignorName_Validation()
		{
			Regex regex = new Regex(@".{3,98}");
			bool flag = regex.IsMatch(tbPartD_consignorName.Text);
			if(!flag)
			{
				epPartD_consignorName.SetError(tbPartD_consignorName, "Empty or wrong format");
			}
			else
			{
				epPartD_consignorName.Clear();
			}
		}

		internal string getConsigneeAddress()
		{
			return tbPartD_consigneeAddress.Text;
		}

		internal bool setConsigneeAddress(string address)
		{
			try
			{
				tbPartD_consigneeAddress.Text = address;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getConsigneeName()
		{
			return tbPartD_consigneeName.Text;
		}

		internal bool setConsigneeName(string name)
		{
			try
			{
				tbPartD_consigneeName.Text = name;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getConsignorAddress()
		{
			return tbPartD_consignorAddress.Text;
		}

		internal string getConsignorName()
		{
			return tbPartD_consignorName.Text;
		}

		internal bool setConsignorName(string name)
		{
			try
			{
				tbPartD_consignorName.Text = name;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getConsignorTin()
		{
			return tbPartD_consignorTin.Text;
		}

		internal bool setConsignorTin(string tin)
		{
			try
			{
				tbPartD_consignorTin.Text = tin;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal bool setConsignorAddress(string address)
		{
			try
			{
				tbPartD_consignorAddress.Text = address;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getConsigneeTin()
		{
			return tbPartD_consigneeTin.Text;
		}

		internal bool setConsigneeTin(string tin)
		{
			try
			{
				tbPartD_consigneeTin.Text = tin;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getConsigneeCountryCode()
		{
			return tbPartD_consigneeCountryCode.Text;
		}

		internal bool setConsigneeCountryCode(string code)
		{
			try
			{
				tbPartD_consigneeCountryCode.Text = code;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private void tbPartD_consignorAddress_TextChanged(object sender, EventArgs e)
		{
			tbPartD_consignorAddress_Validation(); 
		}

		private void tbPartD_consignorAddress_Validation()
		{
			Regex regex = new Regex(@"^.{0,98}$");
			bool flag = regex.IsMatch(tbPartD_consignorAddress.Text);

			if (!flag)
			{
				epPartD_consignorAddress.SetError(tbPartD_consignorAddress, "Wrong format");
			}
			else
			{
				epPartD_consignorAddress.Clear();
			}
		}

		private void tbPartD_consigneeTin_TextChanged(object sender, EventArgs e)
		{
			tbPartD_consigneeTin_Validation();
		}

		private void tbPartD_consigneeTin_Validation()
		{
			Regex regex = new Regex(@"^\d{1,50}$");
			bool flag = regex.IsMatch(tbPartD_consigneeTin.Text);
			if (!flag)
			{
				epPartD_consigneeTin.SetError(tbPartD_consigneeTin, "Empty or wrfong format");
			}
			else 
			{
				if(!checkFOrExistINDB(tbPartD_consigneeTin.Text))
				{
					epPartD_consigneeTin.SetError(tbPartD_consigneeTin, "ИИН/БИН грузополучателя не найден в БД ИС ЭСФ");
				}
				else
				{
					epPartD_consigneeTin.Clear();
				}
			}
		}

		private void tbPartD_consigneeCountryCode_TextChanged(object sender, EventArgs e)
		{
			
		}

		private void tbPartD_consigneeName_TextChanged(object sender, EventArgs e)
		{
			tbPartD_consigneeName_Validation();
		}

		private void tbPartD_consigneeName_Validation()
		{
			Regex regex = new Regex(@".{3,255}");
			bool flag = regex.IsMatch(tbPartD_consigneeName.Text);
			if (!flag)
			{
				epPartD_consigneeName.SetError(tbPartD_consigneeName, "Empty or wrong format");
			}
			{
				epPartD_consigneeName.Clear();
			}
		}

		private void tbPartD_consigneeAddress_TextChanged(object sender, EventArgs e)
		{
			Regex regex = new Regex(@".{0,255}");
			bool flag = regex.IsMatch(tbPartD_consigneeAddress.Text);
			if (!flag)
			{
				epPartD_consigneeAddress.SetError(tbPartD_consigneeAddress, "wrong format");
			}
			{
				epPartD_consigneeAddress.Clear();
			}
		}
	}
}
