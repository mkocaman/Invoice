namespace ESF_kz.Forms
{
	partial class MainForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton6 = new System.Windows.Forms.ToolStripButton();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.rowNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.invoiceType = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.invoiceNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.date = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.invoiceStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.direction = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.info = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.toolStripButton7 = new System.Windows.Forms.ToolStripButton();
			this.toolStrip1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton3,
            this.toolStripButton4,
            this.toolStripButton5,
            this.toolStripButton2,
            this.toolStripButton6,
            this.toolStripButton7});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(800, 25);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			this.toolStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStrip1_ItemClicked);
			// 
			// toolStripButton1
			// 
			this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
			this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton1.Name = "toolStripButton1";
			this.toolStripButton1.Size = new System.Drawing.Size(90, 22);
			this.toolStripButton1.Text = "StartSession";
			this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
			// 
			// toolStripButton3
			// 
			this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
			this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton3.Name = "toolStripButton3";
			this.toolStripButton3.Size = new System.Drawing.Size(90, 22);
			this.toolStripButton3.Text = "CurrentUser";
			this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
			// 
			// toolStripButton4
			// 
			this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
			this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton4.Name = "toolStripButton4";
			this.toolStripButton4.Size = new System.Drawing.Size(138, 22);
			this.toolStripButton4.Text = "CurrentSessionStatus";
			this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton4_Click);
			// 
			// toolStripButton5
			// 
			this.toolStripButton5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton5.Image")));
			this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton5.Name = "toolStripButton5";
			this.toolStripButton5.Size = new System.Drawing.Size(95, 22);
			this.toolStripButton5.Text = "CloseSession";
			this.toolStripButton5.Click += new System.EventHandler(this.toolStripButton5_Click);
			// 
			// toolStripButton2
			// 
			this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
			this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton2.Name = "toolStripButton2";
			this.toolStripButton2.Size = new System.Drawing.Size(102, 22);
			this.toolStripButton2.Text = "QueryUpdates";
			this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
			// 
			// toolStripButton6
			// 
			this.toolStripButton6.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton6.Image")));
			this.toolStripButton6.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton6.Name = "toolStripButton6";
			this.toolStripButton6.Size = new System.Drawing.Size(99, 22);
			this.toolStripButton6.Text = "CreateInvoice";
			this.toolStripButton6.Click += new System.EventHandler(this.toolStripButton6_Click);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 3;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.5F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 91.5F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 56F));
			this.tableLayoutPanel1.Controls.Add(this.dataGridView1, 1, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 25);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.86139F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 86.13861F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 425);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.rowNumber,
            this.invoiceType,
            this.invoiceNumber,
            this.date,
            this.invoiceStatus,
            this.direction,
            this.info});
			this.dataGridView1.Location = new System.Drawing.Point(66, 55);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.RowHeadersVisible = false;
			this.dataGridView1.Size = new System.Drawing.Size(674, 321);
			this.dataGridView1.TabIndex = 0;
			this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
			this.dataGridView1.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseClick);
			this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
			// 
			// rowNumber
			// 
			this.rowNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.rowNumber.FillWeight = 5F;
			this.rowNumber.HeaderText = "#";
			this.rowNumber.Name = "rowNumber";
			// 
			// invoiceType
			// 
			this.invoiceType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.invoiceType.FillWeight = 20F;
			this.invoiceType.HeaderText = "InvoiceType";
			this.invoiceType.Name = "invoiceType";
			// 
			// invoiceNumber
			// 
			this.invoiceNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.invoiceNumber.FillWeight = 35F;
			this.invoiceNumber.HeaderText = "Invoice number";
			this.invoiceNumber.Name = "invoiceNumber";
			// 
			// date
			// 
			this.date.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.date.FillWeight = 20F;
			this.date.HeaderText = "Date";
			this.date.Name = "date";
			// 
			// invoiceStatus
			// 
			this.invoiceStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.invoiceStatus.FillWeight = 20F;
			this.invoiceStatus.HeaderText = "Status";
			this.invoiceStatus.Name = "invoiceStatus";
			// 
			// direction
			// 
			this.direction.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.direction.FillWeight = 20F;
			this.direction.HeaderText = "Direction";
			this.direction.Name = "direction";
			// 
			// info
			// 
			this.info.HeaderText = "Info";
			this.info.Name = "info";
			// 
			// toolStripButton7
			// 
			this.toolStripButton7.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton7.Image")));
			this.toolStripButton7.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton7.Name = "toolStripButton7";
			this.toolStripButton7.Size = new System.Drawing.Size(106, 22);
			this.toolStripButton7.Text = "GetInvoiceById";
			this.toolStripButton7.Click += new System.EventHandler(this.toolStripButton7_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.toolStrip1);
			this.Name = "MainForm";
			this.Text = "MainForm";
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton toolStripButton1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.ToolStripButton toolStripButton2;
		private System.Windows.Forms.DataGridViewTextBoxColumn rowNumber;
		private System.Windows.Forms.DataGridViewTextBoxColumn invoiceType;
		private System.Windows.Forms.DataGridViewTextBoxColumn invoiceNumber;
		private System.Windows.Forms.DataGridViewTextBoxColumn date;
		private System.Windows.Forms.DataGridViewTextBoxColumn invoiceStatus;
		private System.Windows.Forms.DataGridViewTextBoxColumn direction;
		private System.Windows.Forms.DataGridViewTextBoxColumn info;
		private System.Windows.Forms.ToolStripButton toolStripButton3;
		private System.Windows.Forms.ToolStripButton toolStripButton4;
		private System.Windows.Forms.ToolStripButton toolStripButton5;
		private System.Windows.Forms.ToolStripButton toolStripButton6;
		private System.Windows.Forms.ToolStripButton toolStripButton7;
	}
}