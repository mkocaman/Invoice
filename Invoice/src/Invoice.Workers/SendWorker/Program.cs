using Microsoft.EntityFrameworkCore;
using SendWorker;
using Invoice.Infrastructure.Data;

var builder = Host.CreateApplicationBuilder(args);

// PostgreSQL bağlantı dizesini al
var connectionString = builder.Configuration.GetConnectionString("Postgres");

// Entity Framework Core DbContext'i ekle
builder.Services.AddDbContext<InvoiceDbContext>(options =>
    options.UseNpgsql(connectionString));

// Worker servisini ekle
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
