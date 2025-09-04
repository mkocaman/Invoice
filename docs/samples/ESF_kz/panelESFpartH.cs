using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Deployment.Internal;
using System.Threading;

namespace ESF_kz.Forms
{
	public partial class panelESFpartH : AbstractUCESFpanel
	{
		List<int> customerIndexes = new List<int>();
		List<int> sellerIndexes = new List<int>();

		public panelESFpartH()
		{
			InitializeComponent();
		}

		internal List<int> getCustomerIndexes()
		{
			return customerIndexes;
		}

		internal List<int> getSellerIndexes()
		{
			return sellerIndexes;
		}

		public panelESFpartHtab CreateSellerTab(string title)
		{
			panelESFpartHtab PanelESFPartHtab = new panelESFpartHtab();
			PanelESFPartHtab.setESFform(this.getESFform());
			PanelESFPartHtab.Dock = DockStyle.Fill;
			this.tabControl1.TabPages.Add("Seller"+title);
			this.tabControl1.TabPages[tabControl1.TabCount - 1].Controls.Add(PanelESFPartHtab);
			sellerIndexes.Add(tabControl1.TabCount - 1);

			return PanelESFPartHtab;
		}

		internal bool AddNewProductRow(ProductV2 product)
		{
			int sellerParticipantCount = sellerIndexes.Count;
			int customerParticipantCount =	customerIndexes.Count;

			for (int i = 1; i <= sellerParticipantCount; i++)
			{
				float shareParticipation = FormManagerFacade.getSellerShareParticipation(i);
				getSellerTab(i).AddNewProductRow(product, shareParticipation);
			}

			for (int i = 1; i <= customerParticipantCount; i++)
			{
				float shareParticipation = FormManagerFacade.getCustomerShareParticipation(i);
				getCustomerTab(i).AddNewProductRow(product, shareParticipation);
			}

			return true;
		}

		public panelESFpartHtab CreateCustomerTab(string title)
		{
			panelESFpartHtab PanelESFPartHtab = new panelESFpartHtab();
			PanelESFPartHtab.setESFform(this.getESFform());
			PanelESFPartHtab.Dock = DockStyle.Fill;
			this.tabControl1.TabPages.Add("Customer"+title);
			this.tabControl1.TabPages[tabControl1.TabCount - 1].Controls.Add(PanelESFPartHtab);
			customerIndexes.Add(tabControl1.TabCount - 1);
			return PanelESFPartHtab;
		}

		/*public panelESFpartHtab CreateFirstTab(string title)
		{
			panelESFpartHtab PanelESFPartHtab = new panelESFpartHtab();
			PanelESFPartHtab.setESFform(this.getESFform());
			PanelESFPartHtab.Dock = DockStyle.Fill;
			this.tabControl1.TabPages.Add(title);
			this.tabControl1.TabPages[0].Controls.Add(PanelESFPartHtab);
			return PanelESFPartHtab;
		}*/

		public TabControl getTabControll()
		{
			return this.tabControl1;
		}

		public panelESFpartHtab getTab()
		{
			return (panelESFpartHtab)(this.tabControl1.TabPages[0].Controls[0]);
		}

		public panelESFpartHtab getSellerTab(int num)
		{
			return (panelESFpartHtab)(this.tabControl1.TabPages[sellerIndexes[num - 1]].Controls[0]);
		}

		public panelESFpartHtab getCustomerTab(int num)
		{
			return (panelESFpartHtab)(this.tabControl1.TabPages[customerIndexes[num - 1]].Controls[0]);
		}

		internal void RemoveLastSellerTab()
		{
			int count = customerIndexes.Count;
			for (int i = 0; i < count; i++)
			{
				if (customerIndexes[i] > sellerIndexes[sellerIndexes.Count-1])
				{
					customerIndexes[i]--;
				}
			}
			RemoveTabById(sellerIndexes[sellerIndexes.Count-1]);
			sellerIndexes.RemoveAt(sellerIndexes.Count - 1);
		}

		internal void RemoveLastCustomerTab()
		{
			int count = sellerIndexes.Count;
			for (int i = 0; i < count; i++)
			{
				if (sellerIndexes[i] > customerIndexes[customerIndexes.Count-1])
				{
					sellerIndexes[i]--;
				}
			}
			RemoveTabById(customerIndexes[customerIndexes.Count - 1]);
			customerIndexes.RemoveAt(customerIndexes.Count - 1);
		}


		internal void RemoveAllTabs()
		{
			getTabControll().TabPages.Clear();
			sellerIndexes.Clear();
			customerIndexes.Clear();
		}

		internal void RemoveTabById(int id)
		{
			getTabControll().TabPages.Remove(getTabControll().TabPages[id]);
		}

		internal void RemoveAllSellerTabs()
		{
			int count = sellerIndexes.Count;
			for (int i = 0; i < count; i++)
			{
				RemoveLastSellerTab();
			}
		}

		internal void RemoveAllCustomerTabs()
		{
			int count = customerIndexes.Count;
			for (int i = 0; i < count; i++)
			{
				RemoveLastCustomerTab();
			}
		}

		internal void RecalcTotalAmounts()
		{
			foreach (TabPage item in getTabControll().TabPages)
			{
				((panelESFpartHtab)item.Controls[0]).RecalcTotalAmounts();
			}
		}

		internal bool EditProductShareRow(ProductV2 product, int rowNumber)
		{
			int sellerParticipantCount = sellerIndexes.Count;
			int customerParticipantCount = customerIndexes.Count;

			for (int i = 1; i <= sellerParticipantCount; i++)
			{
				float shareParticipation = FormManagerFacade.getSellerShareParticipation(i);
				getSellerTab(i).EditProductRow(product, shareParticipation, rowNumber);
			}

			for (int i = 1; i <= customerParticipantCount; i++)
			{
				float shareParticipation = FormManagerFacade.getCustomerShareParticipation(i);
				getCustomerTab(i).EditProductRow(product, shareParticipation, rowNumber);
			}

			return true;
		}
	}
}
