using ESF_kz.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace ESF_kz
{
	public partial class ESF_form : Form
	{
		AbstractUCESFpanel lastShownPanel;
		Button selectedPanel;

		private void setESFformField(AbstractUCESFpanel esf_panel)
		{
			esf_panel.setESFform(this);
		}

		public ESF_form()
		{
			InitializeComponent();
			hideAllPanels();
			showFirstPanel();
			setESFformFieldForPanels();
			FormManagerFacade.setInvoiceForm(this);
		}

		private void hideAllPanels()
		{
			this.PanelESFpartA.Visible = false;
			this.PanelESFpartB.Visible = false;
			this.PanelESFpartC.Visible = false;
			this.PanelESFpartD.Visible = false;
			this.PanelESFpartE.Visible = false;
			this.PanelESFpartF.Visible = false;
			this.PanelESFpartG.Visible = false;
			this.PanelESFpartH.Visible = false;
			this.PanelESFpartI.Visible = false;
			this.PanelESFpartJ.Visible = false;
			this.PanelESFpartK.Visible = false;
			this.PanelESFpartL.Visible = false;
		}

		private void showFirstPanel()
		{
			if (lastShownPanel != null)
				lastShownPanel.Visible = false;
			this.PanelESFpartA.Visible = true;
			colorSelectedButton(btnESFpartA);
			lastShownPanel = this.PanelESFpartA;
		}
		
		private void colorSelectedButton(Button btn)
		{
			if (selectedPanel != null)
			{
				selectedPanel.BackColor = Color.FromArgb(240);
			}
			selectedPanel = btn;
			selectedPanel.BackColor = Color.SkyBlue;
		}

		private void setESFformFieldForPanels()
		{
			foreach (AbstractUCESFpanel item in this.splitContainer1.Panel2.Controls)
			{
				item.setESFform(this);				
			}
		}

		private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
		{

		}

		private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
		{

		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{

		}

		private void btnESFpartA_Click(object sender, EventArgs e)
		{
			lastShownPanel.Hide();
			colorSelectedButton(btnESFpartA);
			this.PanelESFpartA.Show();
			this.PanelESFpartA.BringToFront();
			lastShownPanel = PanelESFpartA;
		}

		private void btnESFpartB_Click(object sender, EventArgs e)
		{
			lastShownPanel.Hide();
			colorSelectedButton(btnESFpartB);
			this.PanelESFpartB.Show();
			this.PanelESFpartB.BringToFront();
			lastShownPanel = PanelESFpartB;
		}

		private void btnESFpartC_Click(object sender, EventArgs e)
		{
			lastShownPanel.Hide();
			colorSelectedButton(btnESFpartC);
			this.PanelESFpartC.Show();
			this.PanelESFpartC.BringToFront();
			lastShownPanel = PanelESFpartC;
		}

		private void btnESFpartD_Click(object sender, EventArgs e)
		{
			lastShownPanel.Hide();
			colorSelectedButton(btnESFpartD);
			this.PanelESFpartD.Show();
			this.PanelESFpartD.BringToFront();
			lastShownPanel = PanelESFpartD;
		}

		private void btnESFpartE_Click(object sender, EventArgs e)
		{
			lastShownPanel.Hide();
			colorSelectedButton(btnESFpartE);
			this.PanelESFpartE.Show();
			this.PanelESFpartE.BringToFront();
			lastShownPanel = PanelESFpartE;
		}

		private void btnESFpartF_Click(object sender, EventArgs e)
		{
			lastShownPanel.Hide();
			colorSelectedButton(btnESFpartF);
			this.PanelESFpartF.Show();
			this.PanelESFpartF.BringToFront();
			lastShownPanel = PanelESFpartF;
		}

		private void btnESFpartG_Click(object sender, EventArgs e)
		{
			lastShownPanel.Hide();
			colorSelectedButton(btnESFpartG);
			this.PanelESFpartG.Show();
			this.PanelESFpartG.BringToFront();
			lastShownPanel = PanelESFpartG;
		}

		private void btnESFpartH_Click(object sender, EventArgs e)
		{
			lastShownPanel.Hide();
			colorSelectedButton(btnESFpartH);
			this.PanelESFpartH.Show();
			this.PanelESFpartH.BringToFront();
			lastShownPanel = PanelESFpartH;
		}

		private void btnESFpartI_Click(object sender, EventArgs e)
		{
			lastShownPanel.Hide();
			colorSelectedButton(btnESFpartI);
			this.PanelESFpartI.Show();
			this.PanelESFpartI.BringToFront();
			lastShownPanel = PanelESFpartI;
		}

		private void btnESFpartJ_Click(object sender, EventArgs e)
		{
			lastShownPanel.Hide();
			colorSelectedButton(btnESFpartJ);
			this.PanelESFpartJ.Show();
			this.PanelESFpartJ.BringToFront();
			lastShownPanel = PanelESFpartJ;
		}

		private void btnESFpartK_Click(object sender, EventArgs e)
		{
			lastShownPanel.Hide();
			colorSelectedButton(btnESFpartK);
			this.PanelESFpartK.Show();
			this.PanelESFpartK.BringToFront();
			lastShownPanel = PanelESFpartK;
		}

		private void btnESFpartL_Click(object sender, EventArgs e)
		{
			lastShownPanel.Hide();
			colorSelectedButton(btnESFpartL);
			this.PanelESFpartL.Show();
			this.PanelESFpartL.BringToFront();
			lastShownPanel = PanelESFpartL;
		}


		public T getPannel<T>()
		{
			T result = default(T);
			foreach (var item in this.splitContainer1.Panel2.Controls)
			{
				Type first = item.GetType();
				Type second = typeof(T);
				if (first.Name == second.Name)
					result = (T)item;
			}
			return result;
		}

		internal void highlightCustomerBtn()
		{
			btnESFpartC.BackColor = Color.FromArgb(134, 250, 131); 
		}

		internal void highlightSellerBtn()
		{
			btnESFpartB.BackColor = Color.FromArgb(134, 250, 131);
		}

		internal void highlightProductBtn()
		{
			btnESFpartG.BackColor = Color.FromArgb( 134, 250, 131);
		}

		internal void highlightDeliveryBtn()
		{
			btnESFpartE.BackColor = Color.FromArgb(134, 250, 131);
		}

		internal void SetEnableBtnESFpartI(bool state)
		{
			btnESFpartI.Enabled = state;
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
				return;
			// получаем выбранный файл
			string filename = openFileDialog1.FileName;
			// читаем файл в строку
			//string fileText = System.IO.File.ReadAllText(filename);
			XmlDocument xDoc = new XmlDocument();
			xDoc.Load(filename);
			invoiceContainerV2 ic = SessionDataManagerFacade.ParseInvoiceXML(xDoc);
			FormManagerFacade.fillInvoiceForm(ic);
			MessageBox.Show("Файл открыт");
		}

		private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FormManagerFacade.clearAllTabs();
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SessionDataManagerFacade.getInvoiceBodyString();
		}



		private void ESF_form_ResizeBegin(object sender, EventArgs e)
		{

		}

		private void ESF_form_ResizeEnd(object sender, EventArgs e)
		{
			
		}

		private void ESF_form_SizeChanged(object sender, EventArgs e)
		{
			
		}

		internal void SetEnableBtnESFPartH(bool v)
		{
			btnESFpartH.Enabled = v;
		}

		internal void SetEnableBtnESFpartJ(bool v)
		{
			btnESFpartJ.Enabled = v;
		}

		private void sendToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FormManagerFacade.setInvoiceDate(DateTime.Now);
			LocalServiceOperationFacade.GenerateInvoiceSignature();
			UploadInvoiceServiceOperationFacade.SendInvoice();
		}
	}
}
