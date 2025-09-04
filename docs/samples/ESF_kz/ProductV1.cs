using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESF_kz
{
	//Товар (работа, услуга)
	class ProductV1
	{
		//Дополнительные данные (G 18)+
		string additional;

		//Номер заявления в рамках ТС (G 16)-
		string applicationNumberInCustomsUnion;

		//Наименование ТРУ (G 3)+
		string description;

		//Акциз-Сумма (G 10)+
		//fractionDigits value="2", totalDigits value="17"
		float exciseAmount;

		//Акциз-Ставка (G 9)+
		//fractionDigits value="2",minInclusive value="0",totalDigits value="14"
		float exciseRate;

		//НДС-Сумма(G 13)+
		//fractionDigits value="2", totalDigits value="17"
		float ndsAmount;

		//НДС-Ставка (G 12)+
		int ndsRate;

		//Стоимость ТРУ с учетом НДС (G 14)+
		//fractionDigits value = "2", totalDigits value="17
		float priceWithTax;

		//Стоимость ТРУ без косвенных налогов (G 8)+
		//fractionDigits value = "2", totalDigits value="17
		float priceWithoutTax;

		//Кол-во (объем) (G 6)+
		//fractionDigits value="6", totalDigits value="18"
		float quantity;

		//Размер оборота по реализации (облагаемый/необлагаемый оборот) (G 11)+
		//fractionDigits value = "2", totalDigits value="17
		float turnoverSize;

		//Код товара(ТНВД ЕАЭС) (G 4)+
		//pattern value="[0-9]{1,10}
		string unitCode;

		//Ед.изм(G 5)+
		string unitNomenclature;

		//Цена (тариф) за единицу ТРУ без косвенных налогов (G 7)+
		//fractionDigits value="6", totalDigits value="18"
		float unitPrice;
		
	}
}
