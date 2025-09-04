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
	public partial class panelESFpartC : AbstractUCESFpanel
	{
		public panelESFpartC()
		{
			InitializeComponent();
			CreateFirstTab("Customer");
		}


		public panelESFpartCtab CreateTab(string title)
		{
			panelESFpartCtab PanelESFPartCtab = new panelESFpartCtab();
			PanelESFPartCtab.setESFform(this.getESFform());
			PanelESFPartCtab.Dock = DockStyle.Fill;
			this.tabControl1.TabPages.Add(title);
			this.tabControl1.TabPages[tabControl1.TabCount - 1].Controls.Add(PanelESFPartCtab);
			return PanelESFPartCtab;
		}

		public panelESFpartCtab CreateFirstTab(string title)
		{
			panelESFpartCtab PanelESFPartCtab = new panelESFpartCtab();
			PanelESFPartCtab.setESFform(this.getESFform());
			PanelESFPartCtab.Dock = DockStyle.Fill;
			this.tabControl1.TabPages.Add(title);
			this.tabControl1.TabPages[0].Controls.Add(PanelESFPartCtab);
			return PanelESFPartCtab;
		}

		public void RemoveLastTab()
		{

		}

		public void RemoveAllTabs()
		{
			getTabControll().TabPages.Clear();
		}




		public TabControl getTabControll()
		{
			return this.tabControl1;
		}
		public panelESFpartCtab getTab()
		{
			return (panelESFpartCtab)(this.tabControl1.TabPages[0].Controls[0]);
		}

		public panelESFpartCtab getTab(int num)
		{
			return (panelESFpartCtab)(this.tabControl1.TabPages[num-1].Controls[0]);
		}

		internal int getCustomerParticipantsCount()
		{
			return getTab().getCustomerParticipantsCount();
		}

		internal bool setCustomerParticipantsCount(int num)
		{
			try
			{
				getTab().setCustomerParticipantsCount(num);
				return true;
			}
			catch (Exception)
			{
				return false;
			}			
		}
	}
}
