using ESF_kz.Forms;
using ESF_kz.InvoiceService;
using ESF_kz.LocalService;
using ESF_kz.SessionService;
using ESF_kz.UploadInvoiceService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ESF_kz
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Process process = Process.Start(new ProcessStartInfo("cmd.exe", "/c java -jar esf_local_server.jar"));			
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			/*string ialogResult = (((detailsESFfieldAttribute)Attribute.GetCustomAttribute((typeof(ESF)).GetField("registrationNumber"), typeof(detailsESFfieldAttribute))).details1);*/

			/*#region custom binding

			CustomBinding customBinding = new CustomBinding();

			var security = TransportSecurityBindingElement.CreateUserNameOverTransportBindingElement();
			security.IncludeTimestamp = false;
			security.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic256;
			security.MessageSecurityVersion = MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10;

			var encoding = new TextMessageEncodingBindingElement();
			encoding.MessageVersion = MessageVersion.Soap11;

			var transport = new HttpsTransportBindingElement();
			transport.MaxReceivedMessageSize = 20000000; // 20 megs

			customBinding.Elements.Add(security);
			customBinding.Elements.Add(encoding);
			customBinding.Elements.Add(transport);

			#endregion

			EndpointAddress endpointAdress = new EndpointAddress("https://test3.esf.kgd.gov.kz:8443/esf-web/ws/api1/SessionService?wsdl");

			SessionServiceClient sessionService = new SessionServiceClient(customBinding, endpointAdress);
			sessionService.ClientCredentials.UserName.UserName = "760816300415";
			sessionService.ClientCredentials.UserName.Password = "Micr0!nvest";

			LocalServiceClient localService = new LocalServiceClient();
			UploadInvoiceServiceClient uploadInvoiceService = new UploadInvoiceServiceClient();

			CustomBinding invoiceServiceCustomBinding = new CustomBinding();
			invoiceServiceCustomBinding.Elements.Add(transport);
			EndpointAddress invoiceServiceEndpointAddress = new EndpointAddress("https://test3.esf.kgd.gov.kz:8443/esf-web/ws/api1/InvoiceService?wsdl");
			BasicHttpsBinding basicHttpBinding = new BasicHttpsBinding();
			basicHttpBinding.MaxReceivedMessageSize = 20000000;
			basicHttpBinding.MaxBufferSize = 20000000;
			basicHttpBinding.MaxBufferPoolSize = 20000000;
			basicHttpBinding.AllowCookies = true;

			var readerQuotas = new XmlDictionaryReaderQuotas();
			readerQuotas.MaxArrayLength = 20000000;
			readerQuotas.MaxStringContentLength = 20000000;
			readerQuotas.MaxDepth = 32;
			basicHttpBinding.ReaderQuotas = readerQuotas;

			InvoiceServiceClient invoiceService = new InvoiceServiceClient(basicHttpBinding, invoiceServiceEndpointAddress);*/




			/*SessionService.createSessionRequest CreateSessionRequest = new createSessionRequest();
			CreateSessionRequest.tin = "760816300415";
			CreateSessionRequest.x509Certificate = "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";
			System.ServiceModel.Description.ClientCredentials clientCredentials = new System.ServiceModel.Description.ClientCredentials();

			SessionService.closeSessionByCredentialsRequest CloseSessionByCredentialsRequest = new closeSessionByCredentialsRequest();
			CloseSessionByCredentialsRequest.tin = "760816300415";
			CloseSessionByCredentialsRequest.x509Certificate = "MIIGfTCCBGWgAwIBAgIUQQTsqQ33Od/MxFp9exvNkeDbRkQwDQYJKoZIhvcNAQELBQAwUjELMAkGA1UEBhMCS1oxQzBBBgNVBAMMOtKw0JvQotCi0KvSmiDQmtCj05jQm9CQ0J3QlNCr0KDQo9Co0Ksg0J7QoNCi0JDQm9Cr0pogKFJTQSkwHhcNMjAwNjE3MDgzMjM4WhcNMjEwNjE3MDgzMjM4WjCBvzEkMCIGA1UEAwwb0J/QmNCd0KfQo9CaINCS0JjQotCQ0JvQmNCZMRUwEwYDVQQEDAzQn9CY0J3Qp9Cj0JoxGDAWBgNVBAUTD0lJTjc2MDgxNjMwMDQxNTELMAkGA1UEBhMCS1oxHDAaBgNVBAcME9Cd0KPQoC3QodCj0JvQotCQ0J0xHDAaBgNVBAgME9Cd0KPQoC3QodCj0JvQotCQ0J0xHTAbBgNVBCoMFNCS0JDQodCY0JvQrNCV0JLQmNCnMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqwQuWGw75o28RDbitgwb15zzRGiRmaHqLnjwCu+s4sm5LC5FsnM9iIxSQsDz9Y1Pn+tPbdt/RIlw/eljZCsaqRh9jsAs/A2Bt8aajetd+hVcmiALaFxnoAlNQ11e4Vb+dLAw7ZH88H5AHjYXfLrmfUjkHu9+AOd069fQ7hhrDCK3/pHlE9SViQct3oPoXdctchhWE7vKliEXe8521+zFZllVh4uMl3hNupLgG5bkdQkNBfnSRYJA8KRDuGzXXltILrtyd8CTRsXDkdQBCxoWY40DflwlTnPz3CO8Q9avVW5pbvWYyzYVwv8EY4zXJMwUyFSgdSCDf4zOP2Qxce6mlwIDAQABo4IB2zCCAdcwDgYDVR0PAQH/BAQDAgWgMB0GA1UdJQQWMBQGCCsGAQUFBwMCBggqgw4DAwQBATAPBgNVHSMECDAGgARbanQRMB0GA1UdDgQWBBQS/EQPIEnxtbYXZRFPKOWOxn7M/zBeBgNVHSAEVzBVMFMGByqDDgMDAgQwSDAhBggrBgEFBQcCARYVaHR0cDovL3BraS5nb3Yua3ovY3BzMCMGCCsGAQUFBwICMBcMFWh0dHA6Ly9wa2kuZ292Lmt6L2NwczBWBgNVHR8ETzBNMEugSaBHhiFodHRwOi8vY3JsLnBraS5nb3Yua3ovbmNhX3JzYS5jcmyGImh0dHA6Ly9jcmwxLnBraS5nb3Yua3ovbmNhX3JzYS5jcmwwWgYDVR0uBFMwUTBPoE2gS4YjaHR0cDovL2NybC5wa2kuZ292Lmt6L25jYV9kX3JzYS5jcmyGJGh0dHA6Ly9jcmwxLnBraS5nb3Yua3ovbmNhX2RfcnNhLmNybDBiBggrBgEFBQcBAQRWMFQwLgYIKwYBBQUHMAKGImh0dHA6Ly9wa2kuZ292Lmt6L2NlcnQvbmNhX3JzYS5jZXIwIgYIKwYBBQUHMAGGFmh0dHA6Ly9vY3NwLnBraS5nb3Yua3owDQYJKoZIhvcNAQELBQADggIBAJpS1w799YUozZjmX/zJmHYozsULPI4GmjFJ8V/yikniMgNeJJEKSm8TpJ3IaaJZyo1H5b1vZtTJp1s0gi9h84NjWee98Jf8oJzrmuC2OujgMDdp3dkaUz7TS0K3D4gELiaUhRGvNcO+n4vq+Wv4AhddwLJlKBsmrtTaI4MW3gK8kQffaF6Z3kJulUH1DU8lqhmGpMVjMdFqAReyKkYgYYRPqLDl7soQpty2Qq4NIJVbeh8wFXRYHvW0w9nGlVQDaQc0GrhETGmFOAzrXOhnhU1RnDWjzoffaDjlFaimKFK1+Fpu1LiueMKyvI5nOwcUer3dGgYs/ywUoRaHo+4K/YDV3lhYCGF5nnfJNgspdGpAGmoKzfZ9iBfCpxw5Jc+EcbEdW56IgVvVFN5OcurAWDXBYAMWifcRS2oa3/n8SDNgmocQhcUS6uPOUQgRvxu0pwGbE65tKSS2YKJNC3M0BD6qiLZkzZSdBGj2Payq1gf09+BCnMFJTehOkTHhvhGTlti2fFwrBiYfbahsuSLsZTvG6WvcVmyt74QrJ0rlvn6tzKw544YOoQn2MQP+xoOtg0xArfqAz1yCFUzGTrbOND58tDAgx23Sft+5lxvkvX8zt/NxZB+JGtc7IuVo9IBM02vVOBhxAVXibgrxITvzWQK2NoR47eSDDKURLrh/1dVu";

			SessionService.createSessionResponse CreateSessionResponse;
			SessionService.closeSessionResponse CloseSessionByCredentialsResponse;
			

			string sessionId = "";
			try
			{
				CreateSessionResponse = sessionService.createSession(CreateSessionRequest);
				sessionId = CreateSessionResponse.sessionId;
			}
			catch (Exception)
			{
				CloseSessionByCredentialsResponse = sessionService.closeSessionByCredentials(CloseSessionByCredentialsRequest);
			}

			SessionService.currentSessionStatusRequest CurrentSessionStatusRequest = new currentSessionStatusRequest();
			SessionService.currentSessionStatusResponse CurrentSessionStatusResponse;
			CurrentSessionStatusRequest.sessionId = sessionId;
			CurrentSessionStatusResponse = sessionService.currentSessionStatus(CurrentSessionStatusRequest);*/
			/*if (CurrentSessionStatusResponse.status == sessionStatus.OK)
			
			{
				SessionService.currentUserRequest CurrentUserRequest = new currentUserRequest();
				CurrentUserRequest.sessionId = sessionId;
				SessionService.currentUserResponse CurrentUserResponse; 
				CurrentUserResponse = sessionService.currentUser(CurrentUserRequest);

				SessionService.currentUserProfilesRequest CurrentUserProfilesRequest = new currentUserProfilesRequest();
				CurrentUserProfilesRequest.sessionId = sessionId;
				SessionService.currentUserProfilesResponse CurrentUserProfilesResponse;
				CurrentUserProfilesResponse = sessionService.currentUserProfiles(CurrentUserProfilesRequest);

				#region enterpriseValidation InvoiceService

				EnterpriseValidationRequest enterpriseValidationRequest = new EnterpriseValidationRequest();
				enterpriseValidationRequest.sessionId = sessionId;

				EnterpriseKey enterpriseKey = new EnterpriseKey();
				string sellerTin = "760816300415";
				enterpriseKey.tin = sellerTin;
				enterpriseKey.certificateNum = "1399478";
				enterpriseKey.certificateSeries = "13788";
				EnterpriseKey[] enterpriseKeyList = { enterpriseKey };
				enterpriseValidationRequest.enterpriseKeyList=enterpriseKeyList;

				EnterpriseValidationResponse enterpriseValidationResponse;
				enterpriseValidationResponse = invoiceService.enterpriseValidation(enterpriseValidationRequest);
				#endregion

				#region GenerateInvoiceSignature LocalService
				SignatureRequest signatureRequest = new SignatureRequest();
				signatureRequest.version = "InvoiceV2";
				signatureRequest.certificatePath = @"C:\Users\viktor.kassov\source\repos\ESF_kz\ESF_kz\bin\Debug\Сертификат\ИП Пинчук ВВ до 17.06.21\ИП Пинчук ВВ до 17.06.21\RSA256_af8e6f8be023a8cc035198522a70ca7203a7059a.p12";
				signatureRequest.certificatePin = "Aa123456";

				string invoiceBodyPath = @"C:\Users\viktor.kassov\source\repos\ESF_kz\ESF_kz\bin\Debug\InvoiceBodyTestExample.txt";
				string invoiceBodyString = "";
				using (StreamReader sr = new StreamReader(invoiceBodyPath))
				{
					invoiceBodyString = sr.ReadToEnd();
				}

				string[] invoiceBodies = { invoiceBodyString };

				signatureRequest.invoiceBodies = invoiceBodies;

				SignatureResponse signatureResponse = localService.generateSignature(signatureRequest);
				string signature = signatureResponse.invoiceHashList[0].signature;
				#endregion

				#region SendInvoice UploadInvoiceService

				UploadInvoiceService.SyncInvoiceRequest syncInvoiceRequest = new SyncInvoiceRequest();
				syncInvoiceRequest.sessionId = sessionId;
				string sellerSignCertificate = "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";
				syncInvoiceRequest.x509Certificate = sellerSignCertificate;
				invoiceUploadInfo InvoiceUploadInfo = new invoiceUploadInfo();
				InvoiceUploadInfo.invoiceBody = invoiceBodyString;
				InvoiceUploadInfo.version = "InvoiceV2";
				InvoiceUploadInfo.signature = signature;
				InvoiceUploadInfo.signatureType = UploadInvoiceService.SignatureType.COMPANY;
				invoiceUploadInfo[] invoiceUploadInfoList = { InvoiceUploadInfo };
				syncInvoiceRequest.invoiceUploadInfoList = invoiceUploadInfoList;

				UploadInvoiceService.SyncInvoiceResponse syncInvoiceResponse;
				syncInvoiceResponse = uploadInvoiceService.syncInvoice(syncInvoiceRequest);
				long invoiceId = syncInvoiceResponse.acceptedSet[0].id;
				#endregion

				#region QueryInvoiceById InvoiceService

				InvoiceService.InvoiceByIdRequest invoiceByIdRequest = new InvoiceByIdRequest();
				invoiceByIdRequest.sessionId = sessionId;
				long[] idList = { invoiceId };
				invoiceByIdRequest.idList = idList;

				InvoiceService.QueryInvoiceResponse queryInvoiceResponse;
				queryInvoiceResponse = invoiceService.queryInvoiceById(invoiceByIdRequest);
				#endregion

				#region QueryInvoiceSummaryById InvoiceService 

				InvoiceService.InvoiceByIdRequest invoiceSummaryByIdRequest = new InvoiceByIdRequest();
				invoiceSummaryByIdRequest.sessionId = sessionId;
				invoiceSummaryByIdRequest.idList = idList;

				InvoiceService.InvoiceSummaryResponse queryInvoiceSummaryResponse;
				queryInvoiceSummaryResponse = invoiceService.queryInvoiceSummaryById(invoiceSummaryByIdRequest);
				#endregion

				#region QueryInvoiceByKey InvoiceService

				InvoiceService.InvoiceByKeyRequest invoiceByKeyRequest = new InvoiceByKeyRequest();
				invoiceByKeyRequest.sessionId = sessionId;
				InvoiceKey invoiceKey = new InvoiceKey();
				invoiceKey.date = "14.09.2020";
				invoiceKey.num = "7723243884732154991";
				InvoiceKey[] keyList = { invoiceKey};
				invoiceByKeyRequest.invoiceKeyList = keyList;
				invoiceByKeyRequest.direction = InvoiceDirection.OUTBOUND;

				InvoiceService.QueryInvoiceResponse queryInvoiceByKeyResponse;
				queryInvoiceByKeyResponse = invoiceService.queryInvoiceByKey(invoiceByKeyRequest);
				#endregion

				#region QueryUpdates InvoiceService

				InvoiceService.QueryInvoiceUpdateRequest queryInvoiceUpdateRequest = new QueryInvoiceUpdateRequest();
				queryInvoiceUpdateRequest.sessionId = sessionId;
				queryInvoiceUpdateRequest.lastEventDate = new DateTime(2018, 01, 01);
				queryInvoiceUpdateRequest.lastInvoiceId = 0;
				queryInvoiceUpdateRequest.direction = InvoiceDirection.OUTBOUND;
				queryInvoiceUpdateRequest.limit = 10;
				queryInvoiceUpdateRequest.fullInfoOnStatusChange = false;

				InvoiceService.QueryInvoiceUpdateResponse queryInvoiceUpdateResponse;
				queryInvoiceUpdateResponse = invoiceService.queryUpdates(queryInvoiceUpdateRequest);
				#endregion

				#region queryInvoiceSummaryByKey InvoiceService

				InvoiceByKeyRequest invoiceSummaryByKeyRequest = new InvoiceByKeyRequest();
				invoiceSummaryByKeyRequest.sessionId = sessionId;
				invoiceSummaryByKeyRequest.invoiceKeyList = keyList;
				invoiceSummaryByKeyRequest.direction = InvoiceDirection.OUTBOUND;

				InvoiceSummaryResponse invoiceSummaryByKeyResponse;
				invoiceSummaryByKeyResponse = invoiceService.queryInvoiceSummaryByKey(invoiceSummaryByKeyRequest);

				#endregion

				#region QueryInvoice InvoiceService

				QueryInvoiceRequest queryInvoiceRequest = new QueryInvoiceRequest();
				queryInvoiceRequest.sessionId = sessionId;
				QueryInvoiceCriteria queryInvoiceCriteria = new QueryInvoiceCriteria();
				queryInvoiceCriteria.direction = InvoiceDirection.OUTBOUND;
				queryInvoiceCriteria.dateFrom = new DateTime(2020, 9, 1);
				queryInvoiceCriteria.dateTo = DateTime.Now;
				queryInvoiceCriteria.asc = true;
				queryInvoiceRequest.criteria = queryInvoiceCriteria;

				QueryInvoiceResponse queryInvoiceResponse1;
				queryInvoiceResponse1 = invoiceService.queryInvoice(queryInvoiceRequest);

				#endregion

				#region GenerateListSignature LocalService
				
				string sellerSignCertPin = "Aa123456";
				string sellerSignCertPath = @"C:\Users\viktor.kassov\source\repos\ESF_kz\ESF_kz\bin\Debug\Сертификат\ИП Пинчук ВВ до 17.06.21\ИП Пинчук ВВ до 17.06.21\RSA256_af8e6f8be023a8cc035198522a70ca7203a7059a.p12";
				ListWithReasonSignatureRequest listSignatureRequest = new ListWithReasonSignatureRequest();
				listSignatureRequest.certificatePath = sellerSignCertPath;
				listSignatureRequest.certificatePin = sellerSignCertPin;

				LocalService.InvoiceIdWithReason invoiceIdWithReason1 = new LocalService.InvoiceIdWithReason();
				invoiceIdWithReason1.id = invoiceId;
				invoiceIdWithReason1.reason = "reason";
				LocalService.InvoiceIdWithReason[] invoiceIdWithReasonsList1 = { invoiceIdWithReason1 };
				listSignatureRequest.idsWithReasons = invoiceIdWithReasonsList1;

				ListSignatureResponse listSignatureResponse;
				listSignatureResponse = localService.signIdWithReasonList(listSignatureRequest);
				string listSignature = listSignatureResponse.signature;
				#endregion
				
				#region RevokeInvoiceById InvoiceService

				InvoiceByIdWithReasonRequest invoiceByIdWithReasonRequest = new InvoiceByIdWithReasonRequest();
				invoiceByIdWithReasonRequest.sessionId = sessionId;
				invoiceByIdWithReasonRequest.signature = listSignature;
				invoiceByIdWithReasonRequest.x509Certificate = sellerSignCertificate;
				InvoiceService.InvoiceIdWithReason invoiceIdWithReason = new InvoiceService.InvoiceIdWithReason();
				invoiceIdWithReason.id = invoiceId;
				invoiceIdWithReason.reason = "reason";
				InvoiceService.InvoiceIdWithReason[] invoiceIdWithReasonsList = { invoiceIdWithReason };
				invoiceByIdWithReasonRequest.idWithReasonList = invoiceIdWithReasonsList;

				TryChangeStatusResponse tryChangeStatusResponse;
				tryChangeStatusResponse = invoiceService.revokeInvoiceById(invoiceByIdWithReasonRequest);
				#endregion
				
				#region declineInvoiceById InvoiceService

				InvoiceByIdWithReasonRequest invoiceByIdWithReasonRequest1 = new InvoiceByIdWithReasonRequest();
				invoiceByIdWithReasonRequest1.sessionId = sessionId;
				invoiceByIdWithReasonRequest1.signature = listSignature;
				invoiceByIdWithReasonRequest1.x509Certificate = sellerSignCertificate;
				invoiceByIdWithReasonRequest1.idWithReasonList = invoiceIdWithReasonsList;

				TryChangeStatusResponse tryChangeStatusResponse1;
				tryChangeStatusResponse1 = invoiceService.declineInvoiceById(invoiceByIdWithReasonRequest1);
				#endregion

				#region unrevokeInvoiceById InvoiceService

				InvoiceByIdWithReasonRequest invoiceByIdWithReasonRequest2 = new InvoiceByIdWithReasonRequest();
				invoiceByIdWithReasonRequest2.sessionId = sessionId;
				invoiceByIdWithReasonRequest2.signature = listSignature;
				invoiceByIdWithReasonRequest2.x509Certificate = sellerSignCertificate;
				invoiceByIdWithReasonRequest2.idWithReasonList = invoiceIdWithReasonsList;

				TryChangeStatusResponse tryChangeStatusResponse2;
				tryChangeStatusResponse2 = invoiceService.unrevokeInvoiceById(invoiceByIdWithReasonRequest2);
				#endregion

				#region confirmInvoiceById InvoiceService

				InvoiceByIdRequest invoiceByIdRequest1 = new InvoiceByIdRequest();
				invoiceByIdRequest1.sessionId = sessionId;
				invoiceByIdRequest1.idList = idList;

				InvoiceSummaryResponse invoiceSummaryResponse;
				invoiceSummaryResponse = invoiceService.confirmInvoiceById(invoiceByIdRequest1);
				#endregion
				
				#region QueryInvoiceErrorById InvoiceService

				InvoiceErrorByIdRequest invoiceErrorByIdRequest = new InvoiceErrorByIdRequest();
				invoiceErrorByIdRequest.sessionId = sessionId;
				invoiceErrorByIdRequest.idList = idList;

				InvoiceErrorByIdResponse invoiceErrorByIdResponse;
				invoiceErrorByIdResponse = invoiceService.queryInvoiceErrorById(invoiceErrorByIdRequest);
				#endregion

				#region queryInvoiceHistoryById InvoiceService

				QueryInvoiceHistoryByIdRequest queryInvoiceHistoryByIdRequest = new QueryInvoiceHistoryByIdRequest();
				queryInvoiceHistoryByIdRequest.idList = idList;
				queryInvoiceHistoryByIdRequest.sessionId = sessionId;

				QueryInvoiceHistoryByIdResponse queryInvoiceHistoryByIdResponse;
				queryInvoiceHistoryByIdResponse = invoiceService.queryInvoiceHistoryById(queryInvoiceHistoryByIdRequest);
				#endregion	

				#region signIdList LocalService

				ListSignatureRequest listSignatureRequest1 = new ListSignatureRequest();
				listSignatureRequest1.certificatePath = sellerSignCertPath;
				listSignatureRequest1.certificatePin = "Aa123456";
				listSignatureRequest1.ids = idList;

				ListSignatureResponse listSignatureResponse1 = localService.signIdList(listSignatureRequest1);
				string signature2 = listSignatureResponse1.signature;
				#endregion

				#region DeleteInvoiceById InvoiceService
				DeleteInvoiceByIdRequest deleteInvoiceByIdRequest = new DeleteInvoiceByIdRequest();
				deleteInvoiceByIdRequest.idList = idList;
				deleteInvoiceByIdRequest.sessionId = sessionId;
				deleteInvoiceByIdRequest.signature = signature2;
				deleteInvoiceByIdRequest.x509Certificate = sellerSignCertificate;

				DeleteInvoiceByIdResponse deleteInvoiceByIdResponse;
				deleteInvoiceByIdResponse = invoiceService.deleteInvoiceById(deleteInvoiceByIdRequest);
				#endregion

				SessionService.closeSessionRequest CloseSessionRequest = new closeSessionRequest();
				CloseSessionRequest.sessionId = sessionId;
				SessionService.closeSessionResponse CloseSessionResponse;
				CloseSessionResponse = sessionService.closeSession(CloseSessionRequest);

			}*/


			/*bool flag = false;
			if (SessionServiceOperationsFacade.StartSession())
			{
				flag = InvoiceServiceOperationsFacade.EnterpriseValidation();
				flag = LocalServiceOperationFacade.GenerateInvoiceSignature();
				flag = UploadInvoiceServiceOperationFacade.SendInvoice();
				flag = InvoiceServiceOperationsFacade.QueryInvoiceById();
				flag = InvoiceServiceOperationsFacade.QueryInvoiceSummaryById();
				flag = InvoiceServiceOperationsFacade.QueryInvoiceByKey();
				flag = InvoiceServiceOperationsFacade.QueryUpdates();
				flag = InvoiceServiceOperationsFacade.QueryInvoiceSummaryByKey();
				flag = InvoiceServiceOperationsFacade.QueryInvoice();
				flag = LocalServiceOperationFacade.GenerateIdWithReasonListSignature();
				flag = InvoiceServiceOperationsFacade.RevokeInvoiceById();
				flag = InvoiceServiceOperationsFacade.QueryInvoiceErrorById();
				flag = LocalServiceOperationFacade.GenerateIdListSignature();
				flag = InvoiceServiceOperationsFacade.DeleteInvoiceById();
				flag = SessionServiceOperationsFacade.CloseSession();
			}*/

			/*invoiceContainerV2 invoiceCont = new invoiceContainerV2();
			InvoiceV2 inv1 = new InvoiceV2();
			InvoiceV2 inv2 = new InvoiceV2();
			InvoiceV2[] inList = { inv1, inv2 };
			invoiceCont.invoiceSet = inList;

			XmlSerializer xmlSerializer = new XmlSerializer(typeof(invoiceContainerV2));
			XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
			ns.Add("esf", "esf");
			ns.Add("v2", "v2.esf");
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Encoding = Encoding.UTF8;

			using (XmlWriter xmlWriter = XmlWriter.Create("testW.xml",xmlWriterSettings))
			{
				xmlWriter.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"");
				xmlSerializer.Serialize(xmlWriter, invoiceCont,ns);
			}


			using (FileStream fs = new FileStream("TestInvoiceContainer.xml", FileMode.Open))
			{
				invoiceCont = (invoiceContainerV2)xmlSerializer.Deserialize(fs);
			}*/

			//Application.Run(new ESF_form());




			Application.Run(new MainForm());
		}
	}
}
