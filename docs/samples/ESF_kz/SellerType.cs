using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESF_kz
{
	//Тип поставщика (B 10)
	[Serializable]
	public enum SellerType
	{
		COMMITTENT,//Комитент
		BROKER,//Комиссионер
		FORWARDER,//Экспедитор
		LESSOR,//Лизингодатель
		JOINT_ACTIVITY_PARTICIPANT,//Участник договора совместной деятельности
		SHARING_AGREEMENT_PARTICIPANT,//Участник СРП или сделки, заключенной в рамках СРП
		EXPORTER,//Экспортёр
		TRANSPORTER,//Международный перевозчик
		PRINCIPAL,//Доверитель
	}
}
