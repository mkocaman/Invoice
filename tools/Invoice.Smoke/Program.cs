using Microsoft.Extensions.DependencyInjection;
using Invoice.Application.Interfaces;
using Invoice.Infrastructure;
using Invoice.Infrastructure.TR;

var services = new ServiceCollection();
services.AddInfrastructure("Host=localhost;Database=invoice_test");
services.AddInvoiceProviders();
services.AddInvoiceProvidersTR();
var sp = services.BuildServiceProvider();

var factory = sp.GetRequiredService<IInvoiceProviderFactory>();

Console.WriteLine("== TR supported providers ==");
foreach (var p in factory.GetSupportedProviders("TR"))
    Console.WriteLine($" • {p}");
