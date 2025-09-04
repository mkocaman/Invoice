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
	public partial class panelESFpartA : AbstractUCESFpanel
	{

		private const string DOWN_TIME = "Простой системы";
		private const string UNLAWFUL_REMOVAL_REGISTRATIONE = "Блокирование доступа к Системе";
		private const string MISSING_REQUIREMENT = "Отсутствовало требование по выписке ЭСФ";

		public panelESFpartA()
		{
			InitializeComponent();
			ClearDateTextBoxes();
			/*
			//Проверка -1.1 Номер учетной системы-
			tbAccSysNum_Validating(this.tbPartA_AccSysNum, null);
			//Иницализация -2. Дата выписки- текущей датой
			this.dtpPartA_Date.Value = DateTime.Now;


			//Добавление делегата проверки -1.1 Номер учетной системы-
			this.tbPartA_AccSysNum.Validating += tbAccSysNum_Validating;
			//Добавление делегата проверки -Причина выписки на бумажном носителе-
			this.combxPartA_PaperESFReason.Validating += CombxPartA_PaperESFReason_Validating;
			//Добавление делегата проверки -2.1 Дата выписки на бумажном носителе-
			this.dtpPartA_PaperESFDate.Validating += DtpPartA_PaperESFDate_Validating;*/
		}

		private void ClearDateTextBoxes()
		{
			tbPartA_AddedESFDate.Text = "";
			tbPartA_CorrectedESFDate.Text = "";
			tbPartA_Date.Text = "";
			tbPartA_PaperESFDate.Text = "";
			tbPartA_TurnoverDate.Text = "";
		}

		internal string getOperatorFullname()
		{
			return tbPartA_operatorFullname.Text;
		}

		internal bool setOperatorFullname(string name)
		{
			try
			{
				tbPartA_operatorFullname.Text = name;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getAddedESFNum()
		{
			return tbPartA_AddedESFAccSysNum.Text;
		}

		internal bool setAddedESFNum(string num)
		{
			try
			{
				tbPartA_AddedESFAccSysNum.Text = num;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getAddedESFRegistrationNum()
		{
			return tbPartA_AddedESFNum.Text;
		}

		internal bool setAddedESFRegistrationNum(string num)
		{
			try
			{
				tbPartA_AddedESFNum.Text = num;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getCorrectedESFRegistrationNum()
		{
			return tbPartA_CorrectedESFNum.Text;
		}

		internal bool setCorrectedESFRegistrationNum(string num)
		{
			try
			{
				tbPartA_CorrectedESFNum.Text = num;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}



		internal string getCorrectedESFNum()
		{
			return tbPartA_CorrectedESFAccSysNum.Text;
		}

		internal bool setCorrectedESFNum(string num)
		{
			try
			{
				tbPartA_CorrectedESFAccSysNum.Text = num;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal DateTime getAddedESFDate()
		{
			return tbPartA_AddedESFDate.Text == "" ? new DateTime() : dtpPartA_AddedESFDate.Value;
		}

		internal bool setAddedESFDate(DateTime dateTime)
		{
			try
			{
				dtpPartA_AddedESFDate.Value = dateTime;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal DateTime getCorrectedESFDate()
		{
			return tbPartA_CorrectedESFDate.Text == "" ? new DateTime() : dtpPartA_CorrectedESFDate.Value;
		}

		internal bool setCorrectedESFDate(DateTime dateTime)
		{
			try
			{
				dtpPartA_CorrectedESFDate.Value = dateTime;
				return true;
			}
			catch (Exception)
			{
				return false;
			}			
		}

		internal bool isCorrectedESF()
		{
			return chbxPartA_isCorrectedESF.Checked;
		}

		internal bool isAddedESF()
		{
			return chbxPartA_isAddedESF.Checked;
		}


		//Делегата проверки -Причина выписки на бумажном носителе-
		private void CombxPartA_PaperESFReason_Validating(object sender, CancelEventArgs e)
		{
			switch (combxPartA_PaperESFReason.Text)
			{
				case MISSING_REQUIREMENT:
				case UNLAWFUL_REMOVAL_REGISTRATIONE:
				case DOWN_TIME:
					epPartA_PaperESFReason.Clear();
					break;
				default:
					epPartA_PaperESFReason.SetError(combxPartA_PaperESFReason, "Выберите причину");
					break;
			}
		}

		internal bool setInvoiceTurnoverDate(DateTime turnoverDate)
		{
			try
			{
				dtpPartA_TurnoverDate.Value = turnoverDate;
				return true;
			}
			catch
			{
				return false;
			}
		}


		internal bool setInvoiceType(InvoiceType invoiceType)
		{
			try
			{
				if (invoiceType==InvoiceType.ADDITIONAL_INVOICE)
				{
					chbxPartA_isAddedESF.Checked = true;
				}
				else if (invoiceType == InvoiceType.FIXED_INVOICE)
				{
					chbxPartA_isCorrectedESF.Checked = true;
				}
				else
				{
					chbxPartA_isAddedESF.Checked = false;
					chbxPartA_isCorrectedESF.Checked = false;
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal void highLightTurnoverDate()
		{
			highLightTextBox(tbPartA_TurnoverDate);
			highLightDTP(dtpPartA_TurnoverDate);
		}

		private void highLightDTP(DateTimePicker dtp)
		{
			dtp.CalendarForeColor = Color.FromArgb(134, 250, 131);
		}

		private void highLightTextBox(TextBox tb)
		{
			tb.BackColor = Color.FromArgb(134,250,131);
		}

		//Делегат проверки -1.1 Номер учетной системы-
		private void tbAccSysNum_Validating(object sender, CancelEventArgs e)
		{
			Regex regex = new Regex(@"^\d{1,30}$");
			bool flag = regex.IsMatch(tbPartA_AccSysNum.Text);
			if (!flag)
			{
				epPartA_Date.SetError(tbPartA_AccSysNum, "Номер учетной системы отсутствует");
			}
			else
			{
				epPartA_Date.Clear();
			}
		}	

		private void dtpDate_Validating(object sender, CancelEventArgs e)
		{

		}
		

		private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
		{

		}

		private void chbxPartA_isPaperESF_CheckedChanged(object sender, EventArgs e)
		{
			if(this.chbxPartA_isPaperESF.Checked)
			{
				l_PaperESFDate.Enabled = true;
				l_PaperESFReason.Enabled = true;
				combxPartA_PaperESFReason.Enabled = true;
				tbPartA_PaperESFDate.Enabled = true;
				CombxPartA_PaperESFReason_Validating(this.combxPartA_PaperESFReason, null);
			}
			else
			{
				l_PaperESFDate.Enabled = false;
				l_PaperESFReason.Enabled = false;
				combxPartA_PaperESFReason.Enabled = false;
				combxPartA_PaperESFReason.Text = "";
				dtpPartA_PaperESFDate.Value = DateTime.Now;
				dtpPartA_PaperESFDate.Enabled = false;
				tbPartA_PaperESFDate.Enabled = false;
				tbPartA_PaperESFDate.Text = "";
				epPartA_PaperESFDate.Clear();
				epPartA_PaperESFReason.Clear();
			}
		}

		private void combxPartA_PaperESFReason_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (combxPartA_PaperESFReason.Text)
			{
				case DOWN_TIME:
				case UNLAWFUL_REMOVAL_REGISTRATIONE:
				case MISSING_REQUIREMENT:
					dtpPartA_PaperESFDate.Enabled = true;
					CombxPartA_PaperESFReason_Validating(combxPartA_PaperESFReason, null);
					break;
				default:
					dtpPartA_PaperESFDate.Enabled = false;
					break;
			}
		}

		private void dtpPartA_TurnoverDate_ValueChanged(object sender, EventArgs e)
		{
			dtpPartA_TurnoverDate.MaxDate = DateTime.Now;
			dtpPartA_TurnoverDate.MinDate = DateTime.Now.AddYears(-5);
		}

		private void chbxPartA_isCorrectedESF_CheckedChanged(object sender, EventArgs e)
		{
			if (this.chbxPartA_isCorrectedESF.Checked)
			{
				chbxPartA_isAddedESF.Checked = false;
				l_CorrectedESFDate.Enabled = true;
				dtpPartA_CorrectedESFDate.Enabled = true;
				l_CorrectedESFAccSysNum.Enabled = true;
				tbPartA_CorrectedESFAccSysNum.Enabled = true;
				l_CorrectedESFNum.Enabled = true;
				tbPartA_CorrectedESFNum.Enabled = true;
				tbPartA_CorrectedESFDate.Enabled = true;
				FillCorrectedESFData();				
			}
			else
			{
				tbPartA_CorrectedESFDate.Enabled = false;
				l_CorrectedESFDate.Enabled = false;
				dtpPartA_CorrectedESFDate.Enabled = false;
				l_CorrectedESFAccSysNum.Enabled = false;
				tbPartA_CorrectedESFAccSysNum.Enabled = false;
				l_CorrectedESFNum.Enabled = false;
				tbPartA_CorrectedESFNum.Enabled = false;
				ClearCorrectedESFData();
			}
		}


		internal DateTime getInvoiceDatePaper()
		{
			return tbPartA_PaperESFDate.Text == ""? new DateTime(): dtpPartA_PaperESFDate.Value;
		}

		internal bool setInvoiceDatePaper(DateTime date)
		{
			try
			{
				dtpPartA_PaperESFDate.Value = date;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private void ClearCorrectedESFData()
		{
			dtpPartA_CorrectedESFDate.Value = DateTime.Now;
			tbPartA_CorrectedESFAccSysNum.Text = "";
			tbPartA_CorrectedESFNum.Text = "";
			tbPartA_CorrectedESFDate.Text = "";
		}

		private void FillCorrectedESFData()
		{
			
		}

		private void chbxPartA_isAddedESF_CheckedChanged(object sender, EventArgs e)
		{
			if (this.chbxPartA_isAddedESF.Checked)
			{
				chbxPartA_isCorrectedESF.Checked = false;
				l_AddedESFDate.Enabled = true;
				l_AddedESFAccSysNum.Enabled = true;
				l_AddedESFNum.Enabled = true;
				dtpPartA_AddedESFDate.Enabled = true;				
				tbPartA_AddedESFAccSysNum.Enabled = true;				
				tbPartA_AddedESFNum.Enabled = true;
				tbPartA_AddedESFDate.Enabled = true;
				
				FillAddedESFData();
			}
			else
			{
				l_AddedESFDate.Enabled = false;
				l_AddedESFAccSysNum.Enabled = false;
				l_AddedESFNum.Enabled = false;
				dtpPartA_AddedESFDate.Enabled = false;
				tbPartA_AddedESFAccSysNum.Enabled = false;
				tbPartA_AddedESFNum.Enabled = false;
				tbPartA_AddedESFDate.Enabled = true;

				ClearAddedESFData();
			}
		}

		private void ClearAddedESFData()
		{
			dtpPartA_AddedESFDate.Value = DateTime.Now;
			tbPartA_AddedESFAccSysNum.Text = "";
			tbPartA_AddedESFNum.Text = "";
			tbPartA_AddedESFDate.Text = "";
		}

		private void FillAddedESFData()
		{

		}

		private void tbPartA_Num_TextChanged(object sender, EventArgs e)
		{

		}

		internal PaperReasonType getReasonPaper()
		{
			return (PaperReasonType)combxPartA_PaperESFReason.SelectedIndex;
		}

		internal bool setReasonPaper(PaperReasonType reason)
		{
			try
			{
				if (reason != null)
				{
					chbxPartA_isPaperESF.Checked = true;
				}
				combxPartA_PaperESFReason.SelectedItem = combxPartA_PaperESFReason.Items[(int)reason];
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getInvoiceNum()
		{
			return tbPartA_Num.Text;
		}

		internal bool setInvoiceNum(string num)
		{
			try
			{
				tbPartA_Num.Text = num;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal DateTime getInvoiceTurnoverDate()
		{
			return  tbPartA_TurnoverDate.Text == ""? new DateTime(): dtpPartA_TurnoverDate.Value;
		}

		internal void clearReasonPaper()
		{
			combxPartA_PaperESFReason.SelectedItem = null;
		}

		private void panelESFpartA_SizeChanged(object sender, EventArgs e)
		{
			this.Visible = true;
		}

		private void panelESFpartA_Resize(object sender, EventArgs e)
		{
			this.Visible = false;
		}

		internal DateTime getInvoiceDate()
		{
			return tbPartA_Date.Text == "" ? new DateTime():this.dtpPartA_Date.Value;
		}

		internal bool setInvoiceDate(DateTime date)
		{
			try
			{
				this.dtpPartA_Date.Value = date;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal InvoiceType getInvoiceType()
		{
			if (chbxPartA_isAddedESF.Checked)
			{
				return InvoiceType.ADDITIONAL_INVOICE;
			}
			else if (chbxPartA_isCorrectedESF.Checked)
			{
				return InvoiceType.FIXED_INVOICE;
			}
			else
			{
				return InvoiceType.ORDINARY_INVOICE;
			}
		}

		private void dtpPartA_PaperESFDate_ValueChanged(object sender, EventArgs e)
		{
			bool flag = false;
			string alertMsg = "";

			bool isLargeCompany = false;//??????
			bool isNDSPayer = false;//????			
			bool isOlderThanFiveYears = DateTime.Now > dtpPartA_PaperESFDate.Value.AddYears(5);
			bool isOlderThan_2018_01_01 = dtpPartA_PaperESFDate.Value < new DateTime(2018, 1, 1);
			bool isOlderThan_2019_01_01 = dtpPartA_PaperESFDate.Value < new DateTime(2018, 1, 1);
			bool isChosenMissingRequirement = combxPartA_PaperESFReason.Text == MISSING_REQUIREMENT; ;



			if (isChosenMissingRequirement)
			{
				if (isOlderThanFiveYears)
				{
					alertMsg = "старше 5 лет";
				}
				else
				{
					//1. Если дата бумажного СФ, на который	необходимо выписать исправленный и дополнительный ЭСФ, является до 01.01.2018 года(но не больше 5 лет с текущей даты), то проверки осуществлять не нужно.
					if (isOlderThan_2018_01_01)
					{
						flag = true;
					}
					//2. Если дата бумажного СФ, на который необходимо выписать исправленный и дополнительный ЭСФ, равна 01.01.2018 года и позже, то нужно проверить входит ли Пользователь в список крупных компаний.Если пользователь входит в 
					//список крупных компаний, то выходит сообщение: "Вы не можете вводить бумажный СФ с причиной "Отсутствовало требование по выписке ЭСФ".
					else if (!isOlderThan_2018_01_01 && isLargeCompany)
					{
						alertMsg = "Вы не можете вводить бумажный СФ с причиной \"Отсутствовало требование по выписке ЭСФ\"";
					}
					//3.Если дата бумажного СФ, на который необходимо выписать исправленный и дополнительный ЭСФ, равна 01.01.2019 года и позже(но не больше 5 лет с текущей даты), то необходимо проверить
					//является ли Пользователь плательщиком НДС на дату выписки бумажного счетафактуры.При ошибке сообщение: "Вы не можете вводить бумажный СФ с причиной "Отсутствовало требование повыписке ЭСФ"".
					else if (!isOlderThan_2019_01_01 && !isNDSPayer)
					{
						alertMsg = "Вы не можете вводить бумажный СФ с причиной \"Отсутствовало требование повыписке ЭСФ\"";
					}
					else
					{
						flag = true;
					}
				}
			}

			if (!flag)
			{
				epPartA_PaperESFDate.SetError(dtpPartA_PaperESFDate, alertMsg);
			}
			else
			{
				epPartA_PaperESFDate.Clear();
			}
		}

		private void dtpPartA_Date_ValueChanged(object sender, EventArgs e)
		{
			tbPartA_Date.Text = dtpPartA_Date.Value.ToString("dd.MM.yyyy");
		}

		private void dtpPartA_PaperESFDate_ValueChanged_1(object sender, EventArgs e)
		{
			tbPartA_PaperESFDate.Text = dtpPartA_PaperESFDate.Value.ToString("dd.MM.yyyy");
		}

		private void dtpPartA_TurnoverDate_ValueChanged_1(object sender, EventArgs e)
		{
			tbPartA_TurnoverDate.Text = dtpPartA_TurnoverDate.Value.ToString("dd.MM.yyyy");
		}

		private void dtpPartA_CorrectedESFDate_ValueChanged(object sender, EventArgs e)
		{
			tbPartA_CorrectedESFDate.Text = dtpPartA_CorrectedESFDate.Value.ToString("dd.MM.yyyy");
		}

		private void tbPartA_AddedESFDate_TextChanged(object sender, EventArgs e)
		{
			tbPartA_AddedESFDate.Text = dtpPartA_AddedESFDate.Value.ToString("dd.MM.yyyy");
		}

		internal bool isPaperESF()
		{
			return chbxPartA_isPaperESF.Checked;
		}
	}
}
