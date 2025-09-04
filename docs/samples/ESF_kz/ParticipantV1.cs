using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESF_kz
{
	class ParticipantV1
	{
		//Информация по товарам (работам, услугам)+
		List<ProductShare> productShares;

		//РНН реорганизованного лица-
		string rnn;

		//ИИН/БИН участника совместной деятельности (H 34.1)+
		string tin;
	}
}
