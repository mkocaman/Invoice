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
	public partial class panelESFpartHtab : AbstractUCESFpanelTab
	{
		enum column
		{
			rowNumber,
			typeTRN,
			nameTRN,
			fullnameTRN,
			TNVED,
			measure,
			quantity,
			pricePerOne,
			priceWithoutTax,
			exciseRate,
			exciseAmount,
			turnoverSize,
			ndsRate,
			ndsAmount,
			priceWithTax,
			productDeclaration,
			productNumberInDeclaration,
			catalogTruId,
			additional
		}


		public panelESFpartHtab()
		{
			InitializeComponent();
			FillProductShares();
		}

		private void FillProductShares()
		{
			int productCount = SessionDataManagerFacade.getProductsCount();
			for (int i = 1; i <= productCount; i++)
			{
				ProductV2 product = SessionDataManagerFacade.getProductV2ByRowNumber(i);
				AddNewProductRow(product, 0);
			}			
		}

		internal DataGridView GetDataGrid()
		{
			return dataGridView1;
		}

		internal string getProductShareAdditional(int productShareNumber)
		{
			return dataGridView1.Rows[productShareNumber-1].Cells[(int)column.additional].Value.ToString();
		}

		internal bool setProductShareAdditional(int productShareNumber, string additional)
		{
			try
			{
				dataGridView1.Rows[productShareNumber-1].Cells[(int)column.additional].Value = additional;
				return true;
			}
			catch (Exception)
			{
				return false;
			}			
		}

		internal float getProductShareExciseAmount(int productShareNumber)
		{
			return (float)dataGridView1.Rows[productShareNumber-1].Cells[(int)column.exciseAmount].Value;
		}

		internal bool setProductShareExciseAmount(int productShareNumber, float exciseAmount)
		{
			try
			{
				dataGridView1.Rows[productShareNumber-1].Cells[(int)column.exciseAmount].Value = exciseAmount;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		
		}

		internal void AddNewProductRow(ProductV2 product, float shareParticipation)
		{
			dataGridView1.Rows.Add();
			int productShareNumber = dataGridView1.Rows.Count;			
			setShareProductNumber(productShareNumber,productShareNumber);
			setProductShareTruOriginCode(productShareNumber, product.truOriginCode);
			setProductShareAdditional(productShareNumber, product.additional);
			setProductShareCatalogTruId(productShareNumber, product.catalogTruId);
			setProductShareDescription(productShareNumber, product.description);
			setProductShareExciseAmount(productShareNumber, product.exciseAmount * shareParticipation);
			setProductShareExciseRate(productShareNumber, product.exciseRate);
			setProductShareNDSAmount(productShareNumber, product.ndsAmount);
			setProductShareNdsRate(productShareNumber, product.ndsRate);
			setProductSharePriceWithTax(productShareNumber, product.priceWithTax * shareParticipation);
			setProductSharePriceWithoutTax(productShareNumber, product.priceWithoutTax * shareParticipation);
			setProductShareProductDeclaration(productShareNumber, product.productDeclaration);
			setProductShareProductNumberInDeclaration(productShareNumber, product.productNumberInDeclaration);
			setProductShareQuantity(productShareNumber, product.quantity*shareParticipation);
			setProductShareTnvedName(productShareNumber, product.tnvedName);
			setProductShareTurnoverSize(productShareNumber, product.turnoverSize * shareParticipation);
			setProductShareUnitCode(productShareNumber, product.unitCode);
			setProductShareUnitNomenclature(productShareNumber, product.unitNomenclature);
			setProductShareUnitPrice(productShareNumber, product.unitPrice);
		}

		internal void EditProductRow(ProductV2 product, float shareParticipation, int productShareNumber)
		{
			setShareProductNumber(productShareNumber, productShareNumber);
			setProductShareTruOriginCode(productShareNumber, product.truOriginCode);
			setProductShareAdditional(productShareNumber, product.additional);
			setProductShareCatalogTruId(productShareNumber, product.catalogTruId);
			setProductShareDescription(productShareNumber, product.description);
			setProductShareExciseAmount(productShareNumber, product.exciseAmount * shareParticipation);
			setProductShareExciseRate(productShareNumber, product.exciseRate);
			setProductShareNDSAmount(productShareNumber, product.ndsAmount);
			setProductShareNdsRate(productShareNumber, product.ndsRate);
			setProductSharePriceWithTax(productShareNumber, product.priceWithTax * shareParticipation);
			setProductSharePriceWithoutTax(productShareNumber, product.priceWithoutTax * shareParticipation);
			setProductShareProductDeclaration(productShareNumber, product.productDeclaration);
			setProductShareProductNumberInDeclaration(productShareNumber, product.productNumberInDeclaration);
			setProductShareQuantity(productShareNumber, product.quantity * shareParticipation);
			setProductShareTnvedName(productShareNumber, product.tnvedName);
			setProductShareTurnoverSize(productShareNumber, product.turnoverSize * shareParticipation);
			setProductShareUnitCode(productShareNumber, product.unitCode);
			setProductShareUnitNomenclature(productShareNumber, product.unitNomenclature);
			setProductShareUnitPrice(productShareNumber, product.unitPrice);
		}

		internal void RecalcAmounts(float shareParicipation)
		{
			for (int i = 0; i < dataGridView1.Rows.Count; i++)
			{
				ProductV2 product = SessionDataManagerFacade.getProductV2ByRowNumber(getRowNumber(i));
				EditProductRow(product, shareParicipation, i+1);
			}
			RecalcTotalAmounts();
		}

		private int getRowNumber(int i)
		{
			return (int)dataGridView1.Rows[i].Cells["rowNumber"].Value;
		}

		internal void RecalcTotalAmounts()
		{
			DataGridView dataGrid = GetDataGrid();
			float totalExciseAmount = 0;
			float totalNdsAmount = 0;
			float totalPriceWithoutTax = 0;
			float totalPriceWithTax = 0;
			float totalTurnoverSize = 0;
			object temp;
			foreach (DataGridViewRow row in dataGrid.Rows)
			{
				temp = row.Cells["exciseAmount"].Value;
				totalExciseAmount += temp == null ? 0 : (float)temp;
				temp = row.Cells["ndsAmount"].Value;
				totalNdsAmount += temp == null ? 0 : (float)temp;
				temp = row.Cells["priceWithoutTax"].Value;
				totalPriceWithoutTax += temp == null ? 0 : (float)temp;
				temp = row.Cells["priceWithTax"].Value;
				totalPriceWithTax += temp == null ? 0 : (float)temp;
				temp = row.Cells["turnoverSize"].Value;
				totalTurnoverSize += temp == null ? 0 : (float)temp;
			}
			tbPartHtab_totalExciseAmount.Text = totalExciseAmount.ToString();
			tbPartHtab_totalNdsAmount.Text = totalNdsAmount.ToString();
			tbPartHtab_totalPriceWithoutTax.Text = totalPriceWithoutTax.ToString();
			tbPartHtab_totalPriceWithTax.Text = totalPriceWithTax.ToString();
			tbPartHtab_totalTurnoverSize.Text = totalTurnoverSize.ToString();
		}

		internal float getProductShareNDSAmount(int productShareNumber)
		{
			return (float)dataGridView1.Rows[productShareNumber-1].Cells[(int)column.ndsAmount].Value;
		}

		internal bool setProductShareNDSAmount(int productShareNumber, float ndsAmount)
		{
			try
			{
				dataGridView1.Rows[productShareNumber-1].Cells[(int)column.ndsAmount].Value = ndsAmount;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal float getProductSharePriceWithoutTax(int productShareNumber)
		{
			return (float)dataGridView1.Rows[productShareNumber-1].Cells[(int)column.priceWithoutTax].Value;
		}

		internal bool setProductSharePriceWithoutTax(int productShareNumber, float priceWithoutTax)
		{
			try
			{
				dataGridView1.Rows[productShareNumber-1].Cells[(int)column.priceWithoutTax].Value = priceWithoutTax;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal float getProductSharePriceWithTax(int productShareNumber)
		{
			return (float)dataGridView1.Rows[productShareNumber-1].Cells[(int)column.priceWithTax].Value;
		}

		internal bool setProductSharePriceWithTax(int productShareNumber, float priceWithTax)
		{
			try
			{
				dataGridView1.Rows[productShareNumber-1].Cells[(int)column.priceWithTax].Value = priceWithTax;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal int getShareProductNumber(int productShareNumber)
		{
			return (int)dataGridView1.Rows[productShareNumber-1].Cells[(int)column.rowNumber].Value;
		}

		internal bool setShareProductNumber(int productShareNumber, int productNumber)
		{
			try
			{
				dataGridView1.Rows[productShareNumber-1].Cells[(int)column.rowNumber].Value = productNumber;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal float getProductShareQuantity(int productShareNumber)
		{
			return (float)dataGridView1.Rows[productShareNumber-1].Cells[(int)column.quantity].Value;
		}

		internal bool setProductShareQuantity(int productShareNumber, float qtty)
		{
			try
			{
				dataGridView1.Rows[productShareNumber-1].Cells[(int)column.quantity].Value = qtty;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal float getProductShareTurnoverSize(int productShareNumber)
		{
			return (float)dataGridView1.Rows[productShareNumber-1].Cells[(int)column.turnoverSize].Value;
		}

		internal bool setProductShareTurnoverSize(int productShareNumber, float turnoverSize)
		{
			try
			{
				dataGridView1.Rows[productShareNumber-1].Cells[(int)column.turnoverSize].Value = turnoverSize;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal bool setParticipantTin(string tin)
		{
			try
			{
				tbPartH_ParticipantTin.Text = tin;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal bool setParticipantReorganizedTin(string reorganizedTin)
		{
			try
			{
				tbPartH_ParticipantReorgTin.Text = reorganizedTin;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal bool setProductShareDescription(int productShareNumber, string description)
		{
			try
			{
				GetDataGrid().Rows[productShareNumber - 1].Cells[(int)column.nameTRN].Value = description;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal bool setProductShareExciseRate(int productShareNumber, float exciseRate)
		{
			try
			{
				GetDataGrid().Rows[productShareNumber - 1].Cells[(int)column.exciseRate].Value = exciseRate;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal bool setProductShareProductDeclaration(int productShareNumber, string productDeclaration)
		{
			try
			{
				GetDataGrid().Rows[productShareNumber - 1].Cells[(int)column.productDeclaration].Value = productDeclaration;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal bool setProductShareNdsRate(int productShareNumber, int ndsRate)
		{
			try
			{
				GetDataGrid().Rows[productShareNumber - 1].Cells[(int)column.ndsRate].Value = ndsRate;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal bool setProductShareProductNumberInDeclaration(int productShareNumber, string productNumberInDeclaration)
		{
			try
			{
				GetDataGrid().Rows[productShareNumber - 1].Cells[(int)column.productNumberInDeclaration].Value = productNumberInDeclaration;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal bool setProductShareTnvedName(int productShareNumber, string tnvedName)
		{
			try
			{
				GetDataGrid().Rows[productShareNumber - 1].Cells[(int)column.fullnameTRN].Value = tnvedName;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal bool setProductShareTruOriginCode(int productNum, int code)
		{
			try
			{
				DataGridViewComboBoxCell comboBoxCell = (DataGridViewComboBoxCell)GetDataGrid().Rows[productNum - 1].Cells[(int)column.typeTRN];
				comboBoxCell.Value = comboBoxCell.Items[code-1];
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal bool setProductShareUnitCode(int productShareNumber, string unitCode)
		{
			try
			{
				GetDataGrid().Rows[productShareNumber - 1].Cells[(int)column.TNVED].Value = unitCode;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal bool setProductShareUnitNomenclature(int productShareNumber, string unitNomenclature)
		{
			try
			{
				GetDataGrid().Rows[productShareNumber - 1].Cells[(int)column.measure].Value = unitNomenclature;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal bool setProductShareUnitPrice(int productShareNumber, float unitPrice)
		{
			try
			{
				GetDataGrid().Rows[productShareNumber - 1].Cells[(int)column.pricePerOne].Value = unitPrice.ToString();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal bool setProductShareCatalogTruId(int productShareNumber, string catalogTruId)
		{
			try
			{
				GetDataGrid().Rows[productShareNumber - 1].Cells[(int)column.catalogTruId].Value = catalogTruId;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}


	}
}
