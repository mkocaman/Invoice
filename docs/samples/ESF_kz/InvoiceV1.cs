using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ESF_kz
{
	class InvoiceV1:AbstractInvoice
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

		//Дополнительные сведения (K 43)+
		string addInf;

		//Реквизиты грузополучателя (D 24)+
		ConsigneeV1 consignee;

		//Реквизиты грузоотправителя (D 23)+
		Consignor consignor;

		//Получатели (УСД) (H)+
		List<ParticipantV1> customerParticipants;

		//Получатели (C)+
		List<CustomerV1> customers;

		//Условия поставки (E)+
		DeliveryTermV1 deliveryTerm;

		//Товары(работы, услуги) (G)+
		ProductSetV1 productSet;

		//Реквизиты государственного учреждения (F)+
		PublicOffice publicOffice;

		//Поставщики(УСД) (H)+
		List<ParticipantV1> sellerParticipants;

		//Поставщики (B)+
		List<SellerV1> sellers;
	}
}
