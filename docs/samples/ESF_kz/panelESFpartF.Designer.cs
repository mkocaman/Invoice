namespace ESF_kz.Forms
{
	partial class panelESFpartF
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.l_PartF_deliveryDocNum = new System.Windows.Forms.Label();
			this.l_PartF_deliveryDocDate = new System.Windows.Forms.Label();
			this.tbPartF_deliveryDocNum = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.epPartF_deliveryDocNum = new System.Windows.Forms.ErrorProvider(this.components);
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.tbPartF_deliveryDocDate = new System.Windows.Forms.TextBox();
			this.dtpPartF_deliveryDocDate = new System.Windows.Forms.DateTimePicker();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.epPartF_deliveryDocNum)).BeginInit();
			this.tableLayoutPanel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.l_PartF_deliveryDocNum, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.l_PartF_deliveryDocDate, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.tbPartF_deliveryDocNum, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.label9, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 3);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 5;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.00006F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.00006F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.00006F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.00006F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 59.99976F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(783, 561);
			this.tableLayoutPanel1.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.Location = new System.Drawing.Point(4, 57);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(775, 55);
			this.label1.TabIndex = 0;
			this.label1.Text = "32. Документ, подтверждающий поставку товаров, работ, услуг";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// l_PartF_deliveryDocNum
			// 
			this.l_PartF_deliveryDocNum.AutoSize = true;
			this.l_PartF_deliveryDocNum.Dock = System.Windows.Forms.DockStyle.Fill;
			this.l_PartF_deliveryDocNum.Location = new System.Drawing.Point(4, 113);
			this.l_PartF_deliveryDocNum.Name = "l_PartF_deliveryDocNum";
			this.l_PartF_deliveryDocNum.Size = new System.Drawing.Size(384, 55);
			this.l_PartF_deliveryDocNum.TabIndex = 1;
			this.l_PartF_deliveryDocNum.Text = "32.1 номер";
			this.l_PartF_deliveryDocNum.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// l_PartF_deliveryDocDate
			// 
			this.l_PartF_deliveryDocDate.AutoSize = true;
			this.l_PartF_deliveryDocDate.Dock = System.Windows.Forms.DockStyle.Fill;
			this.l_PartF_deliveryDocDate.Location = new System.Drawing.Point(4, 169);
			this.l_PartF_deliveryDocDate.Name = "l_PartF_deliveryDocDate";
			this.l_PartF_deliveryDocDate.Size = new System.Drawing.Size(384, 55);
			this.l_PartF_deliveryDocDate.TabIndex = 3;
			this.l_PartF_deliveryDocDate.Text = "32.2 дата";
			this.l_PartF_deliveryDocDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tbPartF_deliveryDocNum
			// 
			this.tbPartF_deliveryDocNum.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.tbPartF_deliveryDocNum.Location = new System.Drawing.Point(395, 130);
			this.tbPartF_deliveryDocNum.Name = "tbPartF_deliveryDocNum";
			this.tbPartF_deliveryDocNum.Size = new System.Drawing.Size(350, 20);
			this.tbPartF_deliveryDocNum.TabIndex = 14;
			this.tbPartF_deliveryDocNum.TextChanged += new System.EventHandler(this.tbPartF_deliveryDocNum_TextChanged);
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.label9, 2);
			this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label9.Location = new System.Drawing.Point(4, 1);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(775, 55);
			this.label9.TabIndex = 45;
			this.label9.Text = "Раздел F. Реквизиты документов, подтверждающих поставку товаров, работ, услуг";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// epPartF_deliveryDocNum
			// 
			this.epPartF_deliveryDocNum.ContainerControl = this;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.tableLayoutPanel2.ColumnCount = 2;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.Controls.Add(this.dtpPartF_deliveryDocDate, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.tbPartF_deliveryDocDate, 0, 0);
			this.tableLayoutPanel2.Location = new System.Drawing.Point(395, 180);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 1;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(200, 33);
			this.tableLayoutPanel2.TabIndex = 58;
			// 
			// tbPartF_deliveryDocDate
			// 
			this.tbPartF_deliveryDocDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.tbPartF_deliveryDocDate.Location = new System.Drawing.Point(3, 6);
			this.tbPartF_deliveryDocDate.Name = "tbPartF_deliveryDocDate";
			this.tbPartF_deliveryDocDate.Size = new System.Drawing.Size(165, 20);
			this.tbPartF_deliveryDocDate.TabIndex = 18;
			// 
			// dtpPartF_deliveryDocDate
			// 
			this.dtpPartF_deliveryDocDate.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.dtpPartF_deliveryDocDate.Location = new System.Drawing.Point(174, 6);
			this.dtpPartF_deliveryDocDate.Name = "dtpPartF_deliveryDocDate";
			this.dtpPartF_deliveryDocDate.Size = new System.Drawing.Size(18, 20);
			this.dtpPartF_deliveryDocDate.TabIndex = 47;
			this.dtpPartF_deliveryDocDate.ValueChanged += new System.EventHandler(this.dtpPartF_deliveryDocDate_ValueChanged);
			// 
			// panelESFpartF
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "panelESFpartF";
			this.Size = new System.Drawing.Size(783, 561);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.epPartF_deliveryDocNum)).EndInit();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label l_PartF_deliveryDocNum;
		private System.Windows.Forms.Label l_PartF_deliveryDocDate;
		private System.Windows.Forms.TextBox tbPartF_deliveryDocNum;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.ErrorProvider epPartF_deliveryDocNum;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.DateTimePicker dtpPartF_deliveryDocDate;
		private System.Windows.Forms.TextBox tbPartF_deliveryDocDate;
	}
}
