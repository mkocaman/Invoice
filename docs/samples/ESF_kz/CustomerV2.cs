using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ESF_kz
{
	//Реквищиты получателя (C)
	[Serializable]
	public class CustomerV2:AbstractCustomer
	{
		//Адрес(C 18)+
		public string address;

		//БИН филиала, выписавшего ЭСФ за голову
		public string branchTin;

		//Код страны получателя. Обязательно заполняется если установлен статус CustomerType.NONRESIDENT и SellerType.EXPORTER (C 18.1)+
		public string countryCode;

		//Наименование получателя (C 17)+
		public string name;

		//БИН реорганизованного лица (C 16.1)-
		public string reorganizedTin;

		//Доля участия (С 17.1)
		//fractionDigits value="6", totalDigits value="18"
		public float shareParticipation;

		//Категории получателя (С 20)+
		[XmlArrayItem(ElementName = "status")]
		public List<CustomerType> statuses;

		//ИИН/БИН. Может отсутствовать если установлен статус CustomerType.NONRESIDENT (C 16)+
		public string tin;

		//Дополнительные сведения(C 19)+
		public string trailer;

		public CustomerV2() { }
	}
}
