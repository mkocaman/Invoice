using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ESF_kz.Forms
{
	public partial class panelESFpartBtab : AbstractUCESFpanelTab
	{
		private int sellerIndex;
		private Dictionary<TextBox, bool> tbPartB1_isCorrect = new Dictionary<TextBox,bool>();

		private List<SellerType> statusList = new List<SellerType>();

		internal List<SellerType> getStatusList()
		{
			return statusList;
		}

		public panelESFpartBtab()
		{
			InitializeComponent();
			tbPartB1_isCorrect.Add(tbPartB1_kbe, false);
			tbPartB1_isCorrect.Add(tbPartB1_iik, false);
			tbPartB1_isCorrect.Add(tbPartB1_bik, false);
			tbPartB1_isCorrect.Add(tbPartB1_bank, false);
		}

		internal void setSellerIndex(int index)
		{
			sellerIndex = index;
		}
		internal int getSellerIndex()
		{
			return sellerIndex;
		}

		internal SellerType getCustomerStatusById(int statusId)
		{
			return statusList[statusId];
		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

		private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
		{

		}

		private void label13_Click(object sender, EventArgs e)
		{

		}

		private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void textBox10_TextChanged(object sender, EventArgs e)
		{

		}

		private void tbPartB_tin_TextChanged(object sender, EventArgs e)
		{
			chbxPartB_isJointActivityParticipant.Enabled = false;
			Regex regex = new Regex(@"^\d{12}$");
			bool flag = regex.IsMatch(tbPartB_tin.Text);
			bool isEmpty = tbPartB_tin.Text == "";
			string message = "";
			if (!flag || isEmpty)
			{
				message = "ИИН/БИН поставщика отсутствует или неверного формата";
				epPartB_tin.SetError(tbPartB_tin, message);
				return;
			}
			else if (!checkAvailabilityIn(tbPartB_tin.Text))
			{
				message = "ИИН/БИН поставщика не найден в БД ИС ЭСФ";
				epPartB_tin.SetError(tbPartB_tin, message);
				return;
			}
			else if (checkForBlocking(tbPartB_tin.Text))
			{
				message = "Выписка ЭСФ заблокирована";
				epPartB_tin.SetError(tbPartB_tin, message);
				return;
			}
			else
			{
				chbxPartB_isJointActivityParticipant.Enabled = true;
				epPartB_tin.Clear();
			}
		}

		internal bool setSellerParticipantsCount(int num)
		{
			try
			{
				numUpDown_participantCounter.Value = num;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
			
		}

		internal int getSellerParticipantsCount()
		{
			return (int)numUpDown_participantCounter.Value;
		}


		/*Проверка наличия признака блокировки работы в ИС ЭСФ. Механизм описан в СТПО шифр 398.13024001364901.00.01.03-01.2017.*/
		private bool checkForBlocking(string bin)
		{
			return false;
		}

		private void tbPartB_reorganizedTin_TextChanged(object sender, EventArgs e)
		{
			Regex regex = new Regex(@"^\d{12}$");
			bool flag = regex.IsMatch(tbPartB_reorganizedTin.Text);
			bool isEmpty = tbPartB_reorganizedTin.Text == "";
			string message = "";
			if (!flag || isEmpty)
			{
				message = "БИН реорганизованного лица отсутствует или неверного формата";
				epPartB_reorganizedTin.SetError(tbPartB_reorganizedTin, message);
				return;
			}
			else if (!checkAvailabilityIn(tbPartB_reorganizedTin.Text))
			{
				message = "БИН реорганизованного лица поставщика не найден в БД ИС ЭСФ";
				epPartB_reorganizedTin.SetError(tbPartB_reorganizedTin, message);
				return;
			}
			else
			{
				epPartB_reorganizedTin.Clear();
			}
		}

		/* проверка наличия БИН в БД ИС ЭСФ
		 * Механизм реорганизации описан в СТПО шифр 398.13024001364901.00.02.01-01.2017.*/
		private bool checkAvailabilityIn(string bin)
		{
			return true;
		}

		private void tbPartB_name_TextChanged(object sender, EventArgs e)
		{
			Regex regex = new Regex(@"^.{3,450}$");
			bool flag = regex.IsMatch(tbPartB_name.Text);
			bool isEmpty = tbPartB_name.Text == "";
			if (isEmpty)
			{
				epPartB_name.SetError(tbPartB_name, "Наименование поставщика отсутствует");
			}
			if (!flag)
			{
				epPartB_name.SetError(tbPartB_name, "Наименование поставщика неверного формата");
			}
			else
			{
				epPartB_name.Clear();
			}
		}

		private void tbPartB_shareParticipation_TextChanged(object sender, EventArgs e)
		{
			Regex regex = new Regex(@"^[0][.]\d{0,5}[1-9]$");
			bool flag = regex.IsMatch(tbPartB_shareParticipation.Text);
			if (!flag)
			{
				epPartB_shareParticipation.SetError(tbPartB_shareParticipation, "Укажите долю участия в формате десятичной дроби < 1");
			}
			else
			{
				epPartB_shareParticipation.Clear();
				FormManagerFacade.RecalcAmountsFor(sellerIndex, float.Parse(tbPartB_shareParticipation.Text)); 
			}
		}

		private void tbPartB_address_TextChanged(object sender, EventArgs e)
		{
			Regex regex = new Regex(@"^(.{3,450})?$");
			bool flag = regex.IsMatch(tbPartB_address.Text);
			if (!flag)
			{
				epPartB_address.SetError(tbPartB_address, "Неверный формат адреса");
			}
			else
			{
				epPartB_address.Clear();
			}
		}

		private void tbPartB_certificateSeries_TextChanged(object sender, EventArgs e)
		{
			Regex regex = new Regex(@"^\d{5}$");
			bool flag = regex.IsMatch(tbPartB_certificateSeries.Text);
			if (!flag)
			{
				epPartB_certificateSeries.SetError(tbPartB_certificateSeries, "Серия должна состоять из 5 цифр");
			}
			else
			{
				epPartB_certificateSeries.Clear();
			}
		}

		private void tbPartB_certificateNum_TextChanged(object sender, EventArgs e)
		{
			Regex regex = new Regex(@"^\d{7}$");
			bool flag = regex.IsMatch(tbPartB_certificateNum.Text);
			if (!flag)
			{
				epPartB_certificateNum.SetError(tbPartB_certificateNum, "Номер должен состоять из 7 цифр");
			}
			else
			{
				epPartB_certificateNum.Clear();
			}
		}

		/*Проверка указанной серии и номера свидетельства НДС в данных БД ИС ЭСФ.*/
		private bool isNDSPayer(string series,string number)
		{
			return true;
		}

		private void tbPartB1_kbe_TextChanged(object sender, EventArgs e)
		{
			tbPartB1_isCorrect[tbPartB1_kbe] = false;
			tbPartB1_validation();
		}

		private void tbPartB1_validation()
		{
			bool isEmpty_kbe = tbPartB1_kbe.Text == "";
			bool isEmpty_iik = tbPartB1_iik.Text == "";
			bool isEmpty_bik = tbPartB1_bik.Text == "";
			bool isEmpty_bank = tbPartB1_bank.Text == "";
			bool isEmptyTemp = true;
			bool isSomeNotEmpty = !isEmpty_iik || !isEmpty_bik || !isEmpty_bank || !isEmpty_kbe;
			Regex regex=null;
			ErrorProvider tempEP = null;
			string message = "";

			Dictionary<TextBox, bool> temp = new Dictionary<TextBox, bool>();
			foreach (KeyValuePair<TextBox, bool> item in tbPartB1_isCorrect)
			{
				temp.Add(item.Key,item.Value);
			}
			foreach (KeyValuePair<TextBox,bool> item in temp)
			{
				if (item.Value)
				{
					continue;
				}
				switch (item.Key.Name)
				{
					case "tbPartB1_kbe":
						regex = new Regex(@"^\d{0,2}$");
						tempEP = epPartB1_kbe;
						isEmptyTemp = tbPartB1_kbe.Text == "";
						message = "Neverniy format ili otsutstvuet";
						break;
					case "tbPartB1_iik":
						regex = new Regex(@"^.{20}$");
						tempEP = epPartB1_iik;
						isEmptyTemp = tbPartB1_iik.Text == "";
						message = "Neverniy format ili otsutstvuet";
						break;
					case "tbPartB1_bik":
						regex = new Regex(@"^.{0,8}$");
						tempEP = epPartB1_bik;
						isEmptyTemp = tbPartB1_bik.Text == "";
						message = "Neverniy format ili otsutstvuet";
						break;
					case "tbPartB1_bank":
						regex = new Regex(@"^.{1,200}$");
						tempEP = epPartB1_bank;
						isEmptyTemp = tbPartB1_bank.Text == "";
						message = "Neverniy format ili otsutstvuet";
						break;
					default:
						break;
				}
				if (regex!=null && tempEP!=null)
				{
					ESF_form esfform = (ESF_form)this.TopLevelControl;
					panelESFpartC partC = esfform.getPannel<panelESFpartC>();
					panelESFpartCtab partCtab = partC.getTab();
					if (partCtab.isPublicOffice() || isSomeNotEmpty)
					{
						bool flag = regex.IsMatch(item.Key.Text);
						if (!flag || isEmptyTemp)
						{
							tempEP.SetError(item.Key, message);
							tbPartB1_isCorrect[item.Key] = false;
						}
						else
						{
							tempEP.Clear();
							tbPartB1_isCorrect[item.Key] = true;
						}
					}						
				}				
			}
		}

		internal bool setSellerStatuses(List<SellerType> statuses)
		{
			if (statuses == null)
				return false;
			try
			{
				foreach (SellerType status in statuses)
				{
					switch (status)
					{
						case SellerType.COMMITTENT:
							chbxPartB_isCommitent.Checked = true;
							break;
						case SellerType.BROKER:
							chbxPartB_isBroker.Checked = true;
							break;
						case SellerType.FORWARDER:
							chbxPartB_isForwarder.Checked = true;
							break;
						case SellerType.LESSOR:
							chbxPartB_isLessor.Checked = true;
							break;
						case SellerType.JOINT_ACTIVITY_PARTICIPANT:
							chbxPartB_isJointActivityParticipant.Checked = true;
							break;
						case SellerType.SHARING_AGREEMENT_PARTICIPANT:
							chbxPartB_isSharingAgreementParticipant.Checked = true;
							break;
						case SellerType.EXPORTER:
							chbxPartB_isExporter.Checked = true;
							break;
						case SellerType.TRANSPORTER:
							chbxPartB_isTransporter.Checked = true;
							break;
						case SellerType.PRINCIPAL:
							chbxPartB_isPrincipal.Checked = true;
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

		private void tbPartB1_iik_TextChanged(object sender, EventArgs e)
		{
			tbPartB1_isCorrect[tbPartB1_iik] = false;
			tbPartB1_validation();
		}

		private void tbPartB1_bik_TextChanged(object sender, EventArgs e)
		{

			tbPartB1_isCorrect[tbPartB1_bik] = false;
			tbPartB1_validation();
		}

		private void tbPartB1_bank_TextChanged(object sender, EventArgs e)
		{
			tbPartB1_isCorrect[tbPartB1_bank] = false;
			tbPartB1_validation();
		}

		private void chbxPartB_isSharingAgreementParticipant_CheckedChanged(object sender, EventArgs e)
		{
			TabControl tabControl = ((ESF_form)this.TopLevelControl).getPannel<panelESFpartB>().getTabControll();
			if (chbxPartB_isSharingAgreementParticipant.Checked)
			{
				statusList.Add(SellerType.SHARING_AGREEMENT_PARTICIPANT);
				tabControl.TabPages[0].Text = "Seller-Participant #1";
				tbPartB_shareParticipation.Enabled = true;
				numUpDown_participantCounter.Enabled = true;
				if(tabControl.TabPages[0].Controls[0] == this && numUpDown_participantCounter.Value==1)
				{
					numUpDown_participantCounter.Value = 2;
				}				
			}
			else
			{
				statusList.Remove(SellerType.SHARING_AGREEMENT_PARTICIPANT);				
				tbPartB_shareParticipation.Enabled = false;
				tbPartB_shareParticipation.Text = "";
				if (chbxPartB_isJointActivityParticipant.Checked == false)
				{
					tabControl.TabPages[0].Text = "Seller";
					numUpDown_participantCounter.Value = 1;
					numUpDown_participantCounter.Enabled = false;
				}				
			}
		}

		private void chbxPartB_isCommitent_CheckedChanged(object sender, EventArgs e)
		{
			if(chbxPartB_isCommitent.Checked)
			{
				chbxPartB_isBroker.Checked = false;
				statusList.Add(SellerType.COMMITTENT);
				statusList.Remove(SellerType.BROKER);
			}
			else
			{
				statusList.Remove(SellerType.COMMITTENT);
			}
		}

		private void chbxPartB_isBroker_CheckedChanged(object sender, EventArgs e)
		{
			if(chbxPartB_isBroker.Checked)
			{
				chbxPartB_isCommitent.Checked = false;
				statusList.Add(SellerType.BROKER);
				statusList.Remove(SellerType.COMMITTENT);
			}
			else
			{
				statusList.Remove(SellerType.BROKER);
			}
		}

		private void chbxPartB_isJointActivityParticipant_CheckedChanged(object sender, EventArgs e)
		{
			TabControl tabControl = ((ESF_form)this.TopLevelControl).getPannel<panelESFpartB>().getTabControll();
			if (chbxPartB_isJointActivityParticipant.Checked)
			{
				statusList.Add(SellerType.JOINT_ACTIVITY_PARTICIPANT);
				if(!isJointActivityParticipant(tbPartB_tin.Text))
				{
					chbxPartB_isJointActivityParticipant.Checked = false;
				}
				else
				{					
					tabControl.TabPages[0].Text = "Seller-Participant #1";
					numUpDown_participantCounter.Enabled = true;
					if(numUpDown_participantCounter.Value == 1 && tabControl.TabPages[0].Controls[0] == this)
					numUpDown_participantCounter.Value = 2;
				}
			}
			else
			{
				statusList.Remove(SellerType.JOINT_ACTIVITY_PARTICIPANT);
				
				if(chbxPartB_isSharingAgreementParticipant.Checked == false)
				{
					tabControl.TabPages[0].Text = "Seller";
					numUpDown_participantCounter.Value = 1;
					numUpDown_participantCounter.Enabled = false;
				}				
			}

			((ESF_form)this.TopLevelControl).SetEnableBtnESFpartI(chbxPartB_isPrincipal.Checked || chbxPartB_isJointActivityParticipant.Checked);
		}

		private bool isJointActivityParticipant(string bin)
		{
			return true;
		}

		private void chbxPartB_isPrincipal_CheckedChanged(object sender, EventArgs e)
		{
			if (chbxPartB_isPrincipal.Checked)
			{
				statusList.Add(SellerType.PRINCIPAL);
			}
			else
			{
				statusList.Remove(SellerType.PRINCIPAL);
			}
			((ESF_form)this.TopLevelControl).SetEnableBtnESFpartI(chbxPartB_isPrincipal.Checked || chbxPartB_isJointActivityParticipant.Checked);
		}

		private void numUpDown_participantCounter_ValueChanged(object sender, EventArgs e)
		{

			//cTab creating/removing
			panelESFpartB PanelESFpartB = ((ESF_form)this.TopLevelControl).getPannel<panelESFpartB>();
			TabControl tabControl = PanelESFpartB.getTabControll(); 
			if(tabControl.TabCount < numUpDown_participantCounter.Value)
			{
				int dif = (int)numUpDown_participantCounter.Value - tabControl.TabCount;
				for (int i = 0; i < dif; i++)
				{
					panelESFpartBtab PanelESFPartBtab = PanelESFpartB.CreateTab("Seller-Participant #" + (tabControl.TabCount + 1));
					PanelESFPartBtab.numUpDown_participantCounter.Visible = false;
					PanelESFPartBtab.chbxPartB_isSharingAgreementParticipant.Enabled = false;
					PanelESFPartBtab.chbxPartB_isSharingAgreementParticipant.Checked = chbxPartB_isSharingAgreementParticipant.Checked;
					PanelESFPartBtab.chbxPartB_isJointActivityParticipant.Checked = chbxPartB_isJointActivityParticipant.Checked;
					PanelESFPartBtab.chbxPartB_isJointActivityParticipant.Enabled = false;
					PanelESFPartBtab.l_participantCounter.Visible = false;
				}				
			}
			else
			{
				int dif = tabControl.TabCount - (int)numUpDown_participantCounter.Value;
				for (int i = 0; i < dif; i++)
				{
					tabControl.TabPages.Remove(tabControl.TabPages[tabControl.TabCount - 1]);
				}
				if(numUpDown_participantCounter.Value == 1)
				{
					chbxPartB_isSharingAgreementParticipant.Checked = false;
					chbxPartB_isJointActivityParticipant.Checked = false;
				}
			}

			//hTab creating/removing
			ESF_form esf = (ESF_form)this.TopLevelControl;
			panelESFpartH panelH = esf.getPannel<panelESFpartH>();
			panelESFpartC panelC = esf.getPannel<panelESFpartC>();
			int customerParticipantsCount = panelC.getCustomerParticipantsCount();
			int sellerParticipantsCount = (int)numUpDown_participantCounter.Value;
			List<int> indexes = panelH.getSellerIndexes();
			

			if (sellerParticipantsCount > 1)
			{
				esf.SetEnableBtnESFPartH(true);
				int hTabSellerCount = indexes.Count;
				int difference = sellerParticipantsCount - hTabSellerCount;
				if (difference>0)
				{
					for (int i = 1; i <= difference; i++)
					{
						panelESFpartHtab hTab = panelH.CreateSellerTab(" Participant #" + (hTabSellerCount+i));
					}
				}
				else
				{
					for (int i = 0; i > difference; i--)
					{
						panelH.RemoveLastSellerTab();
					}
				}
			}
			else
			{
				panelH.RemoveAllSellerTabs();
				if (customerParticipantsCount < 2)
				{
					esf.SetEnableBtnESFPartH(false);
				}
			}
			

		}

		internal string getSellerParticipantTin()
		{
			return tbPartB_tin.Text;
		}

		internal bool setSellerParticipantTin(string tin)
		{
			try
			{
				tbPartB_tin.Text = tin;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getSellerParticipantReorgTin()
		{
			return tbPartB_reorganizedTin.Text;
		}

		internal bool setSellerParticipantReorgTin(string tin)
		{
			try
			{
				tbPartB_reorganizedTin.Text = tin;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getSellerTrailer()
		{
			return tbPartB_trailer.Text;
		}

		internal bool setSellerTrailer(string trailer)
		{
			try
			{
				tbPartB_trailer.Text = trailer;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getSellerTin()
		{
			return tbPartB_tin.Text;
		}

		internal bool setSellerTin(string tin)
		{
			try
			{
				tbPartB_tin.Text = tin;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getSellerAddress()
		{
			return tbPartB_address.Text;
		}

		internal bool setSellerAddress(string address)
		{
			try
			{
				tbPartB_address.Text = address;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
			
		}

		internal string getSellerReorgTin()
		{
			return tbPartB_reorganizedTin.Text;
		}

		internal bool setSellerReorgTin(string tin)
		{
			try
			{
				tbPartB_reorganizedTin.Text = tin;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal int getSellerStatusesCount()
		{
			return statusList.Count;
		}

		private void chbxPartB_isForwarder_CheckedChanged(object sender, EventArgs e)
		{
			if (chbxPartB_isForwarder.Checked)
			{
				statusList.Add(SellerType.FORWARDER);
			}
			else
			{
				statusList.Remove(SellerType.FORWARDER);
			}
		}

		private void chbxPartB_isLessor_CheckedChanged(object sender, EventArgs e)
		{
			if (chbxPartB_isLessor.Checked)
			{
				statusList.Add(SellerType.LESSOR);
			}
			else
			{
				statusList.Remove(SellerType.LESSOR);
			}
		}

		private void chbxPartB_isExporter_CheckedChanged(object sender, EventArgs e)
		{
			if (chbxPartB_isExporter.Checked)
			{
				statusList.Add(SellerType.EXPORTER);
			}
			else
			{
				statusList.Remove(SellerType.EXPORTER);
			}
		}

		private void chbxPartB_isTransporter_CheckedChanged(object sender, EventArgs e)
		{
			if (chbxPartB_isTransporter.Checked)
			{
				statusList.Add(SellerType.TRANSPORTER);
			}
			else
			{
				statusList.Remove(SellerType.TRANSPORTER);
			}
		}

		internal SellerType getSellerStatusById(int statusId)
		{
			return statusList[statusId];
		}

		internal string getSellerKbe()
		{
			return tbPartB1_kbe.Text;
		}

		internal bool setSellerKbe(string kbe)
		{
			try
			{
				tbPartB1_kbe.Text = kbe;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal bool getSellerIsBranchNonResiden()
		{
			return chbxPartB_isBranchNonResident.Checked;
		}

		internal bool setSellerIsBranchNonResiden(bool isChecked)
		{
			try
			{
				chbxPartB_isBranchNonResident.Checked = isChecked;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getSellerName()
		{
			return tbPartB_name.Text;
		}

		internal bool setSellerName(string name)
		{
			try
			{
				tbPartB_name.Text = name;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getSellerIik()
		{
			return tbPartB1_iik.Text;
		}

		internal bool setSellerIik(string iik)
		{
			try
			{
				tbPartB1_iik.Text = iik;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getSellerCertificateNum()
		{
			return tbPartB_certificateNum.Text;
		}

		internal bool setSellerCertificateNum(string num)
		{
			try
			{
				tbPartB_certificateNum.Text = num;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getSellerCertificateSeries()
		{
			return tbPartB_certificateSeries.Text;
		}

		internal bool setSellerCertificateSeries(string series)
		{
			try
			{
				tbPartB_certificateSeries.Text = series;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getSellerBranchTin()
		{
			return tbPartB_branchTin.Text;
		}

		internal bool setSellerBranchTin(string branchTin)
		{
			try
			{
				tbPartB_branchTin.Text = branchTin;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getSellerBank()
		{
			return tbPartB1_bank.Text;
		}

		internal bool setSellerBank(string bank)
		{
			try
			{
				tbPartB1_bank.Text = bank;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getSellerBik()
		{
			return tbPartB1_bik.Text;
		}

		internal bool setSellerBik(string bik)
		{
			try
			{
				tbPartB1_bik.Text = bik;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal float getSellerShareParticipation()
		{
			return tbPartB_shareParticipation.Text == ""? 0:float.Parse(tbPartB_shareParticipation.Text);
		}

		internal bool setSellerShareParticipation(float part)
		{
			try
			{
				tbPartB_shareParticipation.Text = part.ToString();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
