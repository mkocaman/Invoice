using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESF_kz
{
	//Тип получателя (C 22)
	[Serializable]
	public enum CustomerType
	{
		COMMITTENT,//Комитент
		BROKER,//Комиссионер
		LESSEE,//Лизингополучатель
		JOINT_ACTIVITY_PARTICIPANT,//Участник договора совместной деятельности
		PUBLIC_OFFICE,//Государственное учреждение
		NONRESIDENT,//Нерезидент
		SHARING_AGREEMENT_PARTICIPANT,//Участник СРП или сделки, заключенной в рамках СРП
		PRINCIPAL,//Доверитель
		RETAIL,//Розничная реализация
		INDIVIDUAL//Физическое лицо
	}
}
