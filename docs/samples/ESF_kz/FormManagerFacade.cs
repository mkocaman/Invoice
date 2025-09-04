using ESF_kz.Forms;
using ESF_kz.SessionService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ESF_kz
{
	static class FormManagerFacade
	{
		static private ESF_form invoiceForm;
		static private UserInfoForm userInfoForm;
		static private ProductForm productForm;

		static internal bool setUserInfoForm(UserInfoForm form)
		{
			try
			{
				userInfoForm = form;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal static bool setProductForm(ProductForm form)
		{
			try
			{
				productForm = form;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal static ProductForm getProductForm()
		{
			return productForm;
		}

		internal static ProductV2 getProductFromProductForm()
		{
			return getProductForm().getProductFromForm();
		}

		internal static bool AddNewProductShareRow(ProductV2 product)
		{
			return invoiceForm.getPannel<panelESFpartH>().AddNewProductRow(product);
		}

		static internal bool setInvoiceForm(ESF_form form)
		{
			try
			{
				invoiceForm = form;
				return true;
			}
			catch (Exception)
			{
				return false;
			}			
		}

		static internal ESF_form getInvoiceForm()
		{
			return invoiceForm;
		}

		static internal UserInfoForm getUserInfoForm()
		{
			return userInfoForm;
		}

		internal static string getOperatorFullname()
		{
			return invoiceForm.getPannel<panelESFpartA>().getOperatorFullname();
		}

		internal static DateTime getRelatedInvoiceDate()
		{
			panelESFpartA panelA = invoiceForm.getPannel<panelESFpartA>();
			if (panelA.isCorrectedESF())
			{
				return panelA.getCorrectedESFDate();
			}
			else if (panelA.isAddedESF())
			{
				return panelA.getAddedESFDate();
			}
			return new DateTime();
		}

		internal static void AddRowByInvoiceInfo(MainForm form, InvoiceService.InvoiceInfo invoiceInfo)
		{
			form.AddRowByInvoiceInfo(invoiceInfo);

			//grid.
		}

		internal static string getRelatedInvoiceNum()
		{
			panelESFpartA panelA = invoiceForm.getPannel<panelESFpartA>();
			if (panelA.isCorrectedESF())
			{
				return panelA.getCorrectedESFNum();
			}
			else if (panelA.isAddedESF())
			{
				return panelA.getAddedESFNum();
			}
			return null;
		}

		internal static bool AddNewProductRow(ProductV2 product)
		{
			return invoiceForm.getPannel<panelESFpartG>().AddNewProductRow(product);
		}

		internal static void FillProductPriceWithoutTax(string v)
		{
			productForm.FillProductPriceWithoutTax(v);
		}

		internal static string getRelatedInvoiceRegistrationNum()
		{
			panelESFpartA panelA = invoiceForm.getPannel<panelESFpartA>();
			if (panelA.isCorrectedESF())
			{
				return panelA.getCorrectedESFRegistrationNum();
			}
			else if (panelA.isAddedESF())
			{
				return panelA.getAddedESFRegistrationNum();
			}
			return null;
		}

		internal static string getInvoiceAddInf()
		{
			return invoiceForm.getPannel<panelESFpartK>().getInvoiceAddInf();
		}

		internal static string getConsigneeAddress()
		{
			return invoiceForm.getPannel<panelESFpartD>().getConsigneeAddress();
		}

		internal static string getConsigneeCountryCode()
		{
			return invoiceForm.getPannel<panelESFpartD>().getConsigneeCountryCode();
		}

		internal static void FillProductExciseAmount(string v)
		{
			productForm.FillProductExciseAmount(v);
		}

		internal static void FillProductTurnoverSize(string v)
		{
			productForm.FillProductTurnoverSize(v);
		}

		internal static string getConsigneeName()
		{
			return invoiceForm.getPannel<panelESFpartD>().getConsigneeName();
		}

		internal static bool FillProfileInfoForm(profileInfo[] profileInfos)
		{
			return getUserInfoForm().fillUserInfoForm(profileInfos);
		}

		internal static string getConsigneeTin()
		{
			return invoiceForm.getPannel<panelESFpartD>().getConsigneeTin();
		}

		internal static bool FillUserInfoForm(User user)
		{
			return getUserInfoForm().fillUserInfoForm(user);
		}

		internal static string getConsignorAddress()
		{
			return invoiceForm.getPannel<panelESFpartD>().getConsignorAddress();
		}

		internal static void FillProductNdsAmount(string v)
		{
			productForm.FillProductNdsAmount(v);
		}

		internal static void FillProductPriceWithTax(string v)
		{
			productForm.FillProductPriceWithTax(v);
		}

		internal static string getConsignorName()
		{
			return invoiceForm.getPannel<panelESFpartD>().getConsignorName();
		}

		internal static void FillNewInvoice()
		{
			FormManagerFacade.setInvoiceDate(DateTime.Now);
			FormManagerFacade.setInvoiceNum(SessionDataManagerFacade.getNewInvoiceNum());///maybe before sending???
			FormManagerFacade.setOperatorFullName(SessionDataManagerFacade.getOperatorFullName());
			FormManagerFacade.highlightTurnoverDate();
			FormManagerFacade.highlightSellerBtn();
			FormManagerFacade.highlightCustomerBtn();
			FormManagerFacade.highlightProductBtn();
			FormManagerFacade.highlightDeliveryBtn();
		}

		internal static void RecalcAmountsFor(int sellerIndex, float shareParicipation)
		{
			invoiceForm.getPannel<panelESFpartH>().getSellerTab(sellerIndex).RecalcAmounts(shareParicipation);
		}

		private static void highlightDeliveryBtn()
		{
			invoiceForm.highlightDeliveryBtn();
		}

		private static void highlightProductBtn()
		{
			invoiceForm.highlightProductBtn();
		}

		private static void highlightCustomerBtn()
		{
			invoiceForm.highlightCustomerBtn();
		}

		private static void highlightSellerBtn()
		{
			invoiceForm.highlightSellerBtn();
		}

		private static void highlightTurnoverDate()
		{
			invoiceForm.getPannel<panelESFpartA>().highLightTurnoverDate();
		}

		internal static float getTotalQuantityByProductNumber(int rowNumber)
		{
			return invoiceForm.getPannel<panelESFpartG>().getTotalQuantityByProductNumber(rowNumber);
		}

		private static bool setOperatorFullName(string v)
		{
			return invoiceForm.getPannel<panelESFpartA>().setOperatorFullname(v);
		}

		internal static string getConsignorTin()
		{
			return invoiceForm.getPannel<panelESFpartD>().getConsignorTin();
		}

		internal static string getCustomerAgentAddress()
		{
			return invoiceForm.getPannel<panelESFpartJ>().getCustomerAgentAddress();
		}

		internal static DateTime getCustomerAgentDocDate()
		{
			return invoiceForm.getPannel<panelESFpartJ>().getCustomerAgentDocDate();
		}

		internal static string getCustomerAgentDocNum()
		{
			return invoiceForm.getPannel<panelESFpartJ>().getCustomerAgentDocNum();
		}

		internal static bool EditProductRow(ProductV2 product)
		{
			return invoiceForm.getPannel<panelESFpartG>().EditProductRow(product);
		}

		internal static string getCustomerAgentName()
		{
			return invoiceForm.getPannel<panelESFpartJ>().getCustomerAgentName();
		}

		internal static string getCustomerAgentTin()
		{
			return invoiceForm.getPannel<panelESFpartJ>().getCustomerAgentTin();
		}

		internal static int getCustomerParticipantsCount()
		{
			return invoiceForm.getPannel<panelESFpartC>().getCustomerParticipantsCount();
		}

		internal static ParticipantV2 getCustomerParticipant(int i)
		{
			throw new NotImplementedException();
		}

		internal static bool EditProductShareRow(ProductV2 product)
		{
			int rowNumber = invoiceForm.getPannel<panelESFpartG>().getSelectedRowIndex()+1;
			return invoiceForm.getPannel<panelESFpartH>().EditProductShareRow(product,rowNumber);
		}

		internal static string getCustomerParticipantTin(int number)
		{
			return invoiceForm.getPannel<panelESFpartC>().getTab(number).getCustomerParticipantsTin();
		}

		internal static string getCustomerParticipantReorgTin(int number)
		{
			return invoiceForm.getPannel<panelESFpartC>().getTab(number).getCustomerParticipantsReorgTin();
		}

		internal static void fillInvoiceForm(invoiceContainerV2 ic)
		{
			if (ic.invoiceSet != null)
			{
				foreach (InvoiceV2 invoice in ic.invoiceSet)
				{
					ESF_form invoiceForm = new ESF_form();
					panelESFpartA panelA = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartA>();
					panelA.setInvoiceDate(invoice.date);
					panelA.setInvoiceNum(invoice.num);

					panelA.setInvoiceType(invoice.invoiceType);
					if (invoice.invoiceType == InvoiceType.ADDITIONAL_INVOICE)
					{
						panelA.setAddedESFRegistrationNum(invoice.relatedInvoice.registrationNumber);
						panelA.setAddedESFNum(invoice.relatedInvoice.num);
						panelA.setAddedESFDate(invoice.relatedInvoice.date);
					}
					else if(invoice.invoiceType == InvoiceType.FIXED_INVOICE)
					{
						panelA.setCorrectedESFRegistrationNum(invoice.relatedInvoice.registrationNumber);
						panelA.setCorrectedESFNum(invoice.relatedInvoice.num);
						panelA.setCorrectedESFDate(invoice.relatedInvoice.date);
					}

					panelA.setOperatorFullname(invoice.operatorFullname);
					panelA.setInvoiceTurnoverDate(invoice.turnoverDate);
					panelA.setInvoiceDatePaper(invoice.datePaper);
					panelA.setReasonPaper(invoice.reasonPaper);

					panelESFpartB panelB = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartB>();
					panelB.RemoveAllTabs();
					panelB.CreateFirstTab("Seller");
					int sellerCounter = 0;
					if (invoice.sellers != null)
					{
						foreach (SellerV2 seller in invoice.sellers)
						{
							sellerCounter++;
							panelB.setSellerParticipantsCount(sellerCounter);
							panelESFpartBtab bTab = panelB.getTab(sellerCounter);
							bTab.setSellerAddress(seller.address);
							bTab.setSellerBank(seller.bank);
							bTab.setSellerBik(seller.bik);
							bTab.setSellerBranchTin(seller.branchTin);
							bTab.setSellerCertificateNum(seller.certificateNum);
							bTab.setSellerCertificateSeries(seller.certificateSeries);
							bTab.setSellerIik(seller.iik);
							bTab.setSellerIsBranchNonResiden(seller.isBranchNonResident);
							bTab.setSellerKbe(seller.kbe);
							bTab.setSellerName(seller.name);
							bTab.setSellerReorgTin(seller.reorganizedTin);
							bTab.setSellerShareParticipation(seller.shareParticipation);
							bTab.setSellerStatuses(seller.statuses);
							bTab.setSellerTin(seller.tin);
							bTab.setSellerTrailer(seller.trailer);
						}
					}				
					

					panelESFpartC panelC = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartC>();
					panelC.RemoveAllTabs();
					panelC.CreateFirstTab("Customer");
					int customerCounter = 0;
					if (invoice.customers !=null)
					{
						foreach (CustomerV2 customer in invoice.customers)
						{
							customerCounter++;
							panelC.setCustomerParticipantsCount(customerCounter);
							panelESFpartCtab cTab = panelC.getTab(customerCounter);
							cTab.setCustomerAddress(customer.address);
							cTab.setCustomerBranchTin(customer.branchTin);
							cTab.setCustomerCountryCode(customer.countryCode);
							cTab.setCustomerName(customer.name);
							cTab.setCustomerRoergTin(customer.reorganizedTin);
							cTab.setCustomerShareParticipation(customer.shareParticipation);
							cTab.setCustomerStatuses(customer.statuses);
							cTab.setCustomerTin(customer.tin);
							cTab.setCustomerTrailer(customer.trailer);
							if (invoice.publicOffice != null)
							{
								cTab.setPublicOfficeBik(invoice.publicOffice.bik);
								cTab.setPublicOfficeIik(invoice.publicOffice.iik);
								cTab.setPublicOfficePayPurpose(invoice.publicOffice.payPurpose);
								cTab.setPublicOfficeProductCode(invoice.publicOffice.productCode);
							}
						}
					}
					

					panelESFpartD panelD = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartD>();
					if (invoice.consignee!=null)
					{
						panelD.setConsigneeAddress(invoice.consignee.address);
						panelD.setConsigneeCountryCode(invoice.consignee.countryCode);
						panelD.setConsigneeName(invoice.consignee.name);
						panelD.setConsigneeTin(invoice.consignee.tin);
					}
					if (invoice.consignor != null) 
					{
						panelD.setConsignorAddress(invoice.consignor.address);
						panelD.setConsignorName(invoice.consignor.name);
						panelD.setConsignorTin(invoice.consignor.tin);
					}
					

					panelESFpartE panelE = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartE>();
					if (invoice.deliveryTerm != null)
					{
						panelE.setDeliveryTermContractDate(invoice.deliveryTerm.contractDate);
						panelE.setDeliveryTermContractNum(invoice.deliveryTerm.contractNum);
						panelE.setDeliveryTermConditiomCode(invoice.deliveryTerm.deliveryConditionCode);
						panelE.setDeliveryTermDestination(invoice.deliveryTerm.destination);
						panelE.setDeliveryTermHasContract(invoice.deliveryTerm.hasContract);
						panelE.setDeliveryTermTerm(invoice.deliveryTerm.term);
						panelE.setDeliveryTermTransportTypeCode(invoice.deliveryTerm.transportTypeCode);
						panelE.setDeliveryTermWarrant(invoice.deliveryTerm.warrant);
						panelE.setDeliveryTermWarrantDate(invoice.deliveryTerm.warrantDate);
					}
					

					panelESFpartF panelF = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartF>();
					panelF.setInvoiceDeliveryDocDate(invoice.deliveryDocDate);
					panelF.setInvoiceDeliveryDocNum(invoice.deliveryDocNum);

					panelESFpartG panelG = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartG>();
					if (invoice.productSet !=null)
					{
						panelG.setProductSetCurrencyRate(invoice.productSet.currencyRate);
						panelG.setProductSetCurrentCode(invoice.productSet.currencyCode);
						panelG.setProductSetNdsRateType(invoice.productSet.ndsRateType);
						int productCounter = 0;
						foreach (ProductV2 product in invoice.productSet.products)
						{
							productCounter++;	
							panelG.GetDataGrid().Rows.Add();															
							panelG.setProductNumber(productCounter, productCounter);
							panelG.setProductAdditional(productCounter, product.additional);
							panelG.setProductCatalogTruId(productCounter, product.catalogTruId);
							panelG.setProductDescription(productCounter, product.description);
							panelG.setProductExciseAmount(productCounter, product.exciseAmount);
							panelG.setProductExciseRate(productCounter, product.exciseRate);
							//panelG.setProductKpvedCode(productCounter, product.kpvedCode);
							panelG.setProductNDSAmount(productCounter, product.ndsAmount);
							panelG.setProductNDSRate(productCounter, product.ndsRate);
							panelG.setProductPriceWithTax(productCounter, product.priceWithTax);
							panelG.setProductPriceWithoutTax(productCounter, product.priceWithoutTax);
							panelG.setProductDeclaration(productCounter, product.productDeclaration);
							panelG.setProductNumberInDeclaration(productCounter, product.productDeclaration);
							panelG.setProductQuantity(productCounter, product.quantity);
							panelG.setProductTnvedName(productCounter, product.tnvedName);
							panelG.setProductTruOriginCode(productCounter, product.truOriginCode);
							panelG.setProductTurnoverSize(productCounter, product.turnoverSize);
							panelG.setProductUnitCode(productCounter, product.unitCode);
							panelG.setProductUnitNomenclature(productCounter, product.unitNomenclature);
							panelG.setProductUnitPrice(productCounter, product.unitPrice);
						}
						panelG.setProductSetTotalExciseAmount(invoice.productSet.totalExciseAmount);
						panelG.setProductSetTotalNDSAmount(invoice.productSet.totalNdsAmount);
						panelG.setProductSetTotalPriceWithTax(invoice.productSet.totalPriceWithTax);
						panelG.setProductSetTotalPriceWithoutTax(invoice.productSet.totalPriceWithoutTax);
						panelG.setProductSetTotalTurnoverSize(invoice.productSet.totalTurnoverSize);						
					}
					

					panelESFpartH panelH = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartH>();
					panelH.RemoveAllTabs();
					int sellerParticipantCounter = 0;
					int customerParticipantCounter = 0;
					if (invoice.sellerParticipants !=null)
					{
						foreach (ParticipantV2 participant in invoice.sellerParticipants)
						{
							sellerParticipantCounter++;
							panelH.CreateSellerTab(" Participant  #" + sellerParticipantCounter);
							panelESFpartHtab hTab = panelH.getSellerTab(sellerParticipantCounter);
							int shareCounter = 0;
							foreach (ProductShare share in participant.productShares)
							{
								shareCounter++;
								hTab.GetDataGrid().Rows.Add();							
								hTab.setProductShareAdditional(shareCounter, share.additional);
								hTab.setProductShareExciseAmount(shareCounter, share.exciseAmount);
								hTab.setProductShareNDSAmount(shareCounter, share.ndsAmount);
								hTab.setProductSharePriceWithTax(shareCounter, share.priceWithTax);
								hTab.setProductSharePriceWithoutTax(shareCounter, share.priceWithoutTax);
								hTab.setShareProductNumber(shareCounter, share.productNumber);
								hTab.setProductShareQuantity(shareCounter, share.quantity);
								hTab.setProductShareTurnoverSize(shareCounter, share.turnoverSize);

								ProductV2 product = invoice.productSet.products[share.productNumber - 1];
								hTab.setProductShareDescription(shareCounter, product.description);
								hTab.setProductShareExciseRate(shareCounter, product.exciseRate);
								hTab.setProductShareNdsRate(shareCounter, product.ndsRate);
								hTab.setProductShareProductDeclaration(shareCounter, product.productDeclaration);
								hTab.setProductShareProductNumberInDeclaration(shareCounter, product.productNumberInDeclaration);
								hTab.setProductShareTnvedName(shareCounter, product.tnvedName);
								hTab.setProductShareTruOriginCode(shareCounter, product.truOriginCode);
								hTab.setProductShareUnitCode(shareCounter, product.unitCode);
								hTab.setProductShareUnitNomenclature(shareCounter, product.unitNomenclature);
								hTab.setProductShareUnitPrice(shareCounter, product.unitPrice);
								hTab.setProductShareCatalogTruId(shareCounter, product.catalogTruId);
							}
							hTab.setParticipantTin(participant.tin);
							hTab.setParticipantReorganizedTin(participant.reorganizedTin);
						}
					}

					if(invoice.customerParticipants!=null)
					{
						foreach (ParticipantV2 participant in invoice.customerParticipants)
						{
							customerParticipantCounter++;
							panelH.CreateCustomerTab(" Participant  #" + customerParticipantCounter);
							panelESFpartHtab hTab = panelH.getCustomerTab(customerParticipantCounter);
							int shareCounter = 0;
							foreach (ProductShare share in participant.productShares)
							{
								shareCounter++;
								hTab.GetDataGrid().Rows.Add();
								hTab.setProductShareAdditional(shareCounter, share.additional);
								hTab.setProductShareExciseAmount(shareCounter, share.exciseAmount);
								hTab.setProductShareNDSAmount(shareCounter, share.ndsAmount);
								hTab.setProductSharePriceWithTax(shareCounter, share.priceWithTax);
								hTab.setProductSharePriceWithoutTax(shareCounter, share.priceWithoutTax);
								hTab.setShareProductNumber(shareCounter, share.productNumber);
								hTab.setProductShareQuantity(shareCounter, share.quantity);
								hTab.setProductShareTurnoverSize(shareCounter, share.turnoverSize);

								ProductV2 product = invoice.productSet.products[share.productNumber - 1];
								hTab.setProductShareDescription(shareCounter, product.description);
								hTab.setProductShareExciseRate(shareCounter, product.exciseRate);
								hTab.setProductShareNdsRate(shareCounter, product.ndsRate);
								hTab.setProductShareProductDeclaration(shareCounter, product.productDeclaration);
								hTab.setProductShareProductNumberInDeclaration(shareCounter, product.productNumberInDeclaration);
								hTab.setProductShareTnvedName(shareCounter, product.tnvedName);
								hTab.setProductShareTruOriginCode(shareCounter, product.truOriginCode);
								hTab.setProductShareUnitCode(shareCounter, product.unitCode);
								hTab.setProductShareUnitNomenclature(shareCounter, product.unitNomenclature);
								hTab.setProductShareUnitPrice(shareCounter, product.unitPrice);
								hTab.setProductShareCatalogTruId(shareCounter, product.catalogTruId);
							}
							hTab.setParticipantTin(participant.tin);
							hTab.setParticipantReorganizedTin(participant.reorganizedTin);
						}
					}		

					panelESFpartI panelI = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartI>();
					panelI.setInvoiceSellerAgentAddress(invoice.sellerAgentAddress);
					panelI.setInvoiceSellerAgentDocDate(invoice.sellerAgentDocDate);
					panelI.setInvoiceSellerAgentDocNum(invoice.sellerAgentDocNum);
					panelI.setInvoiceSellerAgentName(invoice.sellerAgentName);
					panelI.setInvoiceSellerAgentTin(invoice.sellerAgentTin);

					panelESFpartJ panelJ= FormManagerFacade.getInvoiceForm().getPannel<panelESFpartJ>();
					panelJ.setCustomerAgentAddress(invoice.customerAgentAddress);
					panelJ.setCustomerAgentDocDate(invoice.customerAgentDocDate);
					panelJ.setCustomerAgentDocNum(invoice.customerAgentDocNum);
					panelJ.setCustomerAgentName(invoice.customerAgentName);
					panelJ.setCustomerAgentTin(invoice.customerAgentTin);

					panelESFpartK panelK = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartK>();
					panelK.setInvoiceAddInf(invoice.addInf);

					panelESFpartL panelL = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartL>();
				}

				
			}
		}

		internal static void RecalcTotalAmounts()
		{
			invoiceForm.getPannel<panelESFpartG>().RecalcTotalAmounts();
			invoiceForm.getPannel<panelESFpartH>().RecalcTotalAmounts();
		}

		internal static ProductForm FillProductFormByProduct(ProductV2 product)
		{
			ProductForm productForm = new ProductForm();
			productForm.setTruOriginCode(product.truOriginCode);
			productForm.setDescription(product.description);
			productForm.setTnvedName(product.tnvedName);
			productForm.setUnitCode(product.unitCode);
			productForm.setUnitNomenclature(product.unitNomenclature);
			productForm.setQuantity(product.quantity);
			productForm.setUnitPrice(product.unitPrice);
			productForm.setExciseRate(product.exciseRate);
			productForm.setNdsRate(product.ndsRate);
			productForm.setProductDeclaration(product.productDeclaration);
			productForm.setProductNumberInDeclaration(product.productNumberInDeclaration);
			productForm.setCatalogTruId(product.catalogTruId);
			productForm.setAdditional(product.additional);
			return productForm;
		}

		internal static ESF_form FillInvoiceFormByInvoice(InvoiceV2 invoice)
		{			
			ESF_form invoiceForm = new ESF_form();
			panelESFpartA panelA = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartA>();
			DateTime zeroDate = new DateTime();

			if (invoice.date != zeroDate)			
				panelA.setInvoiceDate(invoice.date);		
			panelA.setInvoiceNum(invoice.num);

			panelA.setInvoiceType(invoice.invoiceType);
			if (invoice.invoiceType == InvoiceType.ADDITIONAL_INVOICE)
			{
				panelA.setAddedESFRegistrationNum(invoice.relatedInvoice.registrationNumber);
				panelA.setAddedESFNum(invoice.relatedInvoice.num);
				if(invoice.relatedInvoice.date!=zeroDate)
					panelA.setAddedESFDate(invoice.relatedInvoice.date);
			}
			else if (invoice.invoiceType == InvoiceType.FIXED_INVOICE)
			{
				panelA.setCorrectedESFRegistrationNum(invoice.relatedInvoice.registrationNumber);
				panelA.setCorrectedESFNum(invoice.relatedInvoice.num);
				if (invoice.relatedInvoice.date != zeroDate)
					panelA.setCorrectedESFDate(invoice.relatedInvoice.date);
			}

			panelA.setOperatorFullname(invoice.operatorFullname);
			if (invoice.turnoverDate != zeroDate)
				panelA.setInvoiceTurnoverDate(invoice.turnoverDate);
			if (invoice.datePaper != zeroDate)
			{
				panelA.setInvoiceDatePaper(invoice.datePaper);
				panelA.setReasonPaper(invoice.reasonPaper);
			}				

			panelESFpartB panelB = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartB>();
			panelB.RemoveAllTabs();
			panelB.CreateFirstTab("Seller");
			int sellerCounter = 0;
			if (invoice.sellers != null)
			{
				foreach (SellerV2 seller in invoice.sellers)
				{
					sellerCounter++;
					panelB.setSellerParticipantsCount(sellerCounter);
					panelESFpartBtab bTab = panelB.getTab(sellerCounter);
					bTab.setSellerAddress(seller.address);
					bTab.setSellerBank(seller.bank);
					bTab.setSellerBik(seller.bik);
					bTab.setSellerBranchTin(seller.branchTin);
					bTab.setSellerCertificateNum(seller.certificateNum);
					bTab.setSellerCertificateSeries(seller.certificateSeries);
					bTab.setSellerIik(seller.iik);
					bTab.setSellerIsBranchNonResiden(seller.isBranchNonResident);
					bTab.setSellerKbe(seller.kbe);
					bTab.setSellerName(seller.name);
					bTab.setSellerReorgTin(seller.reorganizedTin);
					if(seller.shareParticipation!=0)
						bTab.setSellerShareParticipation(seller.shareParticipation);
					bTab.setSellerStatuses(seller.statuses);
					bTab.setSellerTin(seller.tin);
					bTab.setSellerTrailer(seller.trailer);
				}
			}


			panelESFpartC panelC = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartC>();
			panelC.RemoveAllTabs();
			panelC.CreateFirstTab("Customer");
			int customerCounter = 0;
			if (invoice.customers != null)
			{
				foreach (CustomerV2 customer in invoice.customers)
				{
					customerCounter++;
					panelC.setCustomerParticipantsCount(customerCounter);
					panelESFpartCtab cTab = panelC.getTab(customerCounter);
					cTab.setCustomerAddress(customer.address);
					cTab.setCustomerBranchTin(customer.branchTin);
					cTab.setCustomerCountryCode(customer.countryCode);
					cTab.setCustomerName(customer.name);
					cTab.setCustomerRoergTin(customer.reorganizedTin);
					if (customer.shareParticipation != 0)
						cTab.setCustomerShareParticipation(customer.shareParticipation);
					cTab.setCustomerStatuses(customer.statuses);
					cTab.setCustomerTin(customer.tin);
					cTab.setCustomerTrailer(customer.trailer);
					if (invoice.publicOffice != null)
					{
						cTab.setPublicOfficeBik(invoice.publicOffice.bik);
						cTab.setPublicOfficeIik(invoice.publicOffice.iik);
						cTab.setPublicOfficePayPurpose(invoice.publicOffice.payPurpose);
						cTab.setPublicOfficeProductCode(invoice.publicOffice.productCode);
					}
				}
			}


			panelESFpartD panelD = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartD>();
			if (invoice.consignee != null)
			{
				panelD.setConsigneeAddress(invoice.consignee.address);
				panelD.setConsigneeCountryCode(invoice.consignee.countryCode);
				panelD.setConsigneeName(invoice.consignee.name);
				panelD.setConsigneeTin(invoice.consignee.tin);
			}
			if (invoice.consignor != null)
			{
				panelD.setConsignorAddress(invoice.consignor.address);
				panelD.setConsignorName(invoice.consignor.name);
				panelD.setConsignorTin(invoice.consignor.tin);
			}


			panelESFpartE panelE = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartE>();
			if (invoice.deliveryTerm != null)
			{
				if (invoice.deliveryTerm.contractDate != zeroDate)
					panelE.setDeliveryTermContractDate(invoice.deliveryTerm.contractDate);
				panelE.setDeliveryTermContractNum(invoice.deliveryTerm.contractNum);
				panelE.setDeliveryTermConditiomCode(invoice.deliveryTerm.deliveryConditionCode);
				panelE.setDeliveryTermDestination(invoice.deliveryTerm.destination);
				panelE.setDeliveryTermHasContract(invoice.deliveryTerm.hasContract);
				panelE.setDeliveryTermTerm(invoice.deliveryTerm.term);
				panelE.setDeliveryTermTransportTypeCode(invoice.deliveryTerm.transportTypeCode);
				panelE.setDeliveryTermWarrant(invoice.deliveryTerm.warrant);
				if(invoice.deliveryTerm.warrantDate != zeroDate)
					panelE.setDeliveryTermWarrantDate(invoice.deliveryTerm.warrantDate);
			}


			panelESFpartF panelF = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartF>();
			if (invoice.deliveryDocDate != zeroDate)
				panelF.setInvoiceDeliveryDocDate(invoice.deliveryDocDate);
			panelF.setInvoiceDeliveryDocNum(invoice.deliveryDocNum);

			panelESFpartG panelG = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartG>();
			if (invoice.productSet != null)
			{
				panelG.setProductSetCurrencyRate(invoice.productSet.currencyRate);
				panelG.setProductSetCurrentCode(invoice.productSet.currencyCode);
				panelG.setProductSetNdsRateType(invoice.productSet.ndsRateType);
				int productCounter = 0;
				foreach (ProductV2 product in invoice.productSet.products)
				{
					productCounter++;
					panelG.GetDataGrid().Rows.Add();
					panelG.setProductNumber(productCounter, productCounter);
					panelG.setProductAdditional(productCounter, product.additional);
					panelG.setProductCatalogTruId(productCounter, product.catalogTruId);
					panelG.setProductDescription(productCounter, product.description);
					panelG.setProductExciseAmount(productCounter, product.exciseAmount);
					panelG.setProductExciseRate(productCounter, product.exciseRate);
					//panelG.setProductKpvedCode(productCounter, product.kpvedCode);
					panelG.setProductNDSAmount(productCounter, product.ndsAmount);
					panelG.setProductNDSRate(productCounter, product.ndsRate);
					panelG.setProductPriceWithTax(productCounter, product.priceWithTax);
					panelG.setProductPriceWithoutTax(productCounter, product.priceWithoutTax);
					panelG.setProductDeclaration(productCounter, product.productDeclaration);
					panelG.setProductNumberInDeclaration(productCounter, product.productDeclaration);
					panelG.setProductQuantity(productCounter, product.quantity);
					panelG.setProductTnvedName(productCounter, product.tnvedName);
					panelG.setProductTruOriginCode(productCounter, product.truOriginCode);
					panelG.setProductTurnoverSize(productCounter, product.turnoverSize);
					panelG.setProductUnitCode(productCounter, product.unitCode);
					panelG.setProductUnitNomenclature(productCounter, product.unitNomenclature);
					panelG.setProductUnitPrice(productCounter, product.unitPrice);
				}
				panelG.setProductSetTotalExciseAmount(invoice.productSet.totalExciseAmount);
				panelG.setProductSetTotalNDSAmount(invoice.productSet.totalNdsAmount);
				panelG.setProductSetTotalPriceWithTax(invoice.productSet.totalPriceWithTax);
				panelG.setProductSetTotalPriceWithoutTax(invoice.productSet.totalPriceWithoutTax);
				panelG.setProductSetTotalTurnoverSize(invoice.productSet.totalTurnoverSize);
			}


			panelESFpartH panelH = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartH>();
			panelH.RemoveAllTabs();
			int sellerParticipantCounter = 0;
			int customerParticipantCounter = 0;
			if (invoice.sellerParticipants != null)
			{
				foreach (ParticipantV2 participant in invoice.sellerParticipants)
				{
					sellerParticipantCounter++;
					panelH.CreateSellerTab(" Participant  #" + sellerParticipantCounter);
					panelESFpartHtab hTab = panelH.getSellerTab(sellerParticipantCounter);
					int shareCounter = 0;
					foreach (ProductShare share in participant.productShares)
					{
						shareCounter++;
						hTab.GetDataGrid().Rows.Add();
						hTab.setProductShareAdditional(shareCounter, share.additional);
						hTab.setProductShareExciseAmount(shareCounter, share.exciseAmount);
						hTab.setProductShareNDSAmount(shareCounter, share.ndsAmount);
						hTab.setProductSharePriceWithTax(shareCounter, share.priceWithTax);
						hTab.setProductSharePriceWithoutTax(shareCounter, share.priceWithoutTax);
						hTab.setShareProductNumber(shareCounter, share.productNumber);
						hTab.setProductShareQuantity(shareCounter, share.quantity);
						hTab.setProductShareTurnoverSize(shareCounter, share.turnoverSize);

						ProductV2 product = invoice.productSet.products[share.productNumber - 1];
						hTab.setProductShareDescription(shareCounter, product.description);
						hTab.setProductShareExciseRate(shareCounter, product.exciseRate);
						hTab.setProductShareNdsRate(shareCounter, product.ndsRate);
						hTab.setProductShareProductDeclaration(shareCounter, product.productDeclaration);
						hTab.setProductShareProductNumberInDeclaration(shareCounter, product.productNumberInDeclaration);
						hTab.setProductShareTnvedName(shareCounter, product.tnvedName);
						hTab.setProductShareTruOriginCode(shareCounter, product.truOriginCode);
						hTab.setProductShareUnitCode(shareCounter, product.unitCode);
						hTab.setProductShareUnitNomenclature(shareCounter, product.unitNomenclature);
						hTab.setProductShareUnitPrice(shareCounter, product.unitPrice);
						hTab.setProductShareCatalogTruId(shareCounter, product.catalogTruId);
					}
					hTab.setParticipantTin(participant.tin);
					hTab.setParticipantReorganizedTin(participant.reorganizedTin);
				}
			}

			if (invoice.customerParticipants != null)
			{
				foreach (ParticipantV2 participant in invoice.customerParticipants)
				{
					customerParticipantCounter++;
					panelH.CreateCustomerTab(" Participant  #" + customerParticipantCounter);
					panelESFpartHtab hTab = panelH.getCustomerTab(customerParticipantCounter);
					int shareCounter = 0;
					foreach (ProductShare share in participant.productShares)
					{
						shareCounter++;
						hTab.GetDataGrid().Rows.Add();
						hTab.setProductShareAdditional(shareCounter, share.additional);
						hTab.setProductShareExciseAmount(shareCounter, share.exciseAmount);
						hTab.setProductShareNDSAmount(shareCounter, share.ndsAmount);
						hTab.setProductSharePriceWithTax(shareCounter, share.priceWithTax);
						hTab.setProductSharePriceWithoutTax(shareCounter, share.priceWithoutTax);
						hTab.setShareProductNumber(shareCounter, share.productNumber);
						hTab.setProductShareQuantity(shareCounter, share.quantity);
						hTab.setProductShareTurnoverSize(shareCounter, share.turnoverSize);

						ProductV2 product = invoice.productSet.products[share.productNumber - 1];
						hTab.setProductShareDescription(shareCounter, product.description);
						hTab.setProductShareExciseRate(shareCounter, product.exciseRate);
						hTab.setProductShareNdsRate(shareCounter, product.ndsRate);
						hTab.setProductShareProductDeclaration(shareCounter, product.productDeclaration);
						hTab.setProductShareProductNumberInDeclaration(shareCounter, product.productNumberInDeclaration);
						hTab.setProductShareTnvedName(shareCounter, product.tnvedName);
						hTab.setProductShareTruOriginCode(shareCounter, product.truOriginCode);
						hTab.setProductShareUnitCode(shareCounter, product.unitCode);
						hTab.setProductShareUnitNomenclature(shareCounter, product.unitNomenclature);
						hTab.setProductShareUnitPrice(shareCounter, product.unitPrice);
						hTab.setProductShareCatalogTruId(shareCounter, product.catalogTruId);
					}
					hTab.setParticipantTin(participant.tin);
					hTab.setParticipantReorganizedTin(participant.reorganizedTin);
				}
			}

			panelESFpartI panelI = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartI>();
			panelI.setInvoiceSellerAgentAddress(invoice.sellerAgentAddress);
			panelI.setInvoiceSellerAgentDocDate(invoice.sellerAgentDocDate);
			panelI.setInvoiceSellerAgentDocNum(invoice.sellerAgentDocNum);
			panelI.setInvoiceSellerAgentName(invoice.sellerAgentName);
			panelI.setInvoiceSellerAgentTin(invoice.sellerAgentTin);

			panelESFpartJ panelJ = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartJ>();
			panelJ.setCustomerAgentAddress(invoice.customerAgentAddress);
			panelJ.setCustomerAgentDocDate(invoice.customerAgentDocDate);
			panelJ.setCustomerAgentDocNum(invoice.customerAgentDocNum);
			panelJ.setCustomerAgentName(invoice.customerAgentName);
			panelJ.setCustomerAgentTin(invoice.customerAgentTin);

			panelESFpartK panelK = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartK>();
			panelK.setInvoiceAddInf(invoice.addInf);

			panelESFpartL panelL = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartL>();

			return invoiceForm;
		}

		internal static int getShareByCustomerParticipantCount(int customerNumber)
		{
			return invoiceForm.getPannel<panelESFpartH>().getCustomerTab(customerNumber).GetDataGrid().Rows.Count;
		}

		internal static string getCustomerProductShareAdditional(int participantNumber, int productShareNumber)
		{
			return invoiceForm.getPannel<panelESFpartH>().getCustomerTab(participantNumber).getProductShareAdditional(productShareNumber);
		}

		internal static float getCustomerProductShareExciseAmount(int participantNumber, int productShareNumber)
		{
			return invoiceForm.getPannel<panelESFpartH>().getCustomerTab(participantNumber).getProductShareExciseAmount(productShareNumber);
		}

		internal static float getCustomerProductShareNDSAmount(int participantNumber, int productShareNumber)
		{
			return invoiceForm.getPannel<panelESFpartH>().getCustomerTab(participantNumber).getProductShareNDSAmount(productShareNumber);
		}

		internal static float getCustomerProductSharePriceWithoutTax(int participantNumber, int productShareNumber)
		{
			return invoiceForm.getPannel<panelESFpartH>().getCustomerTab(participantNumber).getProductSharePriceWithoutTax(productShareNumber);
		}

		internal static float getCustomerProductSharePriceWithhTax(int participantNumber, int productShareNumber)
		{
			return invoiceForm.getPannel<panelESFpartH>().getCustomerTab(participantNumber).getProductSharePriceWithTax(productShareNumber);
		}

		internal static int getCustomerProductShareProductNumber(int participantNumber, int productShareNumber)
		{
			return invoiceForm.getPannel<panelESFpartH>().getCustomerTab(participantNumber).getShareProductNumber(productShareNumber);
		}

		internal static int getSellerProductShareProductNumber(int sellerNumber, int productSharNum)
		{
			return invoiceForm.getPannel<panelESFpartH>().getSellerTab(sellerNumber).getShareProductNumber(productSharNum);
		}

		internal static float getCustomerProductShareQuantity(int participantNumber, int productShareNumber)
		{
			return invoiceForm.getPannel<panelESFpartH>().getCustomerTab(participantNumber).getProductShareQuantity(productShareNumber);
		}

		internal static float getCustomerProductShareTurnoverSize(int participantNumber, int productShareNumber)
		{
			return invoiceForm.getPannel<panelESFpartH>().getCustomerTab(participantNumber).getProductShareTurnoverSize(productShareNumber);
		}

		internal static int getCustomersCount()
		{
			return getCustomerParticipantsCount();
		}

		internal static string getCustomerBranchTin(int customerNumber)
		{
			return invoiceForm.getPannel<panelESFpartC>().getTab(customerNumber).getCustomerBranchTin();
		}

		internal static string getCustomerAddress(int customerNumber)
		{
			return invoiceForm.getPannel<panelESFpartC>().getTab(customerNumber).getCustomerAddress();
		}

		internal static string getCustomerCountryCode(int customerNumber)
		{
			return invoiceForm.getPannel<panelESFpartC>().getTab(customerNumber).getCustomerCountryCode();
		}

		internal static string getCustomerName(int customerNumber)
		{
			return invoiceForm.getPannel<panelESFpartC>().getTab(customerNumber).getCustomerName();
		}

		internal static string getCustomerReorgTin(int customerNumber)
		{
			return invoiceForm.getPannel<panelESFpartC>().getTab(customerNumber).getCustomerReorgTin();
		}

		internal static float getCustomerShareParticipation(int customerNumber)
		{
			return invoiceForm.getPannel<panelESFpartC>().getTab(customerNumber).getCustomerShareParticipation();
		}

		internal static int getCustomerStatusesCount(int customerNumber)
		{
			return invoiceForm.getPannel<panelESFpartC>().getTab(customerNumber).getCustomerStatusesCount();
		}

		internal static CustomerType getCustomerStatusById(int customerNumber, int statusId)
		{
			return invoiceForm.getPannel<panelESFpartC>().getTab(customerNumber).getCustomerStatusById(statusId);
		}

		internal static string getCustomerTin(int customerNumber)
		{
			return invoiceForm.getPannel<panelESFpartC>().getTab(customerNumber).getCustomerTin();
		}

		internal static string getCustomerTrailer(int customerNumber)
		{
			return invoiceForm.getPannel<panelESFpartC>().getTab(customerNumber).getCustomerTrailer();
		}

		internal static DateTime getInvoiceDatePaper()
		{
			return invoiceForm.getPannel<panelESFpartA>().getInvoiceDatePaper();
		}

		internal static DateTime getInvoiceDeliveryDocDate()
		{
			return invoiceForm.getPannel<panelESFpartF>().getInvoiceDeliveryDocDate();
		}

		internal static string getInvoiceDeliveryDocNum()
		{
			return invoiceForm.getPannel<panelESFpartF>().getInvoiceDeliveryDocNum();
		}

		internal static DateTime getDeliveryTermContractDate()
		{
			return invoiceForm.getPannel<panelESFpartE>().getDeliveryTermContractDate();
		}

		internal static string getDeliveryTermContractNum()
		{
			return invoiceForm.getPannel<panelESFpartE>().getDeliveryTermContractNum();
		}

		internal static string getDeliveryTermConditiomCode()
		{
			return invoiceForm.getPannel<panelESFpartE>().getDeliveryTermConditiomCode();
		}

		internal static string getDeliveryTermDestination()
		{
			return invoiceForm.getPannel<panelESFpartE>().getDeliveryTermDestination();
		}

		internal static bool getDeliveryTermHasContract()
		{
			return invoiceForm.getPannel<panelESFpartE>().getDeliveryTermHasContract();
		}

		internal static string getDeliveryTermTerm()
		{
			return invoiceForm.getPannel<panelESFpartE>().getDeliveryTermTerm();
		}

		internal static string getDeliveryTermTransportTypeCode()
		{
			return invoiceForm.getPannel<panelESFpartE>().getDeliveryTermTransportTypeCode();
		}

		internal static string getDeliveryTermWarrant()
		{
			return invoiceForm.getPannel<panelESFpartE>().getDeliveryTermWarrant();
		}

		internal static DateTime getDeliveryTermWarrantDate()
		{
			return invoiceForm.getPannel<panelESFpartE>().getDeliveryTermWarrantDate();
		}

		internal static string getProductSetCurrentCode()
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductSetCurrentCode();
		}

		internal static float getProductSetCurrencyRate()
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductSetCurrencyRate();
		}

		internal static NdsRateType getProductSetNdsRateType()
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductSetNdsRateType();
		}

		internal static int getProductsCount()
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductsCount();
		}

		internal static string getProductAdditional(int productNum)
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductAdditional(productNum);
		}

		internal static string getProductCatalogTruId(int productNum)
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductCatalogTruId(productNum);
		}

		internal static string getProductDescription(int productNum)
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductDescription(productNum);
		}

		internal static float getProductExciseAmount(int productNum)
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductExciseAmount(productNum);
		}

		internal static float getProductExciseRate(int productNum)
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductExciseRate(productNum);
		}

		internal static string getProductKpvedCode(int productNum)
		{
			return "";
			//return invoiceForm.getPannel<panelESFpartG>().getProductKpvedCode(productNum);
		}

		internal static float getProductNDSAmount(int productNum)
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductNDSAmount(productNum);
		}

		internal static int getProductNDSRAte(int productNum)
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductNDSRate(productNum);
		}

		internal static float getProductPriceWithoutTax(int productNum)
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductPriceWithoutTax(productNum);
		}

		internal static string getProductDeclaration(int productNum)
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductDeclaration(productNum);
		}

		internal static float getProductPriceWithTax(int productNum)
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductPriceWithTax(productNum);
		}

		internal static string getProductNumberInDeclaration(int productNum)
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductNumberInDeclaration(productNum);
		}

		internal static string getProductTnvedName(int productNum)
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductTnvedName(productNum);
		}

		internal static TruOriginCode getProductTruOriginCode(int productNum)
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductTruOriginCode(productNum);
		}

		internal static float getProductTurnoverSize(int productNum)
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductTurnoverSize(productNum);
		}

		internal static string getProductUnitCode(int productNum)
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductUnitCode(productNum);
		}

		internal static string getProductUnitNominclature(int productNum)
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductUnitNominclature(productNum);
		}

		internal static float getProductUnitPrice(int productNum)
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductUnitPrice(productNum);
		}

		internal static float getProductSetTotalExciseAmount()
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductSetTotalExciseAmount();
		}

		internal static float getProductSetTotalNDSAmount()
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductSetTotalNDSAmount();
		}

		internal static float getProductSetTotalPriceWithoutTax()
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductSetTotalPriceWithoutTax();
		}

		internal static float getProductSetTotalPriceWithTax()
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductSetTotalPriceWithTax();
		}

		internal static float getProductSetTotalTurnoverSize()
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductSetTotalTurnoverSize();
		}

		internal static string getPublicOfficeBik()
		{
			return invoiceForm.getPannel<panelESFpartC>().getTab().getPublicOfficeBik();
		}

		internal static string getPublicOfficeIik()
		{
			return invoiceForm.getPannel<panelESFpartC>().getTab().getPublicOfficeIik();
		}

		internal static string getPublicOfficePayPurpose()
		{
			return invoiceForm.getPannel<panelESFpartC>().getTab().getPublicOfficePayPurpose();
		}

		internal static string getPublicOfficeProductCode()
		{
			return invoiceForm.getPannel<panelESFpartC>().getTab().getPublicOfficeProductCode();
		}

		internal static PaperReasonType getReasonPaper()
		{
			return invoiceForm.getPannel<panelESFpartA>().getReasonPaper();
		}

		
		internal static string getSellerCertificateSeries()
		{
			throw new NotImplementedException();
		}

		internal static string getInvoiceSellerAgentAddress()
		{
			return invoiceForm.getPannel<panelESFpartI>().getInvoiceSellerAgentAddress();
		}

		internal static DateTime getInvoiceSellerAgentDocDate()
		{
			return invoiceForm.getPannel<panelESFpartI>().getInvoiceSellerAgentDocDate();
		}

		internal static string getInvoiceSellerAgentDocNum()
		{
			return invoiceForm.getPannel<panelESFpartI>().getInvoiceSellerAgentDocNum();
		}

		internal static string getInvoiceSellerAgentName()
		{
			return invoiceForm.getPannel<panelESFpartI>().getInvoiceSellerAgentName();
		}

		internal static bool setInvoiceNum(string num)
		{
			return invoiceForm.getPannel<panelESFpartA>().setInvoiceNum(num);
		}

		internal static string getInvoiceSellerAgentTin()
		{
			return invoiceForm.getPannel<panelESFpartI>().getInvoiceSellerAgentTin();
		}

		internal static int getSellerParticipantsCount()
		{
			return invoiceForm.getPannel<panelESFpartB>().getSellerParticipantsCount();
		}

		internal static bool setInvoiceDate(DateTime date)
		{
			return invoiceForm.getPannel<panelESFpartA>().setInvoiceDate(date);
		}

		internal static string getSellerParticipantTin(int number)
		{
			return invoiceForm.getPannel<panelESFpartB>().getTab(number).getSellerParticipantTin();
		}

		internal static string getInvoiceNum()
		{
			return invoiceForm.getPannel<panelESFpartA>().getInvoiceNum();
		}

		internal static string getSellerParticipantReorgTin(int number)
		{
			return invoiceForm.getPannel<panelESFpartB>().getTab(number).getSellerParticipantReorgTin();
		}

		internal static DateTime getInvoiceDate()
		{
			return invoiceForm.getPannel<panelESFpartA>().getInvoiceDate();
		}

		internal static int getShareBySellerParticipantCount(int sellerNumber)
		{
			return invoiceForm.getPannel<panelESFpartH>().getSellerTab(sellerNumber).GetDataGrid().Rows.Count;
		}

		internal static bool isPaperESF()
		{
			return invoiceForm.getPannel<panelESFpartA>().isPaperESF();
		}

		internal static string getSellerProductShareAdditional(int sellerNumber, int productSharNum)
		{
			return invoiceForm.getPannel<panelESFpartH>().getSellerTab(sellerNumber).getProductShareAdditional(productSharNum);
		}

		internal static float getSellerProductShareExciseAmount(int sellerNumber, int productSharNum)
		{
			return invoiceForm.getPannel<panelESFpartH>().getSellerTab(sellerNumber).getProductShareExciseAmount(productSharNum);
		}

		internal static float getSellerProductShareNDSAmount(int sellerNumber, int productSharNum)
		{
			return invoiceForm.getPannel<panelESFpartH>().getSellerTab(sellerNumber).getProductShareNDSAmount(productSharNum);
		}

		internal static float getSellerProductSharePriceWithoutTax(int sellerNumber, int productSharNum)
		{
			return invoiceForm.getPannel<panelESFpartH>().getSellerTab(sellerNumber).getProductSharePriceWithoutTax(productSharNum);
		}

		internal static float getSellerProductSharePriceWithhTax(int sellerNumber, int productSharNum)
		{
			return invoiceForm.getPannel<panelESFpartH>().getSellerTab(sellerNumber).getProductSharePriceWithTax(productSharNum);
		}

		internal static float getSellerProductShareQuantity(int sellerNumber, int productSharNum)
		{
			return invoiceForm.getPannel<panelESFpartH>().getSellerTab(sellerNumber).getProductShareQuantity(productSharNum);
		}

		internal static float getSellerProductShareTurnoverSize(int sellerNumber, int productSharNum)
		{
			return invoiceForm.getPannel<panelESFpartH>().getSellerTab(sellerNumber).getProductShareTurnoverSize(productSharNum);
		}

		internal static string getSellerTrailer(int sellerNum)
		{
			return invoiceForm.getPannel<panelESFpartB>().getTab(sellerNum).getSellerTrailer();
		}

		internal static string getSellerTin(int sellerNum)
		{
			return invoiceForm.getPannel<panelESFpartB>().getTab(sellerNum).getSellerTin();
		}

		internal static int getSellersCount()
		{
			return getSellerParticipantsCount();
		}

		internal static string getSellerAddress(int sellerNum)
		{
			return invoiceForm.getPannel<panelESFpartB>().getTab(sellerNum).getSellerAddress();
		}

		internal static string getSellerReorgTin(int sellerNum)
		{
			return invoiceForm.getPannel<panelESFpartB>().getTab(sellerNum).getSellerReorgTin();
		}

		internal static int getSellerStatusesCount(int sellerNum)
		{
			return invoiceForm.getPannel<panelESFpartB>().getTab(sellerNum).getSellerStatusesCount();
		}

		internal static SellerType getSellerStatusById(int sellerNum, int statusId)
		{
			return invoiceForm.getPannel<panelESFpartB>().getTab(sellerNum).getSellerStatusById(statusId);
		}

		internal static string getSellerKbe(int sellerNum)
		{
			return invoiceForm.getPannel<panelESFpartB>().getTab(sellerNum).getSellerKbe();
		}

		internal static bool getSellerIsBranchNonResident(int sellerNum)
		{
			return invoiceForm.getPannel<panelESFpartB>().getTab(sellerNum).getSellerIsBranchNonResiden();
		}

		internal static string getSellerName(int sellerNum)
		{
			return invoiceForm.getPannel<panelESFpartB>().getTab(sellerNum).getSellerName();
		}

		internal static string getSellerIik(int sellerNum)
		{
			return invoiceForm.getPannel<panelESFpartB>().getTab(sellerNum).getSellerIik();
		}

		internal static string getSellerCertificateNum(int sellerNum)
		{
			return invoiceForm.getPannel<panelESFpartB>().getTab(sellerNum).getSellerCertificateNum();
		}

		internal static string getSellerCertificateSeries(int sellerNum)
		{
			return invoiceForm.getPannel<panelESFpartB>().getTab(sellerNum).getSellerCertificateSeries();
		}

		internal static string getSellerBranchTin(int sellerNum)
		{
			return invoiceForm.getPannel<panelESFpartB>().getTab(sellerNum).getSellerBranchTin();
		}

		internal static string getSellerBank(int sellerNum)
		{
			return invoiceForm.getPannel<panelESFpartB>().getTab(sellerNum).getSellerBank();
		}

		internal static string getSellerBik(int sellerNum)
		{
			return invoiceForm.getPannel<panelESFpartB>().getTab(sellerNum).getSellerBik();
		}

		internal static DateTime getInvoiceTurnoverDate()
		{
			return invoiceForm.getPannel<panelESFpartA>().getInvoiceTurnoverDate();
		}

		internal static float getSellerShareParticipation(int sellerNum)
		{
			return invoiceForm.getPannel<panelESFpartB>().getTab(sellerNum).getSellerShareParticipation();
		}

		internal static float getProductQuantity(int productNum)
		{
			return invoiceForm.getPannel<panelESFpartG>().getProductQuantity(productNum);
		}


		internal static bool clearAllTabs()
		{
			try
			{						
				panelESFpartA panelA = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartA>();
				panelA.setInvoiceDate(DateTime.Now);
				panelA.setInvoiceNum("");
				panelA.setInvoiceType(InvoiceType.ORDINARY_INVOICE);
				panelA.setAddedESFRegistrationNum("");
				panelA.setAddedESFNum("");
				panelA.setAddedESFDate(DateTime.Now);
				panelA.setOperatorFullname("");
				panelA.setInvoiceTurnoverDate(DateTime.Now);
				panelA.setInvoiceDatePaper(DateTime.Now);
				panelA.clearReasonPaper();

				panelESFpartB panelB = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartB>();
				panelB.RemoveAllTabs();
				panelB.CreateFirstTab("Seller");	

				panelESFpartC panelC = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartC>();
				panelC.RemoveAllTabs();
				panelC.CreateFirstTab("Customer");

				panelESFpartD panelD = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartD>();
				panelD.setConsigneeAddress("");
				panelD.setConsigneeCountryCode("");
				panelD.setConsigneeName("");
				panelD.setConsigneeTin("");			

				panelD.setConsignorAddress("");
				panelD.setConsignorName("");
				panelD.setConsignorTin("");

				panelESFpartE panelE = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartE>();

				panelE.setDeliveryTermContractDate(DateTime.Now);
				panelE.setDeliveryTermContractNum("");
				panelE.setDeliveryTermConditiomCode("");
				panelE.setDeliveryTermDestination("");
				panelE.setDeliveryTermHasContract(false);
				panelE.setDeliveryTermTerm("");
				panelE.setDeliveryTermTransportTypeCode("");
				panelE.setDeliveryTermWarrant("");
				panelE.setDeliveryTermWarrantDate(DateTime.Now);

				panelESFpartF panelF = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartF>();
				panelF.setInvoiceDeliveryDocDate(DateTime.Now);
				panelF.setInvoiceDeliveryDocNum("");

				panelESFpartG panelG = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartG>();
				panelG.clearProductSetCurrencyRate();
				panelG.setProductSetCurrentCode("");
				panelG.clearProductSetNdsRateType();
				panelG.GetDataGrid().Rows.Clear();						
				panelG.setProductSetTotalExciseAmount(0);
				panelG.setProductSetTotalNDSAmount(0);
				panelG.setProductSetTotalPriceWithTax(0);
				panelG.setProductSetTotalPriceWithoutTax(0);
				panelG.setProductSetTotalTurnoverSize(0);				


				panelESFpartH panelH = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartH>();
				panelH.RemoveAllTabs();

				panelESFpartI panelI = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartI>();
				panelI.setInvoiceSellerAgentAddress("");
				panelI.setInvoiceSellerAgentDocDate(DateTime.Now);
				panelI.setInvoiceSellerAgentDocNum("");
				panelI.setInvoiceSellerAgentName("");
				panelI.setInvoiceSellerAgentTin("");

				panelESFpartJ panelJ = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartJ>();
				panelJ.setCustomerAgentAddress("");
				panelJ.setCustomerAgentDocDate(DateTime.Now);
				panelJ.setCustomerAgentDocNum("");
				panelJ.setCustomerAgentName("");
				panelJ.setCustomerAgentTin("");

				panelESFpartK panelK = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartK>();
				panelK.setInvoiceAddInf("");

				panelESFpartL panelL = FormManagerFacade.getInvoiceForm().getPannel<panelESFpartL>();

				return true;

			}
			catch (Exception)
			{
				return false;
			}
		}

		internal static InvoiceType getInvoiceType()
		{
			return invoiceForm.getPannel<panelESFpartA>().getInvoiceType();
		}
	}
}
