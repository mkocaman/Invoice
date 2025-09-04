using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ESF_kz
{
	//Условия поставки (E)
	[Serializable]
	public class DeliveryTermV2
	{
		//Дата договора(контракт) на поставку товаров (работ, услуг) (E 27.4)+
		[XmlIgnore]
		public DateTime contractDate;

		[XmlElement(ElementName = "contractDate")]
		public string contractDateString
		{
			get { return this.contractDate.ToString("dd.MM.yyyy"); }
			set { this.contractDate = DateTime.Parse(value); }
		}



		//Номер договора(контракт) на поставку товаров (работ, услуг) (E 27.3)+
		public string contractNum;

		//Условия поставки(E 31.1)-
		public string deliveryConditionCode;

		//Пункт назначения поставляемых товаров (работ, услуг) (E 31)+
		public string destination;

		//Договор/без договора(E 27.1 - true, E27.2 - false)
		public bool hasContract;

		//Условия оплаты по договору (E 28)+
		public string term;

		//Способ отправления (E 29)
		public string transportTypeCode;

		//Номер доверенности на поставку товаров (работ, услуг) (E 30.1)+
		public string warrant;

		
		//Дата доверенности на поставку товаров(работ, услуг) (E 30.2)+
		[XmlIgnore]
		public DateTime warrantDate;

		[XmlElement (ElementName = "warrantDate")]
		public string warrantDateString
		{
			get { return warrantDate.ToString("dd.MM.yyyy"); }
			set { this.warrantDate = DateTime.Parse(value); }
		}


		public DeliveryTermV2() { }
	}
}
