using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace ESF_kz.Forms
{
	public partial class InfoForm : Form
	{
		public InfoForm(string title, string info)
		{
			InitializeComponent();
			this.l_Info.MaximumSize = new Size((int)SystemParameters.VirtualScreenWidth / 3, (int)SystemParameters.VirtualScreenHeight / 2);
			this.Text = title;
			this.l_Info.Text = info;			
		}

		private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
		{

		}
	}
}
