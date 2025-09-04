using ESF_kz.UploadInvoiceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESF_kz
{
	static class UploadInvoiceServiceOperationFacade
	{
		static private UploadInvoiceServiceClient  serviceClient;

		static private UploadInvoiceServiceClient getServiceClient()
		{
			if (serviceClient == null)
			{
				serviceClient = new UploadInvoiceServiceClient();
			}

			return serviceClient;
		}

		static internal bool SendInvoice()
		{
			SyncInvoiceRequest syncInvoiceRequest = new SyncInvoiceRequest();
			syncInvoiceRequest.sessionId = SessionDataManagerFacade.getSessionId();
			syncInvoiceRequest.x509Certificate = SessionDataManagerFacade.getX509SignCertificate();
			syncInvoiceRequest.invoiceUploadInfoList = SessionDataManagerFacade.getInvoiceUploadInfoList();

			//SyncInvoiceResponse syncInvoiceResponse;
			object syncInvoiceResponse;
			try
			{
				syncInvoiceResponse = getServiceClient().syncInvoice(syncInvoiceRequest);
				return SessionDataManagerFacade.setInvoiceId((SyncInvoiceResponse)syncInvoiceResponse);
				
			}
			catch (Exception e)
			{
				System.Windows.Forms.MessageBox.Show(e.Message);
				return false;
			}			
		}
	}
}
