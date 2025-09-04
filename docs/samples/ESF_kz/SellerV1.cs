using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESF_kz
{
	//Поставщик(B)
	class SellerV1:AbstractCustomer
	{
		//Адрес (B 8)+
		string address;

		//Банк (B 16)+
		string bank;

		//БИК (B 15)+
		//pattern value="[0-9A-Z]{8}"
		string bik;

		//БИН филиала, выписавшего ЭСФ за голову-
		string branchTin;

		//Номер cвидетельства НДС (B 9.2)+
		string certificateNum;

		//Серия cвидетельства НДС (B 9.1)+
		string certificateSeries;

		//Дата документа, подтверждающего поставку товаров (работ, услуг) (B 11.2)-
		DateTime deliveryDocDate;

		//Номер документа, подтверждающего поставку товаров (работ, услуг) (B 11.1)-
		string deliveryDocNum;

		//Расчетный счет (B14)+
		//pattern value="[0-9A-Z]{20}
		string iik;

		//КБе (B 13)+
		string kbe;

		//Наименование поставщика (B 7)+
		string name;

		//РНН реорганизованного лица (B 6.1)-
		string rnn;

		//Категориu поставщика (B 10)+
		List<SellerType> statuses;

		//ИИН/БИН (B 6)+
		string tin;

		//Дополнительные сведения(B 12)+
		string trailer;
	}
}
