namespace ESF_kz.Forms
{
	partial class panelESFpartK
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.label9 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.tbAddInf = new System.Windows.Forms.TextBox();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
			this.tableLayoutPanel1.Controls.Add(this.label9, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.tbAddInf, 1, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.00005F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 89.99995F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(897, 570);
			this.tableLayoutPanel1.TabIndex = 5;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.label9, 2);
			this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label9.Location = new System.Drawing.Point(4, 1);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(889, 56);
			this.label9.TabIndex = 45;
			this.label9.Text = "Раздел K. Дополнительные сведения";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(4, 58);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(164, 13);
			this.label1.TabIndex = 46;
			this.label1.Text = "43. Дополнительные сведения";
			// 
			// tbAddInf
			// 
			this.tbAddInf.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbAddInf.Location = new System.Drawing.Point(183, 61);
			this.tbAddInf.Multiline = true;
			this.tbAddInf.Name = "tbAddInf";
			this.tbAddInf.Size = new System.Drawing.Size(710, 505);
			this.tbAddInf.TabIndex = 47;
			// 
			// panelESFpartK
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "panelESFpartK";
			this.Size = new System.Drawing.Size(897, 570);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbAddInf;
	}
}
