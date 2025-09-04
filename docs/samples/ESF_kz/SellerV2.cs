using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ESF_kz
{
	//Реквизиты поставщика (B)
	[Serializable]
	public class SellerV2:AbstractCustomer
	{
		//Адрес (B 8)+
		public string address;

		//Наименование банка(B1 15)+
		public string bank;

		//БИК (B1 14)+
		//pattern value="[0-9A-Z]{8}"
		public string bik;

		//БИН филиала, выписавшего ЭСФ за голову-
		public string branchTin;

		//Номер cвидетельства НДС (B 9.2)+
		public string certificateNum;

		//Серия cвидетельства НДС+
		public string certificateSeries;

		//Расчетный счет (B1 13)+
		//pattern value="[0-9A-Z]{20}
		public string iik;

		//Cтруктурное подразделение юридического лица-нерезидента (B 9.3)-
		public bool isBranchNonResident;

		//КБе (B1 12)+
		public string kbe;

		//Наименование поставщика (B 7)+
		public string name;

		//БИН реорганизованного лица (B 6.1)-
		public string reorganizedTin;

		//Доля участия (B 7.1)-
		//fractionDigits value="6", totalDigits value="18"
		public float shareParticipation;

		//Категориu поставщика (B 10)+
		[XmlArrayItem(ElementName = "status")]
		public List<SellerType> statuses;

		//ИИН/БИН (B 6)+
		public string tin;

		//Дополнительные сведения(B 11)+
		public string trailer;

		public SellerV2() { }
	}
}
