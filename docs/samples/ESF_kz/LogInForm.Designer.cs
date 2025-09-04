namespace ESF_kz.Forms
{
	partial class LogInForm
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
			this.ofdSignCert = new System.Windows.Forms.OpenFileDialog();
			this.ofdAuthCert = new System.Windows.Forms.OpenFileDialog();
			this.tbTin = new System.Windows.Forms.TextBox();
			this.tbSignCert = new System.Windows.Forms.TextBox();
			this.tbAuthCert = new System.Windows.Forms.TextBox();
			this.btnSignCert = new System.Windows.Forms.Button();
			this.btnAuthCert = new System.Windows.Forms.Button();
			this.l_tin = new System.Windows.Forms.Label();
			this.l_signCertificatePath = new System.Windows.Forms.Label();
			this.l_authCertificatePath = new System.Windows.Forms.Label();
			this.btnLogIn = new System.Windows.Forms.Button();
			this.l_password = new System.Windows.Forms.Label();
			this.tbPassword = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// ofdSignCert
			// 
			this.ofdSignCert.FileName = "openFileDialog1";
			// 
			// ofdAuthCert
			// 
			this.ofdAuthCert.FileName = "openFileDialog2";
			// 
			// tbTin
			// 
			this.tbTin.Location = new System.Drawing.Point(106, 33);
			this.tbTin.Name = "tbTin";
			this.tbTin.Size = new System.Drawing.Size(234, 20);
			this.tbTin.TabIndex = 0;
			this.tbTin.Text = "760816300415";
			// 
			// tbSignCert
			// 
			this.tbSignCert.Location = new System.Drawing.Point(163, 104);
			this.tbSignCert.Name = "tbSignCert";
			this.tbSignCert.Size = new System.Drawing.Size(234, 20);
			this.tbSignCert.TabIndex = 1;
			this.tbSignCert.Text = "C:\\Users\\viktor.kassov\\source\\repos\\ESF_kz\\ESF_kz\\bin\\Debug\\Сертификат\\ИП Пинчук " +
    "ВВ до 17.06.21\\ИП Пинчук ВВ до 17.06.21\\RSA256_af8e6f8be023a8cc035198522a70ca720" +
    "3a7059a.p12";
			// 
			// tbAuthCert
			// 
			this.tbAuthCert.Location = new System.Drawing.Point(163, 130);
			this.tbAuthCert.Name = "tbAuthCert";
			this.tbAuthCert.Size = new System.Drawing.Size(234, 20);
			this.tbAuthCert.TabIndex = 2;
			this.tbAuthCert.Text = "C:\\Users\\viktor.kassov\\source\\repos\\ESF_kz\\ESF_kz\\bin\\Debug\\Сертификат\\ИП Пинчук " +
    "ВВ до 17.06.21\\ИП Пинчук ВВ до 17.06.21\\AUTH_RSA256_12fc440f2049f1b5b61765114f28" +
    "e58ec67eccff.p12";
			// 
			// btnSignCert
			// 
			this.btnSignCert.Location = new System.Drawing.Point(403, 105);
			this.btnSignCert.Name = "btnSignCert";
			this.btnSignCert.Size = new System.Drawing.Size(47, 19);
			this.btnSignCert.TabIndex = 4;
			this.btnSignCert.Text = "...";
			this.btnSignCert.UseVisualStyleBackColor = true;
			this.btnSignCert.Click += new System.EventHandler(this.btnSignCert_Click);
			// 
			// btnAuthCert
			// 
			this.btnAuthCert.Location = new System.Drawing.Point(403, 131);
			this.btnAuthCert.Name = "btnAuthCert";
			this.btnAuthCert.Size = new System.Drawing.Size(47, 19);
			this.btnAuthCert.TabIndex = 5;
			this.btnAuthCert.Text = "...";
			this.btnAuthCert.UseVisualStyleBackColor = true;
			this.btnAuthCert.Click += new System.EventHandler(this.btnAuthCert_Click);
			// 
			// l_tin
			// 
			this.l_tin.AutoSize = true;
			this.l_tin.Location = new System.Drawing.Point(42, 36);
			this.l_tin.Name = "l_tin";
			this.l_tin.Size = new System.Drawing.Size(22, 13);
			this.l_tin.TabIndex = 6;
			this.l_tin.Text = "Tin";
			// 
			// l_signCertificatePath
			// 
			this.l_signCertificatePath.AutoSize = true;
			this.l_signCertificatePath.Location = new System.Drawing.Point(25, 107);
			this.l_signCertificatePath.Name = "l_signCertificatePath";
			this.l_signCertificatePath.Size = new System.Drawing.Size(97, 13);
			this.l_signCertificatePath.TabIndex = 7;
			this.l_signCertificatePath.Text = "SignCertificatePath";
			// 
			// l_authCertificatePath
			// 
			this.l_authCertificatePath.AutoSize = true;
			this.l_authCertificatePath.Location = new System.Drawing.Point(25, 134);
			this.l_authCertificatePath.Name = "l_authCertificatePath";
			this.l_authCertificatePath.Size = new System.Drawing.Size(95, 13);
			this.l_authCertificatePath.TabIndex = 8;
			this.l_authCertificatePath.Text = "AuthCerificatePath";
			// 
			// btnLogIn
			// 
			this.btnLogIn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnLogIn.Location = new System.Drawing.Point(359, 36);
			this.btnLogIn.Name = "btnLogIn";
			this.btnLogIn.Size = new System.Drawing.Size(72, 39);
			this.btnLogIn.TabIndex = 9;
			this.btnLogIn.Text = "LogIn";
			this.btnLogIn.UseVisualStyleBackColor = true;
			this.btnLogIn.Click += new System.EventHandler(this.button1_Click);
			// 
			// l_password
			// 
			this.l_password.AutoSize = true;
			this.l_password.Location = new System.Drawing.Point(42, 62);
			this.l_password.Name = "l_password";
			this.l_password.Size = new System.Drawing.Size(53, 13);
			this.l_password.TabIndex = 11;
			this.l_password.Text = "Password";
			// 
			// tbPassword
			// 
			this.tbPassword.Location = new System.Drawing.Point(106, 59);
			this.tbPassword.Name = "tbPassword";
			this.tbPassword.PasswordChar = '*';
			this.tbPassword.Size = new System.Drawing.Size(234, 20);
			this.tbPassword.TabIndex = 10;
			this.tbPassword.Text = "Micr0!nvest";
			// 
			// LogInForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(480, 159);
			this.Controls.Add(this.l_password);
			this.Controls.Add(this.tbPassword);
			this.Controls.Add(this.btnLogIn);
			this.Controls.Add(this.l_authCertificatePath);
			this.Controls.Add(this.l_signCertificatePath);
			this.Controls.Add(this.l_tin);
			this.Controls.Add(this.btnAuthCert);
			this.Controls.Add(this.btnSignCert);
			this.Controls.Add(this.tbAuthCert);
			this.Controls.Add(this.tbSignCert);
			this.Controls.Add(this.tbTin);
			this.Name = "LogInForm";
			this.Text = "LogInForm";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.OpenFileDialog ofdSignCert;
		private System.Windows.Forms.OpenFileDialog ofdAuthCert;
		private System.Windows.Forms.TextBox tbTin;
		private System.Windows.Forms.TextBox tbSignCert;
		private System.Windows.Forms.TextBox tbAuthCert;
		private System.Windows.Forms.Button btnSignCert;
		private System.Windows.Forms.Button btnAuthCert;
		private System.Windows.Forms.Label l_tin;
		private System.Windows.Forms.Label l_signCertificatePath;
		private System.Windows.Forms.Label l_authCertificatePath;
		private System.Windows.Forms.Button btnLogIn;
		private System.Windows.Forms.Label l_password;
		private System.Windows.Forms.TextBox tbPassword;
	}
}