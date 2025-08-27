using Microsoft.EntityFrameworkCore;
using Invoice.Infrastructure.Data;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Türkçe kültür ayarlarını yapılandır
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("tr-TR");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("tr-TR");

// PostgreSQL bağlantı dizesini al
var connectionString = builder.Configuration.GetConnectionString("Postgres");

// Entity Framework Core DbContext'i ekle
builder.Services.AddDbContext<InvoiceDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
