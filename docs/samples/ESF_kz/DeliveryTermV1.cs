using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESF_kz
{
	//Условия поставки (E)
	class DeliveryTermV1
	{
		//Дата договора(контракт) на поставку товаров (работ, услуг) (E 27.4)+
		DateTime contractDate;

		//Номер договора(контракт) на поставку товаров (работ, услуг) (E 27.3)+
		string contractNum;

		//Пункт назначения поставляемых товаров (работ, услуг) (E 31)+
		string destination;

		//Условия оплаты по договору (E 28)+
		string term;

		//Номер доверенности на поставку товаров (работ, услуг) (E 30.1)+
		string warrant;

		//Дата доверенности на поставку товаров(работ, услуг) (E 30.2)+
		DateTime warrantDate;

		//Способ отправления (E 27)-
		string exerciseWay;
	}
}
