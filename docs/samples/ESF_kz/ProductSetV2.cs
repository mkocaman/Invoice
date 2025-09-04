using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ESF_kz
{
	//Данные по товарам, работам, услугам(G)
	[Serializable]
	public class ProductSetV2
	{
		//Код валюты (G 33.1)
		public string currencyCode;

		//Курс валюты(G 33.2)
		//fractionDigits value="6", totalDigits value="18"
		public float currencyRate;

		//Тип НДС ('Без НДС – не РК')-
		public NdsRateType ndsRateType;

		//Список ТРУ+
		[XmlArrayItem(ElementName = "product")]
		public List<ProductV2> products;

		//Итоговая Акциз-Сумма (G 10)+
		//fractionDigits value="2", totalDigits value="17"
		public float totalExciseAmount;

		//Итоговая НДС-Сумма (G 13)+
		//fractionDigits value="2", totalDigits value="17"
		public float totalNdsAmount;

		//Итоговая стоимость ТРУ с учетом НДС (G 14)+
		//fractionDigits value="2", totalDigits value="17"
		public float totalPriceWithTax;

		//Итоговая стоимость ТРУ без учета НДС (G 8)+
		//fractionDigits value="2", totalDigits value="17"
		public float totalPriceWithoutTax;

		//Итоговый размер оборота по реализации (G 11)+
		//fractionDigits value="2", totalDigits value="17"
		public float totalTurnoverSize;

		public ProductSetV2() { }
	}
}
