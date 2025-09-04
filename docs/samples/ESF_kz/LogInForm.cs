using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ESF_kz.Forms
{
	public partial class LogInForm : Form
	{
		public LogInForm()
		{
			InitializeComponent();
		}

		private void btnSignCert_Click(object sender, EventArgs e)
		{
			if (ofdSignCert.ShowDialog() == DialogResult.Cancel)
				return;
			string signCertPath = ofdSignCert.FileName;
			tbSignCert.Text = signCertPath;		
		}

		private void btnAuthCert_Click(object sender, EventArgs e)
		{
			if (ofdAuthCert.ShowDialog() == DialogResult.Cancel)
				return;
			string authCertPath = ofdAuthCert.FileName;
			tbAuthCert.Text = authCertPath;
		}

		private void button1_Click(object sender, EventArgs e)
		{

			SessionDataManagerFacade.setUserTin(tbTin.Text);
			SessionDataManagerFacade.setUserPassword(tbPassword.Text);
			SessionDataManagerFacade.setSignCertPath(tbSignCert.Text);
			SessionDataManagerFacade.setAuthCertPath(tbAuthCert.Text);
			//CreateMainForm();
			//closeLogInForm();
		}

		private void CreateMainForm()
		{
			MainForm main = new MainForm();
			main.Show();			
		}

		private void closeLogInForm()
		{
			this.Close();
		}
	}
}
