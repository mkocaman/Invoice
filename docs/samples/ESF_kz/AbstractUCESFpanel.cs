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
	public partial class AbstractUCESFpanel : UserControl
	{
		public AbstractUCESFpanel()
		{
			InitializeComponent();
		}

		private ESF_form ESFForm;

		internal void setESFform(ESF_form eSF_form)
		{
			ESFForm = eSF_form;
		}

		internal ESF_form getESFform()
		{
			return ESFForm;
		}
	}
}
