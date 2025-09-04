using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ESF_kz
{
	static class ParsingManager
	{
		static DateTime zeroDate = new DateTime();
		internal static float ParseStringToFLoat(string str)
		{
			return float.Parse(str);
		}

		internal static T ParseStringToEnum<T>(string str) where T: Enum
		{
			return  (T)Enum.Parse(typeof(T), str);
		}

		internal static string getInvoiceBodyString(InvoiceV2 invoice)
		{
			string result;
			XNamespace a = "abstractInvoice.esf";
			XNamespace v2 = "v2.esf";
			XAttribute xA = new XAttribute(XNamespace.Xmlns + "a", a.NamespaceName);
			XAttribute xV2 = new XAttribute(XNamespace.Xmlns + "v2", v2.NamespaceName);
			XElement xInvoice = new XElement(v2 + "invoice", xA, xV2);

			//XElement num = new XElement("num", "123");
			//xInvoice.Add(num);

			foreach (FieldInfo fi in typeof(InvoiceV2).GetFields())
			{
				object value = invoice.GetType().GetField(fi.Name).GetValue(invoice);
				if(value != null)
				{
					switch (fi.FieldType.ToString())
					{
						case "System.String":
							if (value != "")
							{
								XElement stringEl = new XElement(fi.Name, value);
								xInvoice.Add(stringEl);
							}							
							break;
						case "System.DateTime":
							if ((DateTime)value != zeroDate)
							{
								XElement dateEl = new XElement(fi.Name, ((DateTime)value).ToString("dd.MM.yyyy"));
								xInvoice.Add(dateEl);
							}							
							break;
						default:
							Regex objectRegex = new Regex(@"^ESF_kz[.]");							
							bool isObject = objectRegex.IsMatch(fi.FieldType.ToString());
							Regex listRegex = new Regex(@"System[.]Collections[.]Generic[.]List`1[[]ESF_kz[.]\w*[]]$");
							bool isList = listRegex.IsMatch(fi.FieldType.ToString());
							if (isObject)
							{
								Regex typeRegex = new Regex(@".*Type$");
								if (typeRegex.IsMatch(fi.FieldType.ToString()))
								{
									if ((int)value !=0)
									{
										XElement enumEl = new XElement(fi.Name, value.ToString());
										xInvoice.Add(enumEl);
									}									
								}
								else
								{
									var attrs = fi.GetCustomAttributes();
									Match m2 = Regex.Match(fi.FieldType.ToString(), "^ESF_kz[.](.*?)$");
									string tagName = m2.Groups[1].ToString().Replace("V2","");
									tagName = fi.Name;
									xInvoice.Add(getXmlStringByObject(value, tagName));
								}								
							}
							else if (isList)
							{
								string lisTagName = fi.Name;
								string itemTagName = "item";
								foreach (var attr in fi.GetCustomAttributes())
								{
									if(attr.GetType().ToString() == "System.Xml.Serialization.XmlArrayItemAttribute")
									{
										itemTagName = ((XmlArrayItemAttribute)attr).ElementName;
									}
								} 								
								object listEl = getXmlStringByList(value, lisTagName, itemTagName);
								if (listEl != null)
								{
									xInvoice.Add(listEl);
								}								
							}							
							break;
					}
				}	
			}

			result = xInvoice.ToString();
			return result;
		}

		private static object getXmlStringByList(object value, string tagName, string itemTagName)
		{
			XElement listEl = null;

			//Type itemType = value.GetType().GetGenericArguments()[0];

			int count = (int)value.GetType().GetProperty("Count").GetValue(value, null);
			if (count>0)
			{
				listEl = new XElement(tagName);
				//string tag = 
				if (tagName == "statuses")
				{
					for (int i = 0; i < count; i++)
					{
						object[] index = { i };
						object item = value.GetType().GetProperty("Item").GetValue(value, index);
						XElement statusEl = new XElement("status", item);
						listEl.Add(statusEl);
					}
				}
				else
				{
					for (int i = 0; i < count; i++)
					{
						object[] index = { i };
						object item = value.GetType().GetProperty("Item").GetValue(value, index);

						string itemTag = itemTagName;
						listEl.Add(getXmlStringByObject(item, itemTag));
					}
				}					
			}			
			return listEl;
		}

		private static XElement getXmlStringByObject(object value, string tagName)
		{
			List<string> hideIfZeroTagList = getFloatExclusionList();

			XElement classEl = new XElement(tagName);

			foreach  (FieldInfo fi in value.GetType().GetFields())
			{
				object fieldValue = value.GetType().GetField(fi.Name).GetValue(value);
				if (fieldValue !=null)
				{
					switch (fi.FieldType.ToString())
					{
						case "System.String":
							if(fieldValue!="")
							{
								XElement stringEl = new XElement(fi.Name, fieldValue);
								classEl.Add(stringEl);
							}							
							break;
						case "System.DateTime":
							if((DateTime)fieldValue != zeroDate)
							{
								XElement dateEl = new XElement(fi.Name, ((DateTime)fieldValue).ToString("dd.MM.yyyy"));
								classEl.Add(dateEl);
							}							
							break;
						case "System.Single":
							if (!( hideIfZeroTagList.Contains(fi.Name) && (float)fieldValue == 0))
							{
								XElement floatEl = new XElement(fi.Name, fieldValue);
								classEl.Add(floatEl);
							}
							break;
						case "System.Boolean":
							if (!(fi.Name == "isBranchNonResident" && (bool)fieldValue == false))
							{
								XElement boolEl = new XElement(fi.Name, fieldValue);
								classEl.Add(boolEl);
							}						
							break;
						case "System.Int32":
							if (!( hideIfZeroTagList.Contains(fi.Name) && (int)fieldValue == 0))
							{
								XElement intEl = new XElement(fi.Name, fieldValue);
								classEl.Add(intEl);
							}
							break;
						default:
							Regex objectRegex = new Regex(@"^ESF_kz[.]");
							bool isObject = objectRegex.IsMatch(fi.FieldType.ToString());
							Regex listRegex = new Regex(@"System[.]Collections[.]Generic[.]List`1[[]ESF_kz[.]\w*[]]$");
							bool isList = listRegex.IsMatch(fi.FieldType.ToString());
							if (isObject)
							{
								Regex typeRegex = new Regex(@".*(Type)|(Code)$");

								if (typeRegex.IsMatch(fi.FieldType.ToString()))
								{
									
									if ((int)fieldValue != 0)
									{
										XElement enumEl = new XElement(fi.Name, fieldValue.ToString());
										classEl.Add(enumEl);
									}									
								}
								else
								{
									//Match m2 = Regex.Match(fi.FieldType.ToString(), "^ESF_kz[.](.*?)$");
									//string tag = m2.Groups[1].ToString().Replace("V2", "");
									string tag = fi.Name;
									classEl.Add(getXmlStringByObject(fieldValue, tag));
								}								
							}
							else if (isList)
							{
								string lisTagName = fi.Name;
								string itemTagName = "item";
								foreach (var attr in fi.GetCustomAttributes())
								{
									if (attr.GetType().ToString() == "System.Xml.Serialization.XmlArrayItemAttribute")
									{
										itemTagName = ((XmlArrayItemAttribute)attr).ElementName;
									}
								}
								object listEl = getXmlStringByList(fieldValue, lisTagName, itemTagName);
								if (listEl != null)
								{
									classEl.Add(listEl);
								}
							}							
							break;
					}
				}
			}

			return classEl;
		}

		private static List<string> getFloatExclusionList()
		{
			List<string> hideIfZeroTagList = new List<string>();
			hideIfZeroTagList.Add("shareParticipation");
			hideIfZeroTagList.Add("currencyRate");
			hideIfZeroTagList.Add("exciseAmount");
			hideIfZeroTagList.Add("exciseRate");
			hideIfZeroTagList.Add("ndsRate");
			hideIfZeroTagList.Add("quantity");
			hideIfZeroTagList.Add("unitPrice");
			return hideIfZeroTagList;
		}

		internal static invoiceContainerV2 ParseInvoiceConatinerV2XML(XmlDocument xDoc)
		{
			XmlElement xRoot = xDoc.DocumentElement;//invoiceContainer
			invoiceContainerV2 invoiceContainer = new invoiceContainerV2();
			invoiceContainer.invoiceSet = new List<InvoiceV2>();
			XmlNode xInvoiceSet = xRoot.FirstChild;//invoiceSet

			foreach (XmlNode invoice in xInvoiceSet)
			{
				invoiceContainer.invoiceSet.Add(ParseInvoiceBody(invoice));
			}
			return invoiceContainer;
		}

		internal static InvoiceV2 ParseInvoiceBody(XmlNode invoiceXml)
		{
			InvoiceV2 invoiceV2 = new InvoiceV2();
			if (invoiceXml.Name == "v2:invoice" || invoiceXml.Name == "invoice")
			{
				foreach (XmlNode item in invoiceXml)
				{
					if (item.InnerText != "")
					{
						switch (item.Name)
						{
							case "date":
								try
								{
									invoiceV2.date = (DateTime)ParseDate(item.InnerText);
								}
								catch (Exception e)
								{
									LogManagerFacade.ParsingXmlExeption(item.Name, item.InnerText, e);
								}
								break;
							case "invoiceType":
								try
								{
									invoiceV2.invoiceType = (InvoiceType)ParseInvoiceType(item.InnerText);
								}
								catch (Exception e)
								{
									LogManagerFacade.ParsingXmlExeption(item.Name, item.InnerText, e);
								}
								break;
							case "num":
								try
								{
									invoiceV2.num = item.InnerText;
								}
								catch (Exception e)
								{
									LogManagerFacade.ParsingXmlExeption(item.Name, item.InnerText, e);
								}
								break;
							case "operatorFullname":
								try
								{
									invoiceV2.operatorFullname = item.InnerText;
								}
								catch (Exception e)
								{
									LogManagerFacade.ParsingXmlExeption(item.Name, item.InnerText, e);
								}
								break;
							case "relatedInvoice":
								try
								{
									invoiceV2.relatedInvoice = ParseRelatedInvoice(item);
								}
								catch (Exception e)
								{
									LogManagerFacade.ParsingXmlExeption(item.Name, item.InnerText, e);
								}
								break;
							case "turnoverDate":
								try
								{
									invoiceV2.turnoverDate = (DateTime)ParseDate(item.InnerText);
								}
								catch (Exception e)
								{
									LogManagerFacade.ParsingXmlExeption(item.Name, item.InnerText, e);
								}
								break;
							case "addInf":
								try
								{
									invoiceV2.addInf = item.InnerText;
								}
								catch (Exception e)
								{
									LogManagerFacade.ParsingXmlExeption(item.Name, item.InnerText, e);
								}
								break;
							case "consignee":
								try
								{
									invoiceV2.consignee = ParseConsignee(item);
								}
								catch (Exception e)
								{
									LogManagerFacade.ParsingXmlExeption(item.Name, item.InnerText, e);
								}

								break;
							case "consignor":
								try
								{
									invoiceV2.consignor = ParseConsignor(item);
								}
								catch (Exception e)
								{
									LogManagerFacade.ParsingXmlExeption(item.Name, item.InnerText, e);
								}
								break;
							case "customerAgentAddress":
								invoiceV2.customerAgentAddress = item.InnerText;
								break;
							case "customerAgentDocDate":
								try
								{
									invoiceV2.customerAgentDocDate = (DateTime)ParseDate(item.InnerText);
								}
								catch (Exception e)
								{
									LogManagerFacade.ParsingXmlExeption(item.Name, item.InnerText, e);
								}
								break;
							case "customerAgentDocNum":
								invoiceV2.customerAgentDocNum = item.InnerText;
								break;
							case "customerAgentName":
								invoiceV2.customerAgentName = item.InnerText;
								break;
							case "customerAgentTin":
								invoiceV2.customerAgentTin = item.InnerText;
								break;
							case "customerParticipants":
								try
								{
									invoiceV2.customerParticipants = ParseCustomerParticipants(item);
								}
								catch (Exception e)
								{
									LogManagerFacade.ParsingXmlExeption(item.Name, item.InnerText, e);
								}
								break;
							case "customers":
								invoiceV2.customers = ParseCustomers(item);
								break;
							case "datePaper":
								try
								{
									invoiceV2.datePaper = (DateTime)ParseDate(item.InnerText);
								}
								catch (Exception e)
								{
									LogManagerFacade.ParsingXmlExeption(item.Name, item.InnerText, e);
								}
								break;
							case "deliveryDocDate":
								try
								{
									invoiceV2.deliveryDocDate = (DateTime)ParseDate(item.InnerText);
								}
								catch (Exception e)
								{
									LogManagerFacade.ParsingXmlExeption(item.Name, item.InnerText, e);
								}
								break;
							case "deliveryDocNum":
								invoiceV2.deliveryDocNum = item.InnerText;
								break;
							case "deliveryTerm":
								try
								{
									invoiceV2.deliveryTerm = ParseDeliveryTerm(item);
								}
								catch (Exception e)
								{
									LogManagerFacade.ParsingXmlExeption(item.Name, item.InnerText, e);
								}

								break;
							case "productSet":
								try
								{
									invoiceV2.productSet = ParseProductSet(item);
								}
								catch (Exception e)
								{
									LogManagerFacade.ParsingXmlExeption(item.Name, item.InnerText, e);
								}

								break;
							case "publicOffice":
								try
								{
									invoiceV2.publicOffice = ParsePublicOffice(item);
								}
								catch (Exception e)
								{
									LogManagerFacade.ParsingXmlExeption(item.Name, item.InnerText, e);
								}
								break;
							case "reasonPaper":
								invoiceV2.reasonPaper = (PaperReasonType)Enum.Parse(typeof(PaperReasonType), item.InnerText);
								break;
							case "sellerAgentAddress":
								invoiceV2.sellerAgentAddress = item.InnerText;
								break;
							case "sellerAgentDocDate":
								try
								{
									invoiceV2.sellerAgentDocDate = (DateTime)ParseDate(item.InnerText);
								}
								catch (Exception e)
								{
									LogManagerFacade.ParsingXmlExeption(item.Name, item.InnerText, e);
								}
								break;
							case "sellerAgentDocNum":
								invoiceV2.sellerAgentDocNum = item.InnerText;
								break;
							case "sellerAgentName":
								invoiceV2.sellerAgentName = item.InnerText;
								break;
							case "sellerAgentTin":
								invoiceV2.sellerAgentTin = item.InnerText;
								break;
							case "sellerParticipants":
								try
								{
									invoiceV2.sellerParticipants = ParseSellerParticipants(item);
								}
								catch (Exception e)
								{
									LogManagerFacade.ParsingXmlExeption(item.Name, item.InnerText, e);
								}
								break;
							case "sellers":
								try
								{
									invoiceV2.sellers = ParseSellers(item);
								}
								catch (Exception e)
								{
									LogManagerFacade.ParsingXmlExeption(item.Name, item.InnerText, e);
								}
								break;
							default:
								break;
						}
					}
				}
			}
			return invoiceV2;
		}

		private static List<SellerV2> ParseSellers(XmlNode item)
		{
			List<SellerV2> sellers = new List<SellerV2>();
			foreach (XmlNode seller in item)
			{
				SellerV2 sellerV2 = ParseSeller(seller);
				sellers.Add(sellerV2);
			}
			return sellers;
		}


		private static SellerV2 ParseSeller(XmlNode seller)
		{
			SellerV2 sellerV2 = new SellerV2();
			foreach (XmlNode subitem in seller)
			{
				if (subitem.InnerText != "")
				{
					switch (subitem.Name)
					{
						case "address":
							sellerV2.address = subitem.InnerText;
							break;
						case "bank":
							sellerV2.bank = subitem.InnerText;
							break;
						case "bik":
							sellerV2.bik = subitem.InnerText;
							break;
						case "branchTin":
							sellerV2.branchTin = subitem.InnerText;
							break;
						case "certificateNum":
							sellerV2.certificateNum = subitem.InnerText;
							break;
						case "certificateSeries":
							sellerV2.certificateSeries = subitem.InnerText;
							break;
						case "iik":
							sellerV2.iik = subitem.InnerText;
							break;
						case "isBranchNonResident":
							sellerV2.isBranchNonResident = bool.Parse(subitem.InnerText);
							break;
						case "kbe":
							sellerV2.kbe = subitem.InnerText;
							break;
						case "name":
							sellerV2.name = subitem.InnerText;
							break;
						case "reorganizedTin":
							sellerV2.reorganizedTin = subitem.InnerText;
							break;
						case "shareParticipation":
							sellerV2.shareParticipation = float.Parse(subitem.InnerText);
							break;
						case "statuses":
							sellerV2.statuses = ParseSellerStatuses(subitem);
							break;
						case "tin":
							sellerV2.tin = subitem.InnerText;
							break;
						case "trailer":
							sellerV2.trailer = subitem.InnerText;
							break;
						default:
							break;
					}
				}
			}
			return sellerV2;
		}

		private static List<SellerType> ParseSellerStatuses(XmlNode subitem)
		{
			List<SellerType> statuses = new List<SellerType>();
			foreach (XmlNode status in subitem)
			{
				var result = ParseType(status.InnerText, typeof(SellerType));
				if (result != null)
				{
					statuses.Add((SellerType)result);
				}
			}
			return statuses;
		}

		private static object ParseType(string innerText, Type type)
		{
			try
			{
				return Enum.Parse(type, innerText);
			}
			catch (Exception)
			{
				return null;
			}
		}

		private static List<ParticipantV2> ParseSellerParticipants(XmlNode item)
		{
			List<ParticipantV2> sellerParticipants = new List<ParticipantV2>();
			foreach (XmlNode participant in item)
			{
				ParticipantV2 sellerParticipant = ParseSellerParticipant(participant);
				sellerParticipants.Add(sellerParticipant);
			}
			return sellerParticipants;
		}

		private static ParticipantV2 ParseSellerParticipant(XmlNode participant)
		{
			ParticipantV2 sellerParticipant = new ParticipantV2();
			foreach (XmlNode node in participant)
			{
				switch (node.Name)
				{
					case "productShares":
						sellerParticipant.productShares = ParseProductShares(node);
						break;
					case "reorganizedTin":
						sellerParticipant.reorganizedTin = node.InnerText;
						break;
					case "tin":
						sellerParticipant.tin = node.InnerText;
						break;
					default:
						break;
				}
			}
			return sellerParticipant;
		}

		private static PublicOffice ParsePublicOffice(XmlNode item)
		{
			PublicOffice publicOffice = new PublicOffice();
			foreach (XmlNode node in item)
			{
				if (node.InnerText != "")
				{
					switch (node.Name)
					{
						case "bik":
							publicOffice.bik = node.InnerText;
							break;
						case "iik":
							publicOffice.iik = node.InnerText;
							break;
						case "payPurpose":
							publicOffice.payPurpose = node.InnerText;
							break;
						case "productCode":
							publicOffice.productCode = node.InnerText;
							break;
						default:
							break;
					}
				}
			}
			return publicOffice;
		}

		private static ProductSetV2 ParseProductSet(XmlNode item)
		{
			ProductSetV2 productSet = new ProductSetV2();
			foreach (XmlNode node in item)
			{
				if (node.InnerText != "")
				{
					switch (node.Name)
					{
						case "currencyCode":
							productSet.currencyCode = node.InnerText;
							break;
						case "currencyRate":
							productSet.currencyRate = float.Parse(node.InnerText);
							break;
						case "ndsRateType":
							productSet.ndsRateType = (NdsRateType)Enum.Parse(typeof(NdsRateType), node.InnerText);
							break;
						case "products":
							try
							{
								productSet.products = ParseProducts(node);
							}
							catch (Exception e)
							{
								LogManagerFacade.ParsingXmlExeption(node.Name, node.Value, e);
							}
							break;
						case "totalExciseAmount":
							productSet.totalExciseAmount = float.Parse(node.InnerText);
							break;
						case "totalNdsAmount":
							productSet.totalNdsAmount = float.Parse(node.InnerText);
							break;
						case "totalPriceWithTax":
							productSet.totalPriceWithTax = float.Parse(node.InnerText);
							break;
						case "totalPriceWithoutTax":
							productSet.totalPriceWithoutTax = float.Parse(node.InnerText);
							break;
						case "totalTurnoverSize":
							productSet.totalTurnoverSize = float.Parse(node.InnerText);
							break;
						default:
							break;
					}
				}
			}
			return productSet;
		}

		private static List<ProductV2> ParseProducts(XmlNode node)
		{
			List<ProductV2> products = new List<ProductV2>();
			foreach (XmlNode product in node)
			{
				ProductV2 productV2 = ParseProduct(product);
				products.Add(productV2);
			}
			return products;
		}

		private static ProductV2 ParseProduct(XmlNode product)
		{
			ProductV2 productV2 = new ProductV2();
			foreach (XmlNode subnode in product)
			{
				switch (subnode.Name)
				{
					case "additional":
						productV2.additional = subnode.InnerText;
						break;
					case "catalogTruId":
						productV2.catalogTruId = subnode.InnerText;
						break;
					case "description":
						productV2.description = subnode.InnerText;
						break;
					case "exciseAmount":
						productV2.exciseAmount = float.Parse(subnode.InnerText);
						break;
					case "exciseRate":
						productV2.exciseRate = float.Parse(subnode.InnerText);
						break;
					case "kpvedCode":
						productV2.kpvedCode = subnode.InnerText;
						break;
					case "ndsAmount":
						productV2.ndsAmount = float.Parse(subnode.InnerText);
						break;
					case "ndsRate":
						productV2.ndsRate = int.Parse(subnode.InnerText);
						break;
					case "priceWithTax":
						productV2.priceWithTax = float.Parse(subnode.InnerText);
						break;
					case "priceWithoutTax":
						productV2.priceWithoutTax = float.Parse(subnode.InnerText);
						break;
					case "productDeclaration":
						productV2.productDeclaration = subnode.InnerText;
						break;
					case "productNumberInDeclaration":
						productV2.productNumberInDeclaration = subnode.InnerText;
						break;
					case "quantity":
						productV2.quantity = float.Parse(subnode.InnerText);
						break;
					case "tnvedName":
						productV2.tnvedName = subnode.InnerText;
						break;
					case "truOriginCode":
						productV2.truOriginCode = int.Parse(subnode.InnerText);
						break;
					case "turnoverSize":
						productV2.turnoverSize = float.Parse(subnode.InnerText);
						break;
					case "unitCode":
						productV2.unitCode = subnode.InnerText;
						break;
					case "unitNomenclature":
						productV2.unitNomenclature = subnode.InnerText;
						break;
					case "unitPrice":
						productV2.unitPrice = float.Parse(subnode.InnerText);
						break;
					default:
						break;
				}
			}
			return productV2;
		}

		private static DeliveryTermV2 ParseDeliveryTerm(XmlNode item)
		{
			DeliveryTermV2 deliveryTerm = new DeliveryTermV2();
			foreach (XmlNode node in item)
			{
				if (node.InnerText != "")
				{
					switch (node.Name)
					{
						case "contractDate":
							try
							{
								deliveryTerm.contractDate = (DateTime)ParseDate(node.InnerText);
							}
							catch (Exception e)
							{
								LogManagerFacade.ParsingXmlExeption(node.Name, node.InnerText, e);
							}
							break;
						case "contractNum":
							deliveryTerm.contractNum = node.InnerText;
							break;
						case "deliveryConditionCode":
							deliveryTerm.deliveryConditionCode = node.InnerText;
							break;
						case "destination":
							deliveryTerm.destination = node.InnerText;
							break;
						case "hasContract":
							deliveryTerm.hasContract = bool.Parse(node.InnerText);
							break;
						case "term":
							deliveryTerm.term = node.InnerText;
							break;
						case "transportTypeCode":
							deliveryTerm.transportTypeCode = node.InnerText;
							break;
						case "warrant":
							deliveryTerm.warrant = node.InnerText;
							break;
						case "warrantDate":
							try
							{
								deliveryTerm.warrantDate = (DateTime)ParseDate(node.InnerText);
							}
							catch (Exception e)
							{
								LogManagerFacade.ParsingXmlExeption(node.Name, node.InnerText, e);
							}
							break;
						default:
							break;
					}
				}
			}
			return deliveryTerm;
		}

		private static List<CustomerV2> ParseCustomers(XmlNode item)
		{
			List<CustomerV2> customers = new List<CustomerV2>();
			foreach (XmlNode customer in item)
			{
				CustomerV2 customerV2 = ParseCustomer(customer);
				customers.Add(customerV2);
			}
			return customers;
		}

		private static CustomerV2 ParseCustomer(XmlNode customer)
		{
			CustomerV2 customerV2 = new CustomerV2();
			foreach (XmlNode node in customer)
			{
				if (node.InnerText != "")
				{
					switch (node.Name)
					{
						case "address":
							customerV2.address = node.InnerText;
							break;
						case "branchTin":
							customerV2.branchTin = node.InnerText;
							break;
						case "countryCode":
							customerV2.countryCode = node.InnerText;
							break;
						case "name":
							customerV2.name = node.InnerText;
							break;
						case "reorganizedTin":
							customerV2.reorganizedTin = node.InnerText;
							break;
						case "shareParticipation":
							customerV2.shareParticipation = float.Parse(node.InnerText);
							break;
						case "statuses":
							try
							{
								customerV2.statuses = ParseCustomerStatuses(node);
							}
							catch (Exception e)
							{
								LogManagerFacade.ParsingXmlExeption(node.Name, node.InnerText, e);
							}
							break;
						case "tin":
							customerV2.tin = node.InnerText;
							break;
						case "trailer":
							customerV2.trailer = node.InnerText;
							break;
						default:
							break;
					}
				}
			}
			return customerV2;
		}

		private static List<CustomerType> ParseCustomerStatuses(XmlNode node)
		{
			List<CustomerType> statuses = new List<CustomerType>();
			foreach (XmlNode status in node)
			{
				var result = ParseType(status.InnerText, typeof(CustomerType));
				if (result != null)
				{
					statuses.Add((CustomerType)result);
				}
			}
			return statuses;
		}

		private static List<ParticipantV2> ParseCustomerParticipants(XmlNode item)
		{
			List<ParticipantV2> customerParticipants = new List<ParticipantV2>();
			foreach (XmlNode participant in item)
			{
				ParticipantV2 customerParticipant = ParseCustomerParticipant(participant);
				customerParticipants.Add(customerParticipant);
			}
			return customerParticipants;
		}

		private static ParticipantV2 ParseCustomerParticipant(XmlNode participant)
		{
			ParticipantV2 customerParticipant = new ParticipantV2();
			foreach (XmlNode node in participant)
			{
				switch (node.Name)
				{
					case "productShares":
						customerParticipant.productShares = ParseProductShares(node);
						break;
					case "reorganizedTin":
						customerParticipant.reorganizedTin = node.InnerText;
						break;
					case "tin":
						customerParticipant.tin = node.InnerText;
						break;
					default:
						break;
				}
			}
			return customerParticipant;
		}

		private static List<ProductShare> ParseProductShares(XmlNode node)
		{
			List<ProductShare> productShares = new List<ProductShare>();
			foreach (XmlNode share in node)
			{
				ProductShare productShare = ParseProductShare(share);
				productShares.Add(productShare);
			}
			return productShares;
		}

		private static ProductShare ParseProductShare(XmlNode share)
		{
			ProductShare productShare = new ProductShare();
			foreach (XmlNode subnode in share)
			{
				switch (subnode.Name)
				{
					case "additional":
						productShare.additional = subnode.InnerText;
						break;
					case "exciseAmount":
						productShare.exciseAmount = float.Parse(subnode.InnerText);
						break;
					case "ndsAmount":
						productShare.ndsAmount = float.Parse(subnode.InnerText);
						break;
					case "priceWithTax":
						productShare.priceWithTax = float.Parse(subnode.InnerText);
						break;
					case "priceWithoutTax":
						productShare.priceWithoutTax = float.Parse(subnode.InnerText);
						break;
					case "productNumber":
						productShare.productNumber = int.Parse(subnode.InnerText);
						break;
					case "quantity":
						productShare.quantity = float.Parse(subnode.InnerText);
						break;
					case "turnoverSize":
						productShare.turnoverSize = float.Parse(subnode.InnerText);
						break;
					default:
						break;
				}
			}
			return productShare;
		}

		private static Consignor ParseConsignor(XmlNode item)
		{
			Consignor consignor = new Consignor();
			foreach (XmlNode subItem in item)
			{
				if (subItem.InnerText != "")
				{
					switch (subItem.Name)
					{
						case "address":
							consignor.address = subItem.InnerText;
							break;
						case "name":
							consignor.name = subItem.InnerText;
							break;
						case "tin":
							consignor.tin = subItem.InnerText;
							break;
						default:
							break;
					}
				}
			}
			return consignor;
		}

		private static ConsigneeV2 ParseConsignee(XmlNode item)
		{
			ConsigneeV2 consignee = new ConsigneeV2();
			foreach (XmlNode subItem in item)
			{
				if (subItem.InnerText != "")
				{
					switch (subItem.Name)
					{
						case "address":
							consignee.address = subItem.InnerText;
							break;
						case "countryCode":
							consignee.countryCode = subItem.InnerText;
							break;
						case "name":
							consignee.name = subItem.InnerText;
							break;
						case "tin":
							consignee.tin = subItem.InnerText;
							break;
						default:
							break;
					}
				}
			}
			return consignee;
		}

		private static RelatedInvoice ParseRelatedInvoice(XmlNode relInvoice)
		{
			RelatedInvoice relatedInvoice = new RelatedInvoice();
			foreach (XmlNode item in relInvoice)
			{
				if (item.InnerText != "")
				{
					switch (item.Name)
					{
						case "date":
							try
							{
								relatedInvoice.date = (DateTime)ParseDate(item.InnerText);
							}
							catch (Exception e)
							{
								LogManagerFacade.ParsingXmlExeption(item.Name, item.InnerText, e);
							}
							break;
						case "num":
							try
							{
								relatedInvoice.num = item.InnerText;
							}
							catch (Exception e)
							{
								LogManagerFacade.ParsingXmlExeption(item.Name, item.InnerText, e);
							}
							break;
						case "registrationNumber":
							try
							{
								relatedInvoice.registrationNumber = item.InnerText;
							}
							catch (Exception e)
							{
								LogManagerFacade.ParsingXmlExeption(item.Name, item.InnerText, e);
							}
							break;
						default:
							break;
					}
				}
			}
			return relatedInvoice;
		}

		private static object ParseInvoiceType(string innerText)
		{
			if (innerText == InvoiceType.ORDINARY_INVOICE.ToString())
				return InvoiceType.ORDINARY_INVOICE;
			else if (innerText == InvoiceType.ADDITIONAL_INVOICE.ToString())
				return InvoiceType.ADDITIONAL_INVOICE;
			else if (innerText == InvoiceType.FIXED_INVOICE.ToString())
				return InvoiceType.FIXED_INVOICE;
			else
				return null;
		}

		//Parse date in format dd.MM.yyyy
		private static object ParseDate(string innerText)
		{
			try
			{
				string[] tmp = new string[] { };
				int day, month, year;

				tmp = innerText.Split('.');
				day = int.Parse(tmp[0]);
				month = int.Parse(tmp[1]);
				year = int.Parse(tmp[2]);
				return new DateTime(year, month, day);
			}
			catch (Exception)
			{
				return null;
			}

		}
	}
}
