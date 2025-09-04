using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESF_kz
{
	//Реквизиты грузоотправителя(D 25)
	[Serializable]
	public class Consignor
	{
		//Адрес(D 25.3)
		public string address;

		//Наименование (D 25.2)
		public string name;

		//ИИН/БИН (D 25.1)
		public string tin;

		public Consignor() { }
	}
}
