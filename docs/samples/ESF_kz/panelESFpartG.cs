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
	public partial class panelESFpartG : AbstractUCESFpanel
	{
		int selectedRowIndex = 0;

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

		public panelESFpartG()
		{
			InitializeComponent();
		}

		private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{

		}

		private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
		{

		}

		internal string getProductSetCurrentCode()
		{
			return tbPartG_currencyCode.Text;
		}

		internal bool setProductSetCurrentCode(string code)
		{
			try
			{
				tbPartG_currencyCode.Text = code;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal float getProductSetCurrencyRate()
		{
			return tbPartG_currencyRate.Text == "" ? 0: float.Parse(tbPartG_currencyRate.Text);
		}

		internal bool setProductSetCurrencyRate(float rate)
		{
			try
			{
				tbPartG_currencyRate.Text = rate.ToString();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal int getProductsCount()
		{
			return GetDataGrid().Rows.Count;
		}

		internal DataGridView GetDataGrid()
		{
			return dataGridView1;
		}

		internal string getProductAdditional(int productNum)
		{
			if(GetDataGrid().Rows[productNum - 1].Cells[(int)column.additional].Value!=null)
			return GetDataGrid().Rows[productNum-1].Cells[(int)column.additional].Value.ToString();
			return "";
		}

		internal bool setProductAdditional(int productNum,string additional)
		{
			try
			{
				GetDataGrid().Rows[productNum-1].Cells[(int)column.additional].Value = additional;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal bool AddNewProductRow(ProductV2 product)
		{
			try
			{
				GetDataGrid().Rows.Add();
				int newRowIndex = GetDataGrid().Rows.Count;				
				setProductNumber(newRowIndex, newRowIndex);
				setProductAdditional(newRowIndex, product.additional);
				setProductCatalogTruId(newRowIndex, product.catalogTruId);
				setProductDescription(newRowIndex, product.description);
				setProductExciseAmount(newRowIndex, product.exciseAmount);
				setProductExciseRate(newRowIndex, product.exciseRate);
				//panelG.setProductKpvedCode(productCounter, product.kpvedCode);
				setProductNDSAmount(newRowIndex, product.ndsAmount);
				setProductNDSRate(newRowIndex, product.ndsRate);
				setProductPriceWithTax(newRowIndex, product.priceWithTax);
				setProductPriceWithoutTax(newRowIndex, product.priceWithoutTax);
				setProductDeclaration(newRowIndex, product.productDeclaration);
				setProductNumberInDeclaration(newRowIndex, product.productNumberInDeclaration);
				setProductQuantity(newRowIndex, product.quantity);
				setProductTnvedName(newRowIndex, product.tnvedName);
				setProductTruOriginCode(newRowIndex, product.truOriginCode);
				setProductTurnoverSize(newRowIndex, product.turnoverSize);
				setProductUnitCode(newRowIndex, product.unitCode);
				setProductUnitNomenclature(newRowIndex, product.unitNomenclature);
				setProductUnitPrice(newRowIndex, product.unitPrice);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getProductCatalogTruId(int productNum)
		{
			object result = GetDataGrid().Rows[productNum - 1].Cells[(int)column.catalogTruId].Value;
			if (null!= GetDataGrid().Rows[productNum - 1].Cells[(int)column.catalogTruId].Value)
			return GetDataGrid().Rows[productNum - 1].Cells[(int)column.catalogTruId].Value.ToString();
			return "";			
		}

		internal bool setProductCatalogTruId(int productNum, string TRUid)
		{
			try
			{
				GetDataGrid().Rows[productNum-1].Cells[(int)column.catalogTruId].Value = TRUid;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getProductDescription(int productNum)
		{
			if (GetDataGrid().Rows[productNum - 1].Cells[(int)column.nameTRN].Value !=null)
			{
				return GetDataGrid().Rows[productNum - 1].Cells[(int)column.nameTRN].Value.ToString();
			}
			return "";
		}

		internal bool EditProductRow(ProductV2 product)
		{
			try
			{
				int RowIndex = getSelectedRowIndex()+1;
				setProductNumber(RowIndex, RowIndex);
				setProductAdditional(RowIndex, product.additional);
				setProductCatalogTruId(RowIndex, product.catalogTruId);
				setProductDescription(RowIndex, product.description);
				setProductExciseAmount(RowIndex, product.exciseAmount);
				setProductExciseRate(RowIndex, product.exciseRate);
				//panelG.setProductKpvedCode(productCounter, product.kpvedCode);
				setProductNDSAmount(RowIndex, product.ndsAmount);
				setProductNDSRate(RowIndex, product.ndsRate);
				setProductPriceWithTax(RowIndex, product.priceWithTax);
				setProductPriceWithoutTax(RowIndex, product.priceWithoutTax);
				setProductDeclaration(RowIndex, product.productDeclaration);
				setProductNumberInDeclaration(RowIndex, product.productDeclaration);
				setProductQuantity(RowIndex, product.quantity);
				setProductTnvedName(RowIndex, product.tnvedName);
				setProductTruOriginCode(RowIndex, product.truOriginCode);
				setProductTurnoverSize(RowIndex, product.turnoverSize);
				setProductUnitCode(RowIndex, product.unitCode);
				setProductUnitNomenclature(RowIndex, product.unitNomenclature);
				setProductUnitPrice(RowIndex, product.unitPrice);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
			
		}

		internal float getTotalQuantityByProductNumber(int rowNumber)
		{
			return getProductQuantity(rowNumber);
		}

		internal bool setProductDescription(int productNum, string nameTRN)
		{
			try
			{
				GetDataGrid().Rows[productNum-1].Cells[(int)column.nameTRN].Value = nameTRN;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal float getProductExciseAmount(int productNum)
		{
			if (GetDataGrid().Rows[productNum - 1].Cells[(int)column.exciseAmount].Value != null)
			{
				return (float)GetDataGrid().Rows[productNum - 1].Cells[(int)column.exciseAmount].Value;
			}
			return 0;
		}

		internal bool setProductExciseAmount(int productNum, float exciseAmount)
		{
			try
			{
				GetDataGrid().Rows[productNum-1].Cells[(int)column.exciseAmount].Value = exciseAmount;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal float getProductExciseRate(int productNum)
		{
			if (null!= GetDataGrid().Rows[productNum - 1].Cells[(int)column.exciseRate].Value)
			{
				return (float)GetDataGrid().Rows[productNum - 1].Cells[(int)column.exciseRate].Value;				
			}
			return 0;
		}

		internal string getProductKpvedCode(int productNum)
		{
			throw new NotImplementedException();
		}

		internal float getProductNDSAmount(int productNum)
		{
			if(null!= GetDataGrid().Rows[productNum - 1].Cells[(int)column.ndsAmount].Value)
			return (float)GetDataGrid().Rows[productNum - 1].Cells[(int)column.ndsAmount].Value;
			return 0;			
		}

		internal bool setProductNDSAmount(int productNum, float ndsAmount)
		{
			try
			{
				GetDataGrid().Rows[productNum-1].Cells[(int)column.ndsAmount].Value = ndsAmount;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal int getProductNDSRate(int productNum)
		{
			if(null!= GetDataGrid().Rows[productNum - 1].Cells[(int)column.ndsRate].Value)
			return (int)GetDataGrid().Rows[productNum - 1].Cells[(int)column.ndsRate].Value;
			return 0;			
		}

		internal bool setProductNDSRate(int productNum, int ndsRate)
		{
			try
			{
				GetDataGrid().Rows[productNum-1].Cells[(int)column.ndsRate].Value = ndsRate;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal float getProductPriceWithoutTax(int productNum)
		{
			if(null!= GetDataGrid().Rows[productNum - 1].Cells[(int)column.priceWithoutTax].Value)
			return (float)GetDataGrid().Rows[productNum - 1].Cells[(int)column.priceWithoutTax].Value;
			return 0;			
		}

		internal bool setProductPriceWithoutTax(int productNum, float priceWithoutTax)
		{
			try
			{
				GetDataGrid().Rows[productNum-1].Cells[(int)column.priceWithoutTax].Value = priceWithoutTax;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getProductDeclaration(int productNum)
		{
			if (GetDataGrid().Rows[productNum - 1].Cells[(int)column.productDeclaration].Value !=null)
			{
				return GetDataGrid().Rows[productNum - 1].Cells[(int)column.productDeclaration].Value.ToString();
			}				
			return "";
		}

		internal bool setProductDeclaration(int productNum, string declaration)
		{
			try
			{
				GetDataGrid().Rows[productNum-1].Cells[(int)column.productDeclaration].Value = declaration;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal float getProductPriceWithTax(int productNum)
		{
			if(null!= GetDataGrid().Rows[productNum - 1].Cells[(int)column.priceWithTax].Value)
			return (float)GetDataGrid().Rows[productNum - 1].Cells[(int)column.priceWithTax].Value;	
			return 0;		
		}

		internal bool setProductPriceWithTax(int productNum, float priceWithTax)
		{
			try
			{
				GetDataGrid().Rows[productNum-1].Cells[(int)column.priceWithTax].Value = priceWithTax;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getProductNumberInDeclaration(int productNum)
		{
			if(GetDataGrid().Rows[productNum - 1].Cells[(int)column.productNumberInDeclaration].Value != null)
			return GetDataGrid().Rows[productNum - 1].Cells[(int)column.productNumberInDeclaration].Value.ToString();	
			return "";			
		}

		internal bool setProductNumberInDeclaration(int productNum,string num)
		{
			try
			{
				GetDataGrid().Rows[productNum-1].Cells[(int)column.productNumberInDeclaration].Value = num;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getProductTnvedName(int productNum)
		{
			if (GetDataGrid().Rows[productNum - 1].Cells[(int)column.fullnameTRN].Value != null)
			{
				return GetDataGrid().Rows[productNum - 1].Cells[(int)column.fullnameTRN].Value.ToString();
			}			
			return "";			
		}

		internal bool setProductTnvedName(int productNum, string tnvedName)
		{
			try
			{
				GetDataGrid().Rows[productNum-1].Cells[(int)column.fullnameTRN].Value = tnvedName;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal TruOriginCode getProductTruOriginCode(int productNum)
		{
			if (GetDataGrid().Rows[productNum - 1].Cells[(int)column.typeTRN].Value!=null)
			{
				int obj = int.Parse(GetDataGrid().Rows[productNum - 1].Cells[(int)column.typeTRN].Value.ToString());
				return (TruOriginCode)obj;
			}
			return default;
		}

		internal bool setProductTruOriginCode(int productNum, int value)
		{
			try
			{
				DataGridViewTextBoxCell textBoxCell = (DataGridViewTextBoxCell)GetDataGrid().Rows[productNum-1].Cells[(int)column.typeTRN];
				textBoxCell.Value = textBoxCell.Value = value;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal float getProductTurnoverSize(int productNum)
		{
			return (float)GetDataGrid().Rows[productNum-1].Cells[(int)column.turnoverSize].Value;
		}

		internal bool setProductTurnoverSize(int productNum, float turnoverSize)
		{
			try
			{
				GetDataGrid().Rows[productNum-1].Cells[(int)column.turnoverSize].Value = turnoverSize;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getProductUnitCode(int productNum)
		{
			if (GetDataGrid().Rows[productNum - 1].Cells[(int)column.TNVED].Value != null)
			{
				return GetDataGrid().Rows[productNum - 1].Cells[(int)column.TNVED].Value.ToString();
			}
			return "";
		}

		internal bool setProductUnitCode(int productNum, string tnved)
		{
			try
			{
				GetDataGrid().Rows[productNum-1].Cells[(int)column.TNVED].Value = tnved;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal string getProductUnitNominclature(int productNum)
		{
			if(null!= GetDataGrid().Rows[productNum - 1].Cells[(int)column.measure].Value)
			return GetDataGrid().Rows[productNum - 1].Cells[(int)column.measure].Value.ToString();
			return "";			
		}

		internal bool setProductUnitNomenclature(int productNum, string measure)
		{
			try
			{
				GetDataGrid().Rows[productNum-1].Cells[(int)column.measure].Value = measure;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal float getProductUnitPrice(int productNum)
		{
			if (null != GetDataGrid().Rows[productNum - 1].Cells[(int)column.pricePerOne].Value)
			return (float)GetDataGrid().Rows[productNum - 1].Cells[(int)column.pricePerOne].Value;
			return 0;			
		}

		internal bool setProductUnitPrice(int productNum, float unitPrice)
		{
			try
			{
				GetDataGrid().Rows[productNum-1].Cells[(int)column.pricePerOne].Value =unitPrice;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal NdsRateType getProductSetNdsRateType()
		{
			return chbxPartG_withoutNDS.Checked ? NdsRateType.WITHOUT_NDS_NOT_KZ : NdsRateType.WITH_NDS;
		}

		internal bool setProductSetNdsRateType(NdsRateType ndsRateType)
		{
			try
			{
				chbxPartG_withoutNDS.Checked = ndsRateType == NdsRateType.WITHOUT_NDS_NOT_KZ;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal bool setProductNumber(int productNum, int num)
		{
			try
			{
				GetDataGrid().Rows[productNum - 1].Cells[(int)column.rowNumber].Value = num;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal int getProductNumber(int productNum)
		{
			if(null!= GetDataGrid().Rows[productNum - 1].Cells[(int)column.rowNumber].Value)
			return (int)GetDataGrid().Rows[productNum - 1].Cells[(int)column.rowNumber].Value;
			return 0;			
		}

		internal float getProductSetTotalExciseAmount()
		{
			return tbPartG_totalExciseAmount.Text == ""? 0:float.Parse(tbPartG_totalExciseAmount.Text);
		}

		internal bool setProductSetTotalExciseAmount(float amount)
		{
			try
			{
				tbPartG_totalExciseAmount.Text = amount.ToString();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal bool setProductExciseRate(int productNum, float exciseRate)
		{
			try
			{
				GetDataGrid().Rows[productNum-1].Cells[(int)column.exciseRate].Value = exciseRate;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal float getProductSetTotalNDSAmount()
		{
			return tbPartG_totalNdsAmount.Text == ""? 0:float.Parse(tbPartG_totalNdsAmount.Text);
		}

		internal bool setProductSetTotalNDSAmount(float amount)
		{
			try
			{
				tbPartG_totalNdsAmount.Text = amount.ToString();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal bool setProductQuantity(int productNum, float quantity)
		{
			try
			{
				GetDataGrid().Rows[productNum - 1].Cells[(int)column.quantity].Value = quantity;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal float getProductSetTotalPriceWithoutTax()
		{
			return tbPartG_totalPriceWithoutTax.Text  == "" ? 0 : float.Parse(tbPartG_totalPriceWithoutTax.Text);
		}

		internal bool setProductSetTotalPriceWithoutTax(float amount)
		{
			try
			{
				tbPartG_totalPriceWithoutTax.Text = amount.ToString();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal float getProductSetTotalPriceWithTax()
		{
			return tbPartG_totalPriceWithoutTax.Text == "" ? 0 : float.Parse(tbPartG_totalPriceWithoutTax.Text);
		}

		internal bool setProductSetTotalPriceWithTax(float amount)
		{
			try
			{
				tbPartG_totalPriceWithTax.Text = amount.ToString();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal float getProductSetTotalTurnoverSize()
		{
			return tbPartG_totalTurnoverSize.Text == ""?0:float.Parse(tbPartG_totalTurnoverSize.Text);
		}

		internal bool setProductSetTotalTurnoverSize(float amount)
		{
			try
			{
				tbPartG_totalTurnoverSize.Text = amount.ToString();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal float getProductQuantity(int productNum)
		{
			return (float)GetDataGrid().Rows[productNum - 1].Cells[(int)column.quantity].Value;
		}

		internal void clearProductSetCurrencyRate()
		{
			tbPartG_currencyRate.Text = "";
		}

		internal void clearProductSetNdsRateType()
		{
			chbxPartG_withoutNDS.Checked = false;
		}
		private void miAddProduct_Click(object sender, EventArgs e)
		{
			ProductForm productFormAdd = new ProductForm();
			productFormAdd.Show();
		}

		private void miEditProduct_Click(object sender, EventArgs e)
		{
			int selectedRow = getSelectedRowIndex();
			ProductV2 product = SessionDataManagerFacade.getProductV2ByRowNumber(selectedRow+1);
			ProductForm productFormEdit = FormManagerFacade.FillProductFormByProduct(product);
			productFormEdit.Show();
		}

		private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			setSelectedIndexRow(e.RowIndex);
		}

		internal void setSelectedIndexRow(int index)
		{
			if (index>=0)
			selectedRowIndex = index;
			MessageBox.Show(selectedRowIndex.ToString());
		}

		internal int getSelectedRowIndex()
		{
			return selectedRowIndex;
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
			foreach ( DataGridViewRow row in dataGrid.Rows)
			{
				temp = row.Cells["exciseAmount"].Value;
				totalExciseAmount += temp==null?0:(float)temp;
				temp = row.Cells["ndsAmount"].Value;
				totalNdsAmount += temp == null ? 0 : (float)temp;
				temp = row.Cells["priceWithoutTax"].Value;
				totalPriceWithoutTax += temp == null ? 0 : (float)temp;
				temp = row.Cells["priceWithTax"].Value;
				totalPriceWithTax += temp == null ? 0 : (float)temp;
				temp = row.Cells["turnoverSize"].Value;
				totalTurnoverSize += temp == null ? 0 : (float)temp;
			}
			tbPartG_totalExciseAmount.Text = totalExciseAmount.ToString();
			tbPartG_totalNdsAmount.Text = totalNdsAmount.ToString();
			tbPartG_totalPriceWithoutTax.Text = totalPriceWithoutTax.ToString();
			tbPartG_totalPriceWithTax.Text = totalPriceWithTax.ToString();
			tbPartG_totalTurnoverSize.Text = totalTurnoverSize.ToString();
		}

		private void dataGridView1_CellContentClick_2(object sender, DataGridViewCellEventArgs e)
		{

		}


		/*internal ProductV2 getProductByNumber(int productNumber)
		{
			ProductV2 product = new ProductV2();
			foreach (DataGridViewRow row in GetDataGrid().Rows)
			{
				if ((int)row.Cells[(int)column.rowNumber].Value == productNumber)
				{
					product.additional = row.Cells[(int)column.additional].Value;
				}
			} 
			
		}*/
	}
}
