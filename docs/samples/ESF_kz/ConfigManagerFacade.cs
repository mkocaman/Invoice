using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESF_kz
{
	static class ConfigManagerFacade
	{
		internal static string getInvoiceService_EndpointAddress()
		{
			return "https://test3.esf.kgd.gov.kz:8443/esf-web/ws/api1/InvoiceService?wsdl";
		}

		internal static long getInvoiceService_MaxReceivedMessageSize()
		{
			return  20000000;
		}

		internal static bool isSessionServiceConfigChanged()
		{
			return false;
		}

		internal static int getInvoiceService_MaxBufferSize()
		{
			return 20000000;
		}

		internal static bool isInvoiceServiceConfigChanged()
		{
			return false;
		}

		internal static string getESFVersion()
		{
			return "InvoiceV2";
		}

		internal static long getInvoiceService_MaxBufferPoolSize()
		{
			return 20000000;
		}

		internal static int getInvoiceService_readerQuotasMaxArrayLength()
		{
			return 20000000;
		}

		internal static int getInvoiceService_readerQuotasMaxStringContentLength()
		{
			return 20000000;
		}

		internal static bool getInvoiceService_AllowCookies()
		{
			return true;
		}

		internal static int getInvoiceService_readerQuotasMaxDepth()
		{
			return 32;
		}

		internal static string getSellerTin()
		{
			return "760816300415";
		}

		internal static string getCertificateNum()
		{
			return "1399478";
		}

		internal static long getSessionService_MaxReceivedMessageSize()
		{
			return 20000000;
		}

		internal static string getCertificateSeries()
		{
			return "13788";
		}

		internal static string getSessionService_EndpointAddress()
		{
			return "https://test3.esf.kgd.gov.kz:8443/esf-web/ws/api1/SessionService?wsdl";
		}
	}
}
