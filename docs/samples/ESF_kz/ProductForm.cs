using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ESF_kz.Forms
{
	public partial class ProductForm : Form
	{
		public ProductForm()
		{
			InitializeComponent();
			/*foreach(TruOriginCode code in Enum.GetValues(typeof(TruOriginCode)))
			{
				cbProductTruOriginCode.Items.Add(code.ToString());
			}*/
			FormManagerFacade.setProductForm(this);
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			ProductV2 product = FormManagerFacade.getProductFromProductForm();			
			FormManagerFacade.AddNewProductRow(product);
			//ProductShare share = SessionDataManagerFacade.getProductShare(product);
			FormManagerFacade.AddNewProductShareRow(product);
			FormManagerFacade.RecalcTotalAmounts();
			this.Close();			
		}

		internal ProductV2 getProductFromForm()
		{
			ProductV2 product = new ProductV2();
			
			product.truOriginCode = cbProductTruOriginCode.Text != ""? int.Parse(cbProductTruOriginCode.Text):6;
			product.description = tbProductDescription.Text;
			product.tnvedName = tbProductTnvedName.Text;
			product.unitCode = tbProductUnitCode.Text;
			product.unitNomenclature = tbProductUnitNomenclature.Text;
			product.quantity = float.Parse(tbProductQuantity.Text);
			product.unitPrice = float.Parse(tbProductUnitPrice.Text);
			product.priceWithoutTax = product.quantity * product.unitPrice;		
			product.exciseRate = float.Parse(tbProductExciseRate.Text);
			product.exciseAmount = product.priceWithoutTax * product.exciseRate/100;
			product.turnoverSize = product.priceWithoutTax + product.exciseAmount;
			product.ndsRate = int.Parse(tbProductNdsRate.Text);
			product.ndsAmount = product.turnoverSize * product.ndsRate / 100;
			product.priceWithTax = product.turnoverSize + product.ndsAmount;
			product.productDeclaration = tbProductProductDeclaration.Text;
			product.productNumberInDeclaration = tbProductNumberInDeclaration.Text;
			product.catalogTruId = tbProductCatalogTruId.Text;
			product.additional = tbProductAdditional.Text;
			return product;
		}

		private void tbProductQuantity_Validated(object sender, EventArgs e)
		{
			if(ValidatingManager.ValidateFloatTextBox(tbProductQuantity,epProductFormQuantity) && ValidatingManager.ValidateFloatTextBox(tbProductUnitPrice, epProductUnitPrice))
			{
				FormManagerFacade.FillProductPriceWithoutTax(CalcProductPriceWithoutTax());
				FormManagerFacade.FillProductExciseAmount(CalcProductExciseAmount());
				FormManagerFacade.FillProductTurnoverSize(CalcProductTurnoverSize());
				FormManagerFacade.FillProductNdsAmount(CalcProductNdsAmount());
				FormManagerFacade.FillProductPriceWithTax(CalcProductPriceWithTax());
			}
		}

		private void tbProductUnitPrice_Validated(object sender, EventArgs e)
		{
			if (ValidatingManager.ValidateFloatTextBox(tbProductQuantity, epProductFormQuantity) && ValidatingManager.ValidateFloatTextBox(tbProductUnitPrice, epProductUnitPrice))
			{
				FormManagerFacade.FillProductPriceWithoutTax(CalcProductPriceWithoutTax());
				FormManagerFacade.FillProductExciseAmount(CalcProductExciseAmount());
				FormManagerFacade.FillProductTurnoverSize(CalcProductTurnoverSize());
				FormManagerFacade.FillProductNdsAmount(CalcProductNdsAmount());
				FormManagerFacade.FillProductPriceWithTax(CalcProductPriceWithTax());
			}
		}

		private string CalcProductPriceWithoutTax()
		{
			return (float.Parse(tbProductQuantity.Text) * float.Parse(tbProductUnitPrice.Text)).ToString();
		}

		internal void FillProductPriceWithoutTax(string v)
		{
			tbProductPriceWithoutTax.Text = v;
		}		

		private void tbProductExciseRate_Validated(object sender, EventArgs e)
		{
			if(ValidatingManager.ValidateFloatTextBox(tbProductExciseRate, epProductExciseRate))
			{
				FormManagerFacade.FillProductExciseAmount(CalcProductExciseAmount());
				FormManagerFacade.FillProductTurnoverSize(CalcProductTurnoverSize());
				FormManagerFacade.FillProductNdsAmount(CalcProductNdsAmount());
				FormManagerFacade.FillProductPriceWithTax(CalcProductPriceWithTax());
			}
		}

		private string CalcProductTurnoverSize()
		{
			return (float.Parse(tbProductExciseAmount.Text) + float.Parse(tbProductPriceWithoutTax.Text)).ToString();
		}

		internal void FillProductTurnoverSize(string v)
		{
			tbProductTurnoverSize.Text = v;
		}

		private string CalcProductExciseAmount()
		{
			return (float.Parse(tbProductPriceWithoutTax.Text) * float.Parse(tbProductExciseRate.Text)/100).ToString();
		}

		internal void FillProductExciseAmount(string v)
		{
			tbProductExciseAmount.Text = v;
		}

		private void tbProductNdsRate_Validated(object sender, EventArgs e)
		{
			if(ValidatingManager.ValidateIntegerTextBox(tbProductNdsRate, epProductNdsRate))
			{
				FormManagerFacade.FillProductNdsAmount(CalcProductNdsAmount());
				FormManagerFacade.FillProductPriceWithTax(CalcProductPriceWithTax());
			}
		}

		private string CalcProductPriceWithTax()
		{
			return (float.Parse(tbProductNdsAmount.Text) + float.Parse(tbProductTurnoverSize.Text)).ToString();
		}

		internal void FillProductPriceWithTax(string v)
		{
			tbProductPriceWithTax.Text = v;
		}

		private string CalcProductNdsAmount()
		{
			return (float.Parse(tbProductTurnoverSize.Text) * float.Parse(tbProductNdsRate.Text)/100).ToString();
		}

		internal void FillProductNdsAmount(string v)
		{
			tbProductNdsAmount.Text = v;
		}

		internal void setTruOriginCode(int truOriginCode)
		{
			cbProductTruOriginCode.Text = truOriginCode.ToString();
		}

		internal void setDescription(string description)
		{
			tbProductDescription.Text = description;
		}

		internal void setTnvedName(string tnvedName)
		{
			tbProductTnvedName.Text = tnvedName;
		}

		internal void setUnitCode(string unitCode)
		{
			tbProductUnitCode.Text = unitCode;
		}

		internal void setUnitNomenclature(string unitNomenclature)
		{
			tbProductUnitNomenclature.Text = unitNomenclature;
		}

		internal void setQuantity(float quantity)
		{
			tbProductQuantity.Text = quantity.ToString();
		}

		internal void setUnitPrice(float unitPrice)
		{
			tbProductUnitPrice.Text = unitPrice.ToString();
		}

		internal void setExciseRate(float exciseRate)
		{
			tbProductExciseRate.Text = exciseRate.ToString();
		}

		internal void setNdsRate(int ndsRate)
		{
			tbProductNdsRate.Text = ndsRate.ToString();
		}

		internal void setProductDeclaration(string productDeclaration)
		{
			tbProductProductDeclaration.Text = productDeclaration;
		}

		internal void setProductNumberInDeclaration(string productNumberInDeclaration)
		{
			tbProductNumberInDeclaration.Text = productNumberInDeclaration;
		}

		internal void setCatalogTruId(string catalogTruId)
		{
			tbProductCatalogTruId.Text = catalogTruId;
		}

		internal void setAdditional(string additional)
		{
			tbProductAdditional.Text = additional;
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			ProductV2 product = FormManagerFacade.getProductFromProductForm();
			FormManagerFacade.EditProductRow(product);
			FormManagerFacade.EditProductShareRow(product);
			FormManagerFacade.RecalcTotalAmounts();
			this.Close();
		}

		private void label35_Click(object sender, EventArgs e)
		{
			string info0 = @"- «1» -в случае реализации товара, включенного в Перечень, а также товара, код ТН ВЭД ЕАЭС и наименование которого включен в Перечень,ввезенного на территорию РК с территории государств - членов ЕАЭС;" + "\n";
			string info1 = @"- «2» -в случае реализации товара, не включенного в Перечень, а также товара, код ТН ВЭД ЕАЭС и наименование которого не включен в Перечень, ввезенного на территорию РК из государств - членов ЕАЭС;" + "\n";
			string info2 = @"- «3» -в случае реализации товара, код ТН ВЭД ЕАЭС и наименование которого включен в Перечень, произведенного на территории РК;" + "\n";
			string info3 = @"- «4» -в случае реализации товара, код ТН ВЭД ЕАЭС и наименование которого не включен в Перечень, произведенного на территории РК;" + "\n";
			string info4 = @"- «5» -в случае реализации товара, не относящегося к Признакам «1», «2», «3», «4»;" + "\n";
			string info5 = @"- «6» -в случае выполнения работ, оказания услуг.";
			string title = "Признак происхождения товара";
			InfoForm infoForm = new InfoForm(title, info0 + info1 + info2 + info3 + info4 + info5);
			DialogResult result = infoForm.ShowDialog();
		}

	}
}
