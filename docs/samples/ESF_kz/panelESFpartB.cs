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
	public partial class panelESFpartB :  AbstractUCESFpanel
	{
		public panelESFpartB()
		{
			InitializeComponent();
			CreateFirstTab("Seller");
		}

		public panelESFpartBtab CreateTab(string title)
		{
			panelESFpartBtab PanelESFPartBtab = new panelESFpartBtab();
			//PanelESFPartBtab.setESFform(this.getESFform());
			//var obj = PanelESFPartBtab.getESFform();
			PanelESFPartBtab.Dock = DockStyle.Fill;
			this.tabControl1.TabPages.Add(title);
			this.tabControl1.TabPages[tabControl1.TabCount-1].Controls.Add(PanelESFPartBtab);
			PanelESFPartBtab.setSellerIndex(tabControl1.TabCount);
			return PanelESFPartBtab;
		}

		public panelESFpartBtab CreateFirstTab(string title)
		{
			panelESFpartBtab PanelESFPartBtab = new panelESFpartBtab();
			PanelESFPartBtab.Dock = DockStyle.Fill;
			this.tabControl1.TabPages.Add(title);
			this.tabControl1.TabPages[0].Controls.Add(PanelESFPartBtab);
			PanelESFPartBtab.setSellerIndex(1);
			return PanelESFPartBtab;
		}

		public void RemoveTabById(int id)
		{
			getTabControll().TabPages.Remove(getTabControll().TabPages[id]);
		}

		public void RemoveAllTabs()
		{
			getTabControll().TabPages.Clear();
		}



		public TabControl getTabControll()
		{
			return this.tabControl1;
		}

		public panelESFpartBtab getTab()
		{
			return (panelESFpartBtab)(this.tabControl1.TabPages[0].Controls[0]);
		}

		public panelESFpartBtab getTab(int num)
		{
			return (panelESFpartBtab)(this.tabControl1.TabPages[num - 1].Controls[0]);
		}


		internal bool setSellerParticipantsCount(int num)
		{
			try
			{
				getTab().setSellerParticipantsCount(num);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal int getSellerParticipantsCount()
		{
			return getTab().getSellerParticipantsCount();
		}
	}
}
