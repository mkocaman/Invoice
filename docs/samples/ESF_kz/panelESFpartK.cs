using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ESF_kz.Forms
{
	public partial class panelESFpartK : AbstractUCESFpanel
	{
		public panelESFpartK()
		{
			InitializeComponent();
		}

		internal string getInvoiceAddInf()
		{
			return tbAddInf.Text;
		}

		internal bool setInvoiceAddInf(string addinf)
		{
			try
			{
				tbAddInf.Text = addinf;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
