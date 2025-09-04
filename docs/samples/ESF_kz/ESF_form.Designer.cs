using ESF_kz.Forms;
using System.Windows.Forms;

namespace ESF_kz
{
	partial class ESF_form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.btnESFpartL = new System.Windows.Forms.Button();
			this.btnESFpartK = new System.Windows.Forms.Button();
			this.btnESFpartJ = new System.Windows.Forms.Button();
			this.btnESFpartI = new System.Windows.Forms.Button();
			this.btnESFpartH = new System.Windows.Forms.Button();
			this.btnESFpartG = new System.Windows.Forms.Button();
			this.btnESFpartF = new System.Windows.Forms.Button();
			this.btnESFpartE = new System.Windows.Forms.Button();
			this.btnESFpartD = new System.Windows.Forms.Button();
			this.btnESFpartC = new System.Windows.Forms.Button();
			this.btnESFpartA = new System.Windows.Forms.Button();
			this.btnESFpartB = new System.Windows.Forms.Button();
			this.PanelESFpartA = new ESF_kz.Forms.panelESFpartA();
			this.PanelESFpartB = new ESF_kz.Forms.panelESFpartB();
			this.PanelESFpartC = new ESF_kz.Forms.panelESFpartC();
			this.PanelESFpartD = new ESF_kz.Forms.panelESFpartD();
			this.PanelESFpartE = new ESF_kz.Forms.panelESFpartE();
			this.PanelESFpartF = new ESF_kz.Forms.panelESFpartF();
			this.PanelESFpartG = new ESF_kz.Forms.panelESFpartG();
			this.PanelESFpartH = new ESF_kz.Forms.panelESFpartH();
			this.PanelESFpartI = new ESF_kz.Forms.panelESFpartI();
			this.PanelESFpartJ = new ESF_kz.Forms.panelESFpartJ();
			this.PanelESFpartK = new ESF_kz.Forms.panelESFpartK();
			this.PanelESFpartL = new ESF_kz.Forms.panelESFpartL();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.invoiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.sendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.clearAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.panel1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.902598F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90.0974F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1073, 648);
			this.tableLayoutPanel1.TabIndex = 4;
			this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(3, 67);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel2);
			this.splitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.PanelESFpartA);
			this.splitContainer1.Panel2.Controls.Add(this.PanelESFpartB);
			this.splitContainer1.Panel2.Controls.Add(this.PanelESFpartC);
			this.splitContainer1.Panel2.Controls.Add(this.PanelESFpartD);
			this.splitContainer1.Panel2.Controls.Add(this.PanelESFpartE);
			this.splitContainer1.Panel2.Controls.Add(this.PanelESFpartF);
			this.splitContainer1.Panel2.Controls.Add(this.PanelESFpartG);
			this.splitContainer1.Panel2.Controls.Add(this.PanelESFpartH);
			this.splitContainer1.Panel2.Controls.Add(this.PanelESFpartI);
			this.splitContainer1.Panel2.Controls.Add(this.PanelESFpartJ);
			this.splitContainer1.Panel2.Controls.Add(this.PanelESFpartK);
			this.splitContainer1.Panel2.Controls.Add(this.PanelESFpartL);
			this.splitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.splitContainer1.Size = new System.Drawing.Size(1067, 578);
			this.splitContainer1.SplitterDistance = 256;
			this.splitContainer1.TabIndex = 5;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 1;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Controls.Add(this.btnESFpartL, 0, 11);
			this.tableLayoutPanel2.Controls.Add(this.btnESFpartK, 0, 10);
			this.tableLayoutPanel2.Controls.Add(this.btnESFpartJ, 0, 9);
			this.tableLayoutPanel2.Controls.Add(this.btnESFpartI, 0, 8);
			this.tableLayoutPanel2.Controls.Add(this.btnESFpartH, 0, 7);
			this.tableLayoutPanel2.Controls.Add(this.btnESFpartG, 0, 6);
			this.tableLayoutPanel2.Controls.Add(this.btnESFpartF, 0, 5);
			this.tableLayoutPanel2.Controls.Add(this.btnESFpartE, 0, 4);
			this.tableLayoutPanel2.Controls.Add(this.btnESFpartD, 0, 3);
			this.tableLayoutPanel2.Controls.Add(this.btnESFpartC, 0, 2);
			this.tableLayoutPanel2.Controls.Add(this.btnESFpartA, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.btnESFpartB, 0, 1);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 12;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(256, 578);
			this.tableLayoutPanel2.TabIndex = 2;
			// 
			// btnESFpartL
			// 
			this.btnESFpartL.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnESFpartL.Location = new System.Drawing.Point(3, 531);
			this.btnESFpartL.Name = "btnESFpartL";
			this.btnESFpartL.Size = new System.Drawing.Size(250, 44);
			this.btnESFpartL.TabIndex = 11;
			this.btnESFpartL.Text = "Раздел L. Сведения по ЭЦП";
			this.btnESFpartL.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnESFpartL.UseVisualStyleBackColor = true;
			this.btnESFpartL.Click += new System.EventHandler(this.btnESFpartL_Click);
			// 
			// btnESFpartK
			// 
			this.btnESFpartK.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnESFpartK.Location = new System.Drawing.Point(3, 483);
			this.btnESFpartK.Name = "btnESFpartK";
			this.btnESFpartK.Size = new System.Drawing.Size(250, 42);
			this.btnESFpartK.TabIndex = 10;
			this.btnESFpartK.Text = "Раздел K. Дополнительные сведения";
			this.btnESFpartK.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnESFpartK.UseVisualStyleBackColor = true;
			this.btnESFpartK.Click += new System.EventHandler(this.btnESFpartK_Click);
			// 
			// btnESFpartJ
			// 
			this.btnESFpartJ.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnESFpartJ.Enabled = false;
			this.btnESFpartJ.Location = new System.Drawing.Point(3, 435);
			this.btnESFpartJ.Name = "btnESFpartJ";
			this.btnESFpartJ.Size = new System.Drawing.Size(250, 42);
			this.btnESFpartJ.TabIndex = 9;
			this.btnESFpartJ.Text = "Раздел J. Реквизиты поверенного (оператора) получателя";
			this.btnESFpartJ.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnESFpartJ.UseVisualStyleBackColor = true;
			this.btnESFpartJ.Click += new System.EventHandler(this.btnESFpartJ_Click);
			// 
			// btnESFpartI
			// 
			this.btnESFpartI.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnESFpartI.Enabled = false;
			this.btnESFpartI.Location = new System.Drawing.Point(3, 387);
			this.btnESFpartI.Name = "btnESFpartI";
			this.btnESFpartI.Size = new System.Drawing.Size(250, 42);
			this.btnESFpartI.TabIndex = 8;
			this.btnESFpartI.Text = "Раздел I. Реквизиты поверенного (оператора) поставщика";
			this.btnESFpartI.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnESFpartI.UseVisualStyleBackColor = true;
			this.btnESFpartI.Click += new System.EventHandler(this.btnESFpartI_Click);
			// 
			// btnESFpartH
			// 
			this.btnESFpartH.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnESFpartH.Enabled = false;
			this.btnESFpartH.Location = new System.Drawing.Point(3, 339);
			this.btnESFpartH.Name = "btnESFpartH";
			this.btnESFpartH.Size = new System.Drawing.Size(250, 42);
			this.btnESFpartH.TabIndex = 7;
			this.btnESFpartH.Text = "Раздел H. Данные по товарам, работам, услугам участников совместной деятельности";
			this.btnESFpartH.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnESFpartH.UseVisualStyleBackColor = true;
			this.btnESFpartH.Click += new System.EventHandler(this.btnESFpartH_Click);
			// 
			// btnESFpartG
			// 
			this.btnESFpartG.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnESFpartG.Location = new System.Drawing.Point(3, 291);
			this.btnESFpartG.Name = "btnESFpartG";
			this.btnESFpartG.Size = new System.Drawing.Size(250, 42);
			this.btnESFpartG.TabIndex = 6;
			this.btnESFpartG.Text = "Раздел G. Данные по товарам, работам, услугам";
			this.btnESFpartG.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnESFpartG.UseVisualStyleBackColor = true;
			this.btnESFpartG.Click += new System.EventHandler(this.btnESFpartG_Click);
			// 
			// btnESFpartF
			// 
			this.btnESFpartF.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnESFpartF.Location = new System.Drawing.Point(3, 243);
			this.btnESFpartF.Name = "btnESFpartF";
			this.btnESFpartF.Size = new System.Drawing.Size(250, 42);
			this.btnESFpartF.TabIndex = 5;
			this.btnESFpartF.Text = "Раздел F. Реквизиты документов, подтверждающих поставку товаров, работ, услуг";
			this.btnESFpartF.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnESFpartF.UseVisualStyleBackColor = true;
			this.btnESFpartF.Click += new System.EventHandler(this.btnESFpartF_Click);
			// 
			// btnESFpartE
			// 
			this.btnESFpartE.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnESFpartE.Location = new System.Drawing.Point(3, 195);
			this.btnESFpartE.Name = "btnESFpartE";
			this.btnESFpartE.Size = new System.Drawing.Size(250, 42);
			this.btnESFpartE.TabIndex = 4;
			this.btnESFpartE.Text = "Раздел Е. Договор (контракт)";
			this.btnESFpartE.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnESFpartE.UseVisualStyleBackColor = true;
			this.btnESFpartE.Click += new System.EventHandler(this.btnESFpartE_Click);
			// 
			// btnESFpartD
			// 
			this.btnESFpartD.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnESFpartD.Location = new System.Drawing.Point(3, 147);
			this.btnESFpartD.Name = "btnESFpartD";
			this.btnESFpartD.Size = new System.Drawing.Size(250, 42);
			this.btnESFpartD.TabIndex = 3;
			this.btnESFpartD.Text = "Раздел D. Реквизиты грузоотправителя и грузополучателя";
			this.btnESFpartD.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnESFpartD.UseVisualStyleBackColor = true;
			this.btnESFpartD.Click += new System.EventHandler(this.btnESFpartD_Click);
			// 
			// btnESFpartC
			// 
			this.btnESFpartC.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnESFpartC.Location = new System.Drawing.Point(3, 99);
			this.btnESFpartC.Name = "btnESFpartC";
			this.btnESFpartC.Size = new System.Drawing.Size(250, 42);
			this.btnESFpartC.TabIndex = 2;
			this.btnESFpartC.Text = "Раздел С. Реквизиты получателя";
			this.btnESFpartC.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnESFpartC.UseVisualStyleBackColor = true;
			this.btnESFpartC.Click += new System.EventHandler(this.btnESFpartC_Click);
			// 
			// btnESFpartA
			// 
			this.btnESFpartA.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnESFpartA.Location = new System.Drawing.Point(3, 3);
			this.btnESFpartA.Name = "btnESFpartA";
			this.btnESFpartA.Size = new System.Drawing.Size(250, 42);
			this.btnESFpartA.TabIndex = 0;
			this.btnESFpartA.Text = "Раздел А. Общий раздел";
			this.btnESFpartA.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnESFpartA.UseVisualStyleBackColor = true;
			this.btnESFpartA.Click += new System.EventHandler(this.btnESFpartA_Click);
			// 
			// btnESFpartB
			// 
			this.btnESFpartB.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnESFpartB.Location = new System.Drawing.Point(3, 51);
			this.btnESFpartB.Name = "btnESFpartB";
			this.btnESFpartB.Size = new System.Drawing.Size(250, 42);
			this.btnESFpartB.TabIndex = 1;
			this.btnESFpartB.Text = "Раздел В. Реквизиты поставщика";
			this.btnESFpartB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnESFpartB.UseVisualStyleBackColor = true;
			this.btnESFpartB.Click += new System.EventHandler(this.btnESFpartB_Click);
			// 
			// PanelESFpartA
			// 
			this.PanelESFpartA.AutoSize = true;
			this.PanelESFpartA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.PanelESFpartA.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PanelESFpartA.Location = new System.Drawing.Point(0, 0);
			this.PanelESFpartA.Margin = new System.Windows.Forms.Padding(10);
			this.PanelESFpartA.Name = "PanelESFpartA";
			this.PanelESFpartA.Padding = new System.Windows.Forms.Padding(10);
			this.PanelESFpartA.Size = new System.Drawing.Size(807, 578);
			this.PanelESFpartA.TabIndex = 0;
			// 
			// PanelESFpartB
			// 
			this.PanelESFpartB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.PanelESFpartB.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PanelESFpartB.Location = new System.Drawing.Point(0, 0);
			this.PanelESFpartB.Margin = new System.Windows.Forms.Padding(10);
			this.PanelESFpartB.Name = "PanelESFpartB";
			this.PanelESFpartB.Padding = new System.Windows.Forms.Padding(10);
			this.PanelESFpartB.Size = new System.Drawing.Size(807, 578);
			this.PanelESFpartB.TabIndex = 1;
			// 
			// PanelESFpartC
			// 
			this.PanelESFpartC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.PanelESFpartC.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PanelESFpartC.Location = new System.Drawing.Point(0, 0);
			this.PanelESFpartC.Margin = new System.Windows.Forms.Padding(10);
			this.PanelESFpartC.Name = "PanelESFpartC";
			this.PanelESFpartC.Padding = new System.Windows.Forms.Padding(10);
			this.PanelESFpartC.Size = new System.Drawing.Size(807, 578);
			this.PanelESFpartC.TabIndex = 2;
			// 
			// PanelESFpartD
			// 
			this.PanelESFpartD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.PanelESFpartD.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PanelESFpartD.Location = new System.Drawing.Point(0, 0);
			this.PanelESFpartD.Margin = new System.Windows.Forms.Padding(10);
			this.PanelESFpartD.Name = "PanelESFpartD";
			this.PanelESFpartD.Padding = new System.Windows.Forms.Padding(10);
			this.PanelESFpartD.Size = new System.Drawing.Size(807, 578);
			this.PanelESFpartD.TabIndex = 3;
			// 
			// PanelESFpartE
			// 
			this.PanelESFpartE.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.PanelESFpartE.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PanelESFpartE.Location = new System.Drawing.Point(0, 0);
			this.PanelESFpartE.Margin = new System.Windows.Forms.Padding(10);
			this.PanelESFpartE.Name = "PanelESFpartE";
			this.PanelESFpartE.Padding = new System.Windows.Forms.Padding(10);
			this.PanelESFpartE.Size = new System.Drawing.Size(807, 578);
			this.PanelESFpartE.TabIndex = 4;
			// 
			// PanelESFpartF
			// 
			this.PanelESFpartF.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.PanelESFpartF.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PanelESFpartF.Location = new System.Drawing.Point(0, 0);
			this.PanelESFpartF.Margin = new System.Windows.Forms.Padding(10);
			this.PanelESFpartF.Name = "PanelESFpartF";
			this.PanelESFpartF.Padding = new System.Windows.Forms.Padding(10);
			this.PanelESFpartF.Size = new System.Drawing.Size(807, 578);
			this.PanelESFpartF.TabIndex = 5;
			// 
			// PanelESFpartG
			// 
			this.PanelESFpartG.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.PanelESFpartG.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PanelESFpartG.Location = new System.Drawing.Point(0, 0);
			this.PanelESFpartG.Margin = new System.Windows.Forms.Padding(10);
			this.PanelESFpartG.Name = "PanelESFpartG";
			this.PanelESFpartG.Padding = new System.Windows.Forms.Padding(10);
			this.PanelESFpartG.Size = new System.Drawing.Size(807, 578);
			this.PanelESFpartG.TabIndex = 6;
			// 
			// PanelESFpartH
			// 
			this.PanelESFpartH.AutoSize = true;
			this.PanelESFpartH.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.PanelESFpartH.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PanelESFpartH.Location = new System.Drawing.Point(0, 0);
			this.PanelESFpartH.Margin = new System.Windows.Forms.Padding(10);
			this.PanelESFpartH.Name = "PanelESFpartH";
			this.PanelESFpartH.Padding = new System.Windows.Forms.Padding(10);
			this.PanelESFpartH.Size = new System.Drawing.Size(807, 578);
			this.PanelESFpartH.TabIndex = 7;
			// 
			// PanelESFpartI
			// 
			this.PanelESFpartI.AutoSize = true;
			this.PanelESFpartI.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.PanelESFpartI.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PanelESFpartI.Location = new System.Drawing.Point(0, 0);
			this.PanelESFpartI.Margin = new System.Windows.Forms.Padding(10);
			this.PanelESFpartI.Name = "PanelESFpartI";
			this.PanelESFpartI.Padding = new System.Windows.Forms.Padding(10);
			this.PanelESFpartI.Size = new System.Drawing.Size(807, 578);
			this.PanelESFpartI.TabIndex = 8;
			// 
			// PanelESFpartJ
			// 
			this.PanelESFpartJ.AutoSize = true;
			this.PanelESFpartJ.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.PanelESFpartJ.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PanelESFpartJ.Location = new System.Drawing.Point(0, 0);
			this.PanelESFpartJ.Margin = new System.Windows.Forms.Padding(10);
			this.PanelESFpartJ.Name = "PanelESFpartJ";
			this.PanelESFpartJ.Padding = new System.Windows.Forms.Padding(10);
			this.PanelESFpartJ.Size = new System.Drawing.Size(807, 578);
			this.PanelESFpartJ.TabIndex = 9;
			// 
			// PanelESFpartK
			// 
			this.PanelESFpartK.AutoSize = true;
			this.PanelESFpartK.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.PanelESFpartK.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PanelESFpartK.Location = new System.Drawing.Point(0, 0);
			this.PanelESFpartK.Margin = new System.Windows.Forms.Padding(10);
			this.PanelESFpartK.Name = "PanelESFpartK";
			this.PanelESFpartK.Padding = new System.Windows.Forms.Padding(10);
			this.PanelESFpartK.Size = new System.Drawing.Size(807, 578);
			this.PanelESFpartK.TabIndex = 10;
			// 
			// PanelESFpartL
			// 
			this.PanelESFpartL.AutoSize = true;
			this.PanelESFpartL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.PanelESFpartL.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PanelESFpartL.Location = new System.Drawing.Point(0, 0);
			this.PanelESFpartL.Margin = new System.Windows.Forms.Padding(10);
			this.PanelESFpartL.Name = "PanelESFpartL";
			this.PanelESFpartL.Padding = new System.Windows.Forms.Padding(10);
			this.PanelESFpartL.Size = new System.Drawing.Size(807, 578);
			this.PanelESFpartL.TabIndex = 11;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.menuStrip1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(3, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1067, 58);
			this.panel1.TabIndex = 6;
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.Location = new System.Drawing.Point(0, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(1067, 34);
			this.label1.TabIndex = 0;
			this.label1.Text = "ЭЛЕКТРОННЫЙ СЧЕТ ФАКТУРА";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.invoiceToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(1067, 24);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// invoiceToolStripMenuItem
			// 
			this.invoiceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.sendToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.clearAllToolStripMenuItem});
			this.invoiceToolStripMenuItem.Name = "invoiceToolStripMenuItem";
			this.invoiceToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
			this.invoiceToolStripMenuItem.Text = "invoice";
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.openToolStripMenuItem.Text = "open";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// loadToolStripMenuItem
			// 
			this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
			this.loadToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.loadToolStripMenuItem.Text = "load";
			// 
			// sendToolStripMenuItem
			// 
			this.sendToolStripMenuItem.Name = "sendToolStripMenuItem";
			this.sendToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.sendToolStripMenuItem.Text = "send";
			this.sendToolStripMenuItem.Click += new System.EventHandler(this.sendToolStripMenuItem_Click);
			// 
			// saveAsToolStripMenuItem
			// 
			this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.saveAsToolStripMenuItem.Text = "save as";
			this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
			// 
			// clearAllToolStripMenuItem
			// 
			this.clearAllToolStripMenuItem.Name = "clearAllToolStripMenuItem";
			this.clearAllToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.clearAllToolStripMenuItem.Text = "clear All";
			this.clearAllToolStripMenuItem.Click += new System.EventHandler(this.clearAllToolStripMenuItem_Click);
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			this.openFileDialog1.Filter = "Xml files (*.xml)|*.xml";
			// 
			// ESF_form
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1073, 648);
			this.Controls.Add(this.tableLayoutPanel1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "ESF_form";
			this.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Text = "ESF_form";
			this.ResizeBegin += new System.EventHandler(this.ESF_form_ResizeBegin);
			this.ResizeEnd += new System.EventHandler(this.ESF_form_ResizeEnd);
			this.SizeChanged += new System.EventHandler(this.ESF_form_SizeChanged);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Button btnESFpartL;
		private System.Windows.Forms.Button btnESFpartK;
		private System.Windows.Forms.Button btnESFpartJ;
		private System.Windows.Forms.Button btnESFpartI;
		private System.Windows.Forms.Button btnESFpartH;
		private System.Windows.Forms.Button btnESFpartG;
		private System.Windows.Forms.Button btnESFpartF;
		private System.Windows.Forms.Button btnESFpartE;
		private System.Windows.Forms.Button btnESFpartD;
		private System.Windows.Forms.Button btnESFpartC;
		private System.Windows.Forms.Button btnESFpartA;
		private System.Windows.Forms.Button btnESFpartB;
		private panelESFpartA PanelESFpartA;
		private panelESFpartB PanelESFpartB;
		private panelESFpartC PanelESFpartC;
		private panelESFpartD PanelESFpartD;
		private panelESFpartE PanelESFpartE;
		private panelESFpartF PanelESFpartF;
		private panelESFpartG PanelESFpartG;
		private panelESFpartH PanelESFpartH;
		private panelESFpartI PanelESFpartI;
		private panelESFpartJ PanelESFpartJ;
		private panelESFpartK PanelESFpartK;
		private panelESFpartL PanelESFpartL;
		private MenuStrip menuStrip1;
		private ToolStripMenuItem invoiceToolStripMenuItem;
		private ToolStripMenuItem openToolStripMenuItem;
		private ToolStripMenuItem loadToolStripMenuItem;
		private ToolStripMenuItem sendToolStripMenuItem;
		private ToolStripMenuItem saveAsToolStripMenuItem;
		private OpenFileDialog openFileDialog1;
		private SaveFileDialog saveFileDialog1;
		private ToolStripMenuItem clearAllToolStripMenuItem;
	}
}