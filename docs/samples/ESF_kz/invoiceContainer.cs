using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ESF_kz
{
	[Serializable]
	[XmlRoot(elementName: "invoiceContainer", Namespace = "esf")]
	public class invoiceContainerV2
	{
		//[XmlArray("invoiceSet")]
		[XmlArrayItem(ElementName ="invoice", Namespace = "v2.esf")]
		public List<InvoiceV2> invoiceSet;

		public invoiceContainerV2() { }
	}
}
