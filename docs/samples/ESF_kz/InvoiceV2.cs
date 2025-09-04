using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ESF_kz
{
	//ЭСФ
	[Serializable]
	public class InvoiceV2 :AbstractInvoice
	{
		//Дата выписки ЭСФ (A 2)

		[XmlIgnore]
		public DateTime date;

		[XmlElement("date")]
		public string SomeDateString
		{
			get { return this.date.ToString("dd.MM.yyyy"); }
			set { this.date = DateTime.Parse(value); }
		}

		//Тип ЭСФ
		public InvoiceType invoiceType;

		//Исходящий номер ЭСФ в бухгалтерии отправителя
		//pattern value="[0-9]{1,30}
		public string num;

		//ФИО оператора отправившего ЭСФ
		//{0,200}
		public string operatorFullname;

		//Служит для связки исправленного/дополнительного ЭСФ с основным
		public RelatedInvoice relatedInvoice;

		//Дата совершения оборота (A 3)
		[XmlIgnore]
		public DateTime turnoverDate;

		[XmlElement("turnoverDate")]
		public string turnoverDateString
		{
			get { return this.turnoverDate.ToString("dd.MM.yyyy"); }
			set { this.turnoverDate = DateTime.Parse(value); }
		}




		//Дополнительные сведения (K 43
		public string addInf;

		//Реквизиты грузополучателя (D 24)
		[XmlElement(ElementName = "consignee")]
		[MyCustom("consignee")]
		public ConsigneeV2 consignee;

		//Реквизиты грузоотправителя (D 23)
		[XmlElement(ElementName = "consignor")]
		public Consignor consignor;

		//Реквизиты поверенного (оператора) покупателя. Адрес места нахождения (J 41)-
		public string customerAgentAddress;

		//Документ-Дата (J 42.2)-
		[XmlIgnore]
		public DateTime customerAgentDocDate;

		[XmlElement(ElementName = "customerAgentDocDate")]
		public string customerAgentDocDateString
		{			
			get { return this.customerAgentDocDate.ToString("dd.MM.yyyy"); }
			set { this.customerAgentDocDate = DateTime.Parse(value); }			
		}

		//Документ-Номер (J 42.1)-
		public string customerAgentDocNum;

		//Реквизиты поверенного(оператора) покупателя.Поверенный(J 40)-
		public string customerAgentName;

		//Реквизиты поверенного (оператора) покупателя. БИН (J 39)-
		public string customerAgentTin;

		//Получатели (УСД) (H)
		[XmlArrayItem(ElementName = "participant")]
		public List<ParticipantV2> customerParticipants;

		//Получатели (C)
		[XmlArrayItem(ElementName = "customer")]
		public List<CustomerV2> customers;

		//Дата выписки на бумажном носителе (2.1)-
		[XmlIgnore]
		public DateTime datePaper;

		[XmlElement(ElementName = "datePaper")]
		public string datePaperString
		{
			get { return this.datePaper.ToString("dd.MM.yyyy"); }
			set { this.datePaper = DateTime.Parse(value); }
		}

		//Дата документа, подтверждающего поставку товаров (работ, услуг) (F 32.2)-
		[XmlIgnore]
		public DateTime deliveryDocDate;

		[XmlElement(ElementName = "deliveryDocDate")]
		public string deliveryDocDateString
		{
			get { return this.deliveryDocDate.ToString("dd.MM.yyyy"); }
			set { this.deliveryDocDate = DateTime.Parse(value); }
		}

		//Номер документа, подтверждающего поставку товаров (работ, услуг) (F 32.1)-
		public string deliveryDocNum;

		//Условия поставки (E)
		public DeliveryTermV2 deliveryTerm;

		//Товары(работы, услуги) (G)
		public ProductSetV2 productSet;

		//Реквизиты государственного учреждения (F)
		public PublicOffice publicOffice;

		//Причина выписки на бумажном носителе (2.1)-
		public PaperReasonType reasonPaper;

		//Адрес места нахождения(I 37)-
		public string sellerAgentAddress;

		//Документ-Дата (I 38.2)-
		[XmlIgnore]
		public DateTime sellerAgentDocDate;

		[XmlElement(ElementName = "sellerAgentDocDate")]
		public string sellerAgentDocDateString
		{
			get { return this.sellerAgentDocDate.ToString("dd.MM.yyyy"); }
			set { this.sellerAgentDocDate = DateTime.Parse(value); }
		}

		//Документ-Номер (I 38.1)-
		public string sellerAgentDocNum;

		//Поверенный (I 36)-
		public string sellerAgentName;

		//БИН (I 35)-
		public string sellerAgentTin;

		//Поставщики(УСД) (H)
		[XmlArrayItem(ElementName = "participant")]
		public List<ParticipantV2> sellerParticipants;

		//Поставщики (B)
		[XmlArrayItem(ElementName = "seller")]
		public List<SellerV2> sellers;

		public InvoiceV2() { }
	}
}
