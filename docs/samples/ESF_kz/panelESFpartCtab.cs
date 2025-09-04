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
	public partial class panelESFpartCtab : AbstractUCESFpanelTab
	{
		private const string NATIONAL_BANK_BIN = "941240001151";
		private List<CustomerType> statusList = new List<CustomerType>();

		internal List<CustomerType>  getStatusList()
		{
			return statusList;
		}

		public panelESFpartCtab()
		{
			InitializeComponent();
		}

		private void numUpDown_participantCounter_ValueChanged(object sender, EventArgs e)
		{
			//cTab Create/remove
			panelESFpartC PanelESFpartC = (panelESFpartC)this.Parent.Parent.Parent;
			TabControl tabControl = PanelESFpartC.getTabControll();
			if (tabControl.TabCount < numUpDown_participantCounter.Value)
			{
				int dif = (int)numUpDown_participantCounter.Value - tabControl.TabCount;
				for (int i = 0; i < dif; i++)
				{
					panelESFpartCtab PanelESFPartCtab = PanelESFpartC.CreateTab("Customer-Participant #" + (tabControl.TabCount + 1));
					PanelESFPartCtab.numUpDown_participantCounter.Visible = false;
					PanelESFPartCtab.chbxPartC_isSharingAgreementParticipant.Enabled = false;
					PanelESFPartCtab.chbxPartC_isSharingAgreementParticipant.Checked = chbxPartC_isSharingAgreementParticipant.Checked;
					PanelESFPartCtab.chbxPartC_isJointActivityParticipant.Checked = chbxPartC_isJointActivityParticipant.Checked;
					PanelESFPartCtab.chbxPartC_isJointActivityParticipant.Enabled = false;
					PanelESFPartCtab.l_participantCounter.Visible = false;
				}
			}
			else
			{
				int dif = tabControl.TabCount - (int)numUpDown_participantCounter.Value;
				for (int i = 0; i < dif; i++)
				{
					tabControl.TabPages.Remove(tabControl.TabPages[tabControl.TabCount - 1]);
				}
				if (numUpDown_participantCounter.Value == 1)
				{
					chbxPartC_isSharingAgreementParticipant.Checked = false;
					chbxPartC_isJointActivityParticipant.Checked = false;
				}
			}

			//hTab creating/removing
			ESF_form esf = (ESF_form)this.TopLevelControl;
			panelESFpartH panelH = esf.getPannel<panelESFpartH>();
			panelESFpartB panelB = esf.getPannel<panelESFpartB>();
			int customerParticipantsCount = (int)numUpDown_participantCounter.Value;
			int sellerParticipantsCount = panelB.getSellerParticipantsCount();
			List<int> indexes = panelH.getCustomerIndexes();


			if (customerParticipantsCount > 1)
			{
				esf.SetEnableBtnESFPartH(true);
				int hTabCustomerCount = indexes.Count;
				int difference = customerParticipantsCount - hTabCustomerCount;
				if (difference > 0)
				{
					for (int i = 1; i <= difference; i++)
					{
						panelESFpartHtab hTab = panelH.CreateCustomerTab(" Participant #" + (hTabCustomerCount + i));
					}
				}
				else
				{
					for (int i = 0; i > difference; i--)
					{
						panelH.RemoveLastCustomerTab();
					}
				}
			}
			else
			{
				panelH.RemoveAllCustomerTabs();
				if (sellerParticipantsCount < 2)
				{
					esf.SetEnableBtnESFPartH(false);
				}
			}
			
		}

		internal int getCustomerParticipantsCount()
		{
			return (int)numUpDown_participantCounter.Value;
		}

		internal bool setCustomerParticipantsCount(int count)
		{
			try
			{
				numUpDown_participantCounter.Value = count;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool isPublicOffice()
		{
			return this.chbxPartC_isPublicOffice.Checked;
		}

		private void tbPartC_tin_TextChanged(object sender, EventArgs e)
		{
			tbPartC_tin_Validation();
		}

		private void tbPartC_tin_Validation()
		{
			Regex regex = null;
			bool isNonResident = chbxPartC_isNonResident.Checked;
			bool isRetail = chbxPartC_isRetail.Checked;
			bool isEmpty = tbPartC_tin.Text == "";
			string message = "";
			chbxPartC_isSharingAgreementParticipant.Enabled = false;
			if (isNonResident)
			{
				regex = new Regex(@"^\d{0-50}$");
				message = "Неверный формат";
			}
			else if(isRetail)
			{
				regex = new Regex(@"^(\d{0})|(\d{12})$");
				message = "Неверный формат";
			}
			else
			{
				regex = new Regex(@"^\d{12}$");
				message = "Неверный формат";
			}

			bool flag = regex.IsMatch(tbPartC_tin.Text);
			if(!flag)
			{
				epPartC_tin.SetError(tbPartC_tin, message);
			}
			else
			{
				if(!isNonResident )
				{
					if (!checkForExistInDB(tbPartC_tin.Text))
					{
						message = "ИИН/БИН получателя отсутствует в БДИС ЭСФ";
						epPartC_tin.SetError(tbPartC_tin, message);
					}
					else if (checkForBlocking(tbPartC_tin.Text))
					{
						message = "«Получатель заблокирован».";
						epPartC_tin.SetError(tbPartC_tin, message);
					}
					else
					{
						epPartC_tin.Clear();
						chbxPartC_isSharingAgreementParticipant.Enabled = true;
					}
				}

			}

		}

		internal string getCustomerParticipantsTin()
		{
			return tbPartC_tin.Text;
		}

		internal bool setCustomerParticipantTin(string tin)
		{
			try
			{
				tbPartC_tin.Text = tin;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getCustomerParticipantsReorgTin()
		{
			return tbPartC_reorganizedTin.Text;
		}

		internal bool setCustomerParticipantsReorgTin(string reorgTin)
		{
			try
			{
				tbPartC_reorganizedTin.Text = reorgTin;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal void chbxPartC_isPrincipal_setState(bool flag)
		{
			chbxPartC_isPrincipal.Checked = flag;
		}

		//проверка на наличие ИИН/БИН в БД
		private bool checkForExistInDB(string text)
		{
			return true;
		}

		/*Проверка наличия признака блокировки работы в ИС ЭСФ. Механизм описан в СТПО шифр 398.13024001364901.00.01.03-01.2017.*/
		private bool checkForBlocking(string bin)
		{
			return false;
		}

		private void tbPartC_reorganizedTin_TextChanged(object sender, EventArgs e)
		{
			tbPartC_reorganizedTin_Validation();
		}

		private void tbPartC_reorganizedTin_Validation()
		{
			Regex regex = new Regex(@"^\d{12}$");
			bool isEmpty = tbPartC_reorganizedTin.Text == "";
			bool flag = regex.IsMatch(tbPartC_reorganizedTin.Text);
			string message = "";
			if(!isEmpty)
			{
				if(!flag)
				{
					message = "Неверный формат";
					epPartC_reorganizedTin.SetError(tbPartC_reorganizedTin, message);
				}
				else
				{
					if(!checkForExistInDB(tbPartC_reorganizedTin.Text))
					{
						message = "БИН реорганизованного лица получателя отсутствует в БД ИС ЭСФ";
						epPartC_reorganizedTin.SetError(tbPartC_reorganizedTin, message);
					}
						/*if(!isExistLinkToMainInDB())
						{
							message = "БИН реорганизованного лица получателя отсутствует в БД ИС ЭСФ";
							epPartC_reorganizedTin.SetError(tbPartC_reorganizedTin, message);
						}*/
					else
					{
						epPartC_reorganizedTin.Clear();
					}
				}
			}
		}
		/*Проверка наличия в БД связи структурной единицы с головным предприятием по данным ЕХД.При отсутствии связи сообщение: 
		 * "Указанный БИН не является структурной единицей (филиалом) получателя, указанного в поле 16 "ИИН/БИН*/
		private bool isExistLinkToMainInDB()
		{
			return true;
		}

		private void tbPartC_name_TextChanged(object sender, EventArgs e)
		{
			tbPartC_name_Validation();
		}

		private void tbPartC_name_Validation()
		{
			Regex regex = new Regex(@"^.{3,450}$");
			bool flag = regex.IsMatch(tbPartC_name.Text);
			string message = "";

			if (!flag)
			{
				message = "Наименование получателя отсутствует";
				epPartC_reorganizedTin.SetError(tbPartC_reorganizedTin, message);
			}
			else
			{
				epPartC_reorganizedTin.Clear();				
			}			
		}

		internal bool setCustomerBranchTin(string branchTin)
		{
			try
			{
				tbPartC_branchTin.Text = branchTin;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}


		internal string getCustomerCountryCode()
		{
			return tbPartC_countryCode.Text;
		}

		internal bool setCustomerStatuses(List<CustomerType> statuses)
		{
			if (statuses == null)
			{
				return false;
			}
			try
			{
				foreach (CustomerType status in statuses)
				{
					switch (status)
					{
						case CustomerType.COMMITTENT:
							chbxPartC_isCommitent.Checked = true;
							break;
						case CustomerType.BROKER:
							chbxPartC_isBroker.Checked = true;
							break;
						case CustomerType.LESSEE:
							chbxPartC_isLessee.Checked = true;
							break;
						case CustomerType.JOINT_ACTIVITY_PARTICIPANT:
							chbxPartC_isJointActivityParticipant.Checked = true;
							break;
						case CustomerType.PUBLIC_OFFICE:
							chbxPartC_isPublicOffice.Checked = true;
							break;
						case CustomerType.NONRESIDENT:
							chbxPartC_isNonResident.Checked = true;
							break;
						case CustomerType.SHARING_AGREEMENT_PARTICIPANT:
							chbxPartC_isSharingAgreementParticipant.Checked = true;
							break;
						case CustomerType.PRINCIPAL:
							chbxPartC_isPrincipal.Checked = true;
							break;
						case CustomerType.RETAIL:
							chbxPartC_isRetail.Checked = true;
							break;
						case CustomerType.INDIVIDUAL:
							chbxPartC_isIndividual.Checked = true;
							break;						
						default:
							break;
					}
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}

		}

		internal bool setCustomerCountryCode(string code)
		{
			try
			{
				tbPartC_countryCode.Text = code;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getCustomerName()
		{
			return tbPartC_name.Text;
		}

		internal string getCustomerReorgTin()
		{
			return tbPartC_reorganizedTin.Text;
		}

		internal bool setCustomerRoergTin(string tin)
		{
			try
			{
				tbPartC_reorganizedTin.Text = tin;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal float getCustomerShareParticipation()
		{
			return tbPartC_shareParticipation.Text == ""? 0: float.Parse(tbPartC_shareParticipation.Text);
		}

		internal int getCustomerStatusesCount()
		{
			return statusList.Count;
		}

		internal CustomerType getCustomerStatusById( int statusId)
		{
			return statusList[statusId];
		}

		internal bool setCustomerShareParticipation(float sharePart)
		{
			try
			{
				tbPartC_shareParticipation.Text = sharePart.ToString();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getCustomerTrailer()
		{
			return tbPartC_trailer.Text;
		}

		internal bool setCustomerTrailer(string trailer)
		{
			try
			{
				tbPartC_trailer.Text = trailer;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal bool setCustomerName(string name)
		{
			try
			{
				tbPartC_name.Text = name;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getCustomerAddress()
		{
			return tbPartC_address.Text;
		}

		internal bool setCustomerAddress(string address)
		{
			try
			{
				tbPartC_address.Text = address;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getCustomerTin()
		{
			return tbPartC_tin.Text;
		}

		internal string getCustomerBranchTin()
		{
			return tbPartC_branchTin.Text;
		}



		internal bool setCustomerTin(string tin)
		{
			try
			{
				tbPartC_tin.Text = tin;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private void tbPartC_shareParticipation_TextChanged(object sender, EventArgs e)
		{
			tbPartC_shareParticipation_Validation();
		}

		private void tbPartC_shareParticipation_Validation()
		{

			Regex regex = new Regex(@"^[0][.]\d{0,5}[1-9]$");
			bool flag = regex.IsMatch(tbPartC_shareParticipation.Text);
			if (!flag)
			{
				epPartC_shareParticipation.SetError(tbPartC_shareParticipation, "Укажите долю участия в формате десятичной дроби < 1");
			}
			else
			{
				epPartC_shareParticipation.Clear();
			}
			
		}

		private void chbxPartC_isJointActivityParticipant_CheckedChanged(object sender, EventArgs e)
		{
			TabControl tabControl = ((ESF_form)this.TopLevelControl).getPannel<panelESFpartC>().getTabControll();
			if (chbxPartC_isJointActivityParticipant.Checked)
			{
				statusList.Add(CustomerType.JOINT_ACTIVITY_PARTICIPANT);
				tabControl.TabPages[0].Text = "Customer-Participant #1";
				tbPartC_shareParticipation.Enabled = true;
				numUpDown_participantCounter.Enabled = true;
				if (numUpDown_participantCounter.Value == 1 && tabControl.TabPages[0].Controls[0] == this)
					numUpDown_participantCounter.Value = 2;
				
			}
			else
			{
				statusList.Remove(CustomerType.JOINT_ACTIVITY_PARTICIPANT);
				if (chbxPartC_isSharingAgreementParticipant.Checked == false)
				{
					tabControl.TabPages[0].Text = "Customer";
					tbPartC_shareParticipation.Enabled = false;
					tbPartC_shareParticipation.Text = "";
					numUpDown_participantCounter.Value = 1;
					numUpDown_participantCounter.Enabled = false;
				}	
			}

			((ESF_form)this.TopLevelControl).SetEnableBtnESFpartJ(chbxPartC_isPrincipal.Checked || chbxPartC_isJointActivityParticipant.Checked);
		}

		private void chbxPartC_isSharingAgreementParticipant_CheckedChanged(object sender, EventArgs e)
		{			
			if (chbxPartC_isSharingAgreementParticipant.Checked)
			{
				statusList.Add(CustomerType.SHARING_AGREEMENT_PARTICIPANT);
				if (!checkForSharingAgreementParticipant(tbPartC_tin.Text))
				{
					chbxPartC_isSharingAgreementParticipant.Checked = false;
				}
			}
			else
			{
				statusList.Remove(CustomerType.SHARING_AGREEMENT_PARTICIPANT);
			}
			chbxPartC_isRetail_Validating();

			TabControl tabControl = ((ESF_form)this.TopLevelControl).getPannel<panelESFpartC>().getTabControll();
			if (chbxPartC_isSharingAgreementParticipant.Checked)
			{
				tabControl.TabPages[0].Text = "Customer-Participant #1";
				tbPartC_shareParticipation.Enabled = true;
				numUpDown_participantCounter.Enabled = true;
				if (tabControl.TabPages[0].Controls[0] == this && numUpDown_participantCounter.Value == 1)
				{
					numUpDown_participantCounter.Value = 2;
				}
			}
			else
			{
				
				tbPartC_shareParticipation.Enabled = false;
				tbPartC_shareParticipation.Text = "";
				if (chbxPartC_isJointActivityParticipant.Checked == false)
				{
					tabControl.TabPages[0].Text = "Customer";
					numUpDown_participantCounter.Value = 1;
					numUpDown_participantCounter.Enabled = false;
				}
					
			}
		}

		private bool checkForSharingAgreementParticipant(string tin)
		{
			return true;
		}

		private void tbPartc_countryCode_TextChanged(object sender, EventArgs e)
		{
			tbPartC_countryCode_Validation();
		}

		private void tbPartC_countryCode_Validation()
		{
			Regex regex = new Regex(@"^.{0,2}$");
			bool flag = regex.IsMatch(tbPartC_countryCode.Text);
			if (!flag)
			{
				epPartC_countryCode.SetError(tbPartC_countryCode, "neverniy format adressa");
			}
			else
			{
				epPartC_countryCode.Clear();
			}
		}

		private void tbPartC_address_TextChanged(object sender, EventArgs e)
		{
			tbPartC_address_Validation();
		}

		private void tbPartC_address_Validation()
		{
			Regex regex = new Regex(@"^.{3,450}$");
			bool flag = regex.IsMatch(tbPartC_address.Text);
			if (!flag)
			{
				epPartC_address.SetError(tbPartC_address, "neverniy format adressa");
			}
			else
			{
				epPartC_address.Clear();
			}
		}

		private void tbPartC_trailer_TextChanged(object sender, EventArgs e)
		{
			tbPartC_trailer_Validation();
		}

		private void tbPartC_trailer_Validation()
		{
			Regex regex = new Regex(@"^.{0,255}$");
			bool flag = regex.IsMatch(tbPartC_trailer.Text);
			if(!flag)
			{
				epPartC_trailer.SetError(tbPartC_trailer, "Neverniy format");
			}
			else
			{
				epPartC_trailer.Clear();
			}
		}

		private void chbxPartC_isCommitent_CheckedChanged(object sender, EventArgs e)
		{
			chbxPartC_isRetail_Validating();
			if (chbxPartC_isCommitent.Checked)
			{
				chbxPartC_isBroker.Checked = false;
				statusList.Add(CustomerType.COMMITTENT);
				statusList.Remove(CustomerType.BROKER);
			}
			else
			{
				statusList.Remove(CustomerType.COMMITTENT);
			}
			
		}

		private void chbxPartC_isBroker_CheckedChanged(object sender, EventArgs e)
		{
			if (chbxPartC_isBroker.Checked)
			{
				chbxPartC_isCommitent.Checked = false;
				statusList.Add(CustomerType.BROKER);
				statusList.Remove(CustomerType.COMMITTENT);
			}
			else
			{
				statusList.Remove(CustomerType.BROKER);
			}
			
		}

		private void chbxPartC_isPrincipal_CheckedChanged(object sender, EventArgs e)
		{
			if (chbxPartC_isPrincipal.Checked)
			{
				statusList.Add(CustomerType.PRINCIPAL);
			}
			else
			{
				statusList.Remove(CustomerType.PRINCIPAL);
			}
			chbxPartC_isRetail_Validating();
			((ESF_form)this.TopLevelControl).SetEnableBtnESFpartJ(chbxPartC_isPrincipal.Checked || chbxPartC_isJointActivityParticipant.Checked);
		}

		private void chbxPartC_isRetail_CheckedChanged(object sender, EventArgs e)
		{
			if (chbxPartC_isRetail.Checked)
			{
				statusList.Add(CustomerType.RETAIL);
			}
			else
			{
				statusList.Remove(CustomerType.RETAIL);
			}
			chbxPartC_isRetail_Validating();	
		}

		internal string getPublicOfficeBik()
		{
			return tbPartC1_bik.Text;
		}

		internal string getPublicOfficeIik()
		{
			return tbPartC1_iik.Text;
		}

		internal bool setPublicOfficeIik(string iik)
		{
			try
			{
				tbPartC1_iik.Text = iik;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getPublicOfficePayPurpose()
		{
			return tbPartC1_payPurpose.Text;
		}

		internal string getPublicOfficeProductCode()
		{
			return tbPartC1_productCode.Text;
		}

		internal bool setPublicOfficeProductCode(string code)
		{
			try
			{
				tbPartC1_productCode.Text = code;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal bool setPublicOfficePayPurpose(string purpose)
		{
			try
			{
				tbPartC1_payPurpose.Text = purpose;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal bool setPublicOfficeBik(string bik)
		{
			try
			{
				tbPartC1_bik.Text = bik;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private void chbxPartC_isRetail_Validating()
		{
			if (!hasPermission() && (chbxPartC_isCommitent.Checked || chbxPartC_isPrincipal.Checked || chbxPartC_isLessee.Checked || chbxPartC_isPublicOffice.Checked || chbxPartC_isSharingAgreementParticipant.Checked))
			{
				epPartC_CustomerType.SetError(l_PartC_CustomerType, "В категории получателя при выборе категории I дополнительно нельзя выбрать категории A, C, E, G, H.");
			}
			else
			{
				epPartC_CustomerType.Clear();
			}
		}

		private bool hasPermission()
		{
			return true;
		}

		private void chbxPartC_isLessee_CheckedChanged(object sender, EventArgs e)
		{
			chbxPartC_isRetail_Validating();
			if (chbxPartC_isLessee.Checked)
			{
				statusList.Add(CustomerType.LESSEE);
			}
			else
			{
				statusList.Remove(CustomerType.LESSEE);
			}
			
		}

		private void chbxPartC_isPublicOffice_CheckedChanged(object sender, EventArgs e)
		{
			chbxPartC_isRetail_Validating();
			if(chbxPartC_isPublicOffice.Checked)
			{
				tbPartC1_iik.Enabled = true;
				tbPartC1_productCode.Enabled = true;
				tbPartC1_payPurpose.Enabled = true;
				tbPartC1_bik.Text = "KKMFKZ2A";
				statusList.Add(CustomerType.PUBLIC_OFFICE);
			}
			else
			{
				tbPartC1_iik.Enabled = false;
				tbPartC1_productCode.Enabled = false;
				tbPartC1_payPurpose.Enabled = false;
				tbPartC1_bik.Text = "";
				statusList.Remove(CustomerType.PUBLIC_OFFICE);
			}
			
		}

		private void tbPartC1_iik_TextChanged(object sender, EventArgs e)
		{
			tbPartC1_iik_Validation();
		}

		private void tbPartC1_iik_Validation()
		{
			Regex regex = new Regex(@"^.{0,20}$");
			bool flag = regex.IsMatch(tbPartC1_iik.Text);
			
			if(!flag)
			{
				epPartC1_iik.SetError(tbPartC1_iik, "neverniy format");
			}
			{
				epPartC1_iik.Clear();
			}
		}

		private void tbPartC1_productCode_TextChanged(object sender, EventArgs e)
		{
			tbPartC1_productCode_Validation();
		}

		private void tbPartC1_productCode_Validation()
		{
			Regex regex = new Regex(@"^\d{0,10}");
			bool flag = regex.IsMatch(tbPartC1_productCode.Text);

			if (!flag)
			{
				epPartC1_productCode.SetError(tbPartC1_productCode, "Neverniy format");

			}
			else
			{
				epPartC1_productCode.Clear();
			}
		}

		private void tbPartC1_payPurpose_TextChanged(object sender, EventArgs e)
		{
			tbPartC1_payPurpose_Validation();
		}

		private void tbPartC1_payPurpose_Validation()
		{
			/*string temp = tbPartC1_payPurpose.Text.Trim();
			Regex regex = new Regex(@"['\t']|['\n']|[':']{1}");
			temp = regex.Replace(temp," ");
			tbPartC1_payPurpose.Text = temp;*/

			Regex regex = new Regex(@".{1,240}");

			bool flag = regex.IsMatch(tbPartC1_payPurpose.Text);
			if (!flag)
			{
				epPartC1_payPurpose.SetError(tbPartC1_payPurpose, "otsutstvuet ili neverniy format");
			}
			else
			{
				epPartC1_payPurpose.Clear();
			}
		}

		private void chbxPartC_isNonResident_CheckedChanged(object sender, EventArgs e)
		{
			if (chbxPartC_isNonResident.Checked)
			{
				statusList.Add(CustomerType.NONRESIDENT);
			}
			else
			{
				statusList.Remove(CustomerType.NONRESIDENT);
			}			
		}

		private void chbxPartC_isIndividual_CheckedChanged(object sender, EventArgs e)
		{
			if (chbxPartC_isIndividual.Checked)
			{
				statusList.Add(CustomerType.INDIVIDUAL);
			}
			else
			{
				statusList.Remove(CustomerType.INDIVIDUAL);
			}
		}
	}
}
