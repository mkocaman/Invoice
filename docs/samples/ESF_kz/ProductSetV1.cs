using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESF_kz
{
	class ProductSetV1
	{
		//Код валюты (G 33.1)
		string currencyCode;

		//Курс валюты(G 33.2)
		//fractionDigits value="6", totalDigits value="18"
		float currencyRate;

		//Список ТРУ+
		List<ProductV2> products;

		//Итоговая Акциз-Сумма (G 10)+
		//fractionDigits value="2", totalDigits value="17"
		float totalExciseAmount;

		//Итоговая НДС-Сумма (G 13)+
		//fractionDigits value="2", totalDigits value="17"
		float totalNdsAmount;

		//Итоговая стоимость ТРУ с учетом НДС (G 14)+
		//fractionDigits value="2", totalDigits value="17"
		float totalPriceWithTax;

		//Итоговая стоимость ТРУ без учета НДС (G 8)+
		//fractionDigits value="2", totalDigits value="17"
		float totalPriceWithoutTax;

		//Итоговый размер оборота по реализации (G 11)+
		//fractionDigits value="2", totalDigits value="17"
		float totalTurnoverSize;
	}
}
