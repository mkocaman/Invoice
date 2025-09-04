using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESF_kz
{
	//Тип НДС
	[Serializable]
	public enum NdsRateType
	{
		WITH_NDS,
		WITHOUT_NDS_NOT_KZ//Без НДС – не РК
	}
}
