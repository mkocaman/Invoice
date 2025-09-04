using ESF_kz.LocalService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESF_kz
{
	static class LocalServiceOperationFacade
	{
		static private LocalServiceClient serviceClient;

		static private LocalServiceClient getServiceClient()
		{
			if (serviceClient == null)
			{
				serviceClient = new LocalServiceClient();
			}

			return serviceClient;
		}

		static public bool GenerateInvoiceSignature()
		{
			SignatureRequest signatureRequest = new SignatureRequest();
			signatureRequest.version = ConfigManagerFacade.getESFVersion();
			signatureRequest.certificatePath = SessionDataManagerFacade.getSignCertificatePath();
			signatureRequest.certificatePin = SessionDataManagerFacade.getSignCertificatePin();
			signatureRequest.invoiceBodies = SessionDataManagerFacade.getInvoiceBodies();
			try
			{
				SignatureResponse signatureResponse = getServiceClient().generateSignature(signatureRequest);				
				return SessionDataManagerFacade.setInvoiceSignature(signatureResponse); ;
			}
			catch (Exception)
			{
				return false;
			}			
		}

		static public bool GenerateIdWithReasonListSignature()
		{
			ListWithReasonSignatureRequest listSignatureRequest = new ListWithReasonSignatureRequest();
			listSignatureRequest.certificatePath = SessionDataManagerFacade.getSignCertificatePath();
			listSignatureRequest.certificatePin = SessionDataManagerFacade.getSignCertificatePin();
			listSignatureRequest.idsWithReasons = SessionDataManagerFacade.getInvoiceIdWithReasonsList_LocalService();
						
			ListSignatureResponse listSignatureResponse;
			try
			{
				listSignatureResponse = getServiceClient().signIdWithReasonList(listSignatureRequest);
				return SessionDataManagerFacade.setInvoiceSignatureIdWithReason(listSignatureResponse);
			}
			catch (Exception)
			{
				return false;
			}	
		}

		static public bool GenerateIdListSignature()
		{
			ListSignatureRequest listSignatureRequest = new ListSignatureRequest();
			listSignatureRequest.certificatePath = SessionDataManagerFacade.getSignCertificatePath();
			listSignatureRequest.certificatePin = SessionDataManagerFacade.getSignCertificatePin();
			listSignatureRequest.ids = SessionDataManagerFacade.getInvoiceIdList();


			ListSignatureResponse listSignatureResponse;
			try
			{
				listSignatureResponse = getServiceClient().signIdList(listSignatureRequest);
				return SessionDataManagerFacade.setInvoiceSignatureId(listSignatureResponse);
			}
			catch (Exception)
			{
				return false;
			}	
		}
	}

	
}
