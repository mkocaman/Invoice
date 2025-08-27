using Microsoft.EntityFrameworkCore;
using Invoice.Domain.Entities;

namespace Invoice.Infrastructure.Data;

/// <summary>
/// Invoice veritabanı context'i
/// </summary>
public class InvoiceDbContext : DbContext
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="options">DbContext options</param>
    public InvoiceDbContext(DbContextOptions<InvoiceDbContext> options) : base(options)
    {
    }
    
    /// <summary>
    /// EŞÜ tablosu
    /// </summary>
    public DbSet<Eshu> Eshus { get; set; }
    
    /// <summary>
    /// Şarj seansları tablosu
    /// </summary>
    public DbSet<ChargeSession> ChargeSessions { get; set; }
    
    /// <summary>
    /// Rapor ID'leri tablosu
    /// </summary>
    public DbSet<ReportId> ReportIds { get; set; }
    
    /// <summary>
    /// Faturalar tablosu
    /// </summary>
    public DbSet<Domain.Entities.Invoice> Invoices { get; set; }
    
    /// <summary>
    /// Fatura detayları tablosu
    /// </summary>
    public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
    
    /// <summary>
    /// Outbox mesajları tablosu
    /// </summary>
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    
    /// <summary>
    /// Entegrasyon logları tablosu
    /// </summary>
    public DbSet<IntegrationLog> IntegrationLogs { get; set; }
    
    /// <summary>
    /// Webhook logları tablosu
    /// </summary>
    public DbSet<WebhookLog> WebhookLogs { get; set; }
    
    /// <summary>
    /// Retry işleri tablosu
    /// </summary>
    public DbSet<RetryJob> RetryJobs { get; set; }
    
    /// <summary>
    /// İdempotency anahtarları tablosu
    /// </summary>
    public DbSet<IdempotencyKey> IdempotencyKeys { get; set; }
    
    /// <summary>
    /// Entegratör konfigürasyonları tablosu
    /// </summary>
    public DbSet<ProviderConfig> ProviderConfigs { get; set; }
    
    /// <summary>
    /// Audit kayıtları tablosu
    /// </summary>
    public DbSet<AuditEntry> AuditEntries { get; set; }
    
    /// <summary>
    /// Model konfigürasyonu
    /// </summary>
    /// <param name="modelBuilder">Model builder</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // EŞÜ konfigürasyonu
        modelBuilder.Entity<Eshu>(entity =>
        {
            entity.ToTable("Eshus");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SerialNumber).IsUnique();
            entity.HasIndex(e => e.TenantId);
            entity.Property(e => e.SerialNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Model).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Brand).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Manufacturer).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Location).IsRequired().HasMaxLength(500);
        });
        
        // Şarj seansları konfigürasyonu
        modelBuilder.Entity<ChargeSession>(entity =>
        {
            entity.ToTable("ChargeSessions");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.EshuId);
            entity.HasIndex(e => e.StartDate);
            entity.HasIndex(e => e.TenantId);
            entity.HasOne(e => e.Eshu).WithMany().HasForeignKey(e => e.EshuId);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.CancellationReason).HasMaxLength(500);
        });
        
        // Rapor ID konfigürasyonu
        modelBuilder.Entity<ReportId>(entity =>
        {
            entity.ToTable("ReportIds");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ReportNumber).IsUnique();
            entity.HasIndex(e => e.StartDate);
            entity.HasIndex(e => e.TenantId);
            entity.Property(e => e.ReportNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.RejectionReason).HasMaxLength(1000);
            entity.Property(e => e.Notes).HasMaxLength(2000);
        });
        
        // Faturalar konfigürasyonu
        modelBuilder.Entity<Domain.Entities.Invoice>(entity =>
        {
            entity.ToTable("Invoices");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.InvoiceNumber).IsUnique();
            entity.HasIndex(e => e.InvoiceDate);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.ChargeSessionId);
            entity.HasIndex(e => e.ReportId);
            entity.HasOne(e => e.ChargeSession).WithMany().HasForeignKey(e => e.ChargeSessionId);
            entity.HasOne(e => e.Report).WithMany().HasForeignKey(e => e.ReportId);
            entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Currency).IsRequired().HasMaxLength(3).HasDefaultValue("TRY");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.Uuid).HasMaxLength(100);
            entity.Property(e => e.ProviderReferenceNumber).HasMaxLength(100);
            entity.Property(e => e.ProviderResponseMessage).HasMaxLength(1000);
            entity.Property(e => e.ProviderErrorCode).HasMaxLength(50);
            entity.Property(e => e.ProviderErrorMessage).HasMaxLength(1000);
        });
        
        // Fatura detayları konfigürasyonu
        modelBuilder.Entity<InvoiceDetail>(entity =>
        {
            entity.ToTable("InvoiceDetails");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.InvoiceId);
            entity.HasOne(e => e.Invoice).WithMany(f => f.InvoiceDetails).HasForeignKey(e => e.InvoiceId);
            entity.Property(e => e.ItemName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ItemDescription).HasMaxLength(500);
            entity.Property(e => e.Unit).IsRequired().HasMaxLength(20);
            entity.Property(e => e.GtipCode).HasMaxLength(20);
            entity.Property(e => e.ItemCode).HasMaxLength(20);
        });
        
        // Outbox mesajları konfigürasyonu
        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("OutboxMessages");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.MessageId).IsUnique();
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.QueueName);
            entity.HasIndex(e => e.TenantId);
            entity.Property(e => e.MessageId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.MessageType).IsRequired().HasMaxLength(200);
            entity.Property(e => e.MessageContent).IsRequired();
            entity.Property(e => e.QueueName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ErrorMessage).HasMaxLength(2000);
            entity.Property(e => e.CorrelationId).HasMaxLength(100);
            entity.Property(e => e.ConversationId).HasMaxLength(100);
        });
        
        // Entegrasyon logları konfigürasyonu
        modelBuilder.Entity<IntegrationLog>(entity =>
        {
            entity.ToTable("IntegrationLogs");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.Level);
            entity.HasIndex(e => e.OperationType);
            entity.HasIndex(e => e.TenantId);
            entity.Property(e => e.Level).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.OperationType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ProviderType).HasMaxLength(50);
            entity.Property(e => e.HttpRequestUrl).HasMaxLength(500);
            entity.Property(e => e.HttpRequestMethod).HasMaxLength(10);
            entity.Property(e => e.ExceptionDetails);
            entity.Property(e => e.CorrelationId).HasMaxLength(100);
            entity.Property(e => e.UserId).HasMaxLength(100);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
        });
        
        // Webhook logları konfigürasyonu
        modelBuilder.Entity<WebhookLog>(entity =>
        {
            entity.ToTable("WebhookLogs");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.TenantId);
            entity.Property(e => e.WebhookUrl).IsRequired().HasMaxLength(500);
            entity.Property(e => e.WebhookContent).IsRequired();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ErrorMessage).HasMaxLength(2000);
            entity.Property(e => e.ExceptionDetails);
            entity.Property(e => e.CorrelationId).HasMaxLength(100);
            entity.Property(e => e.OperationType).IsRequired().HasMaxLength(100);
        });
        
        // Retry işleri konfigürasyonu
        modelBuilder.Entity<RetryJob>(entity =>
        {
            entity.ToTable("RetryJobs");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.NextRetryDate);
            entity.HasIndex(e => e.Priority);
            entity.HasIndex(e => e.TenantId);
            entity.Property(e => e.JobType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.JobData).IsRequired();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ErrorMessage).HasMaxLength(2000);
            entity.Property(e => e.ExceptionDetails);
            entity.Property(e => e.CorrelationId).HasMaxLength(100);
            entity.Property(e => e.Tags);
        });
        
        // İdempotency anahtarları konfigürasyonu
        modelBuilder.Entity<IdempotencyKey>(entity =>
        {
            entity.ToTable("IdempotencyKeys");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Key).IsUnique();
            entity.HasIndex(e => e.ExpiresAt);
            entity.HasIndex(e => e.TenantId);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Hash).HasMaxLength(64);
            entity.Property(e => e.Method).IsRequired().HasMaxLength(16);
            entity.Property(e => e.Path).IsRequired().HasMaxLength(256);
            entity.Property(e => e.FirstSeenAt).IsRequired();
            entity.Property(e => e.ExpiresAt).IsRequired();
        });
        
        // Entegratör konfigürasyonları konfigürasyonu
        modelBuilder.Entity<ProviderConfig>(entity =>
        {
            entity.ToTable("ProviderConfigs");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.ProviderKey }).IsUnique();
            entity.HasIndex(e => e.ProviderKey);
            entity.HasIndex(e => e.IsActive);
            entity.Property(e => e.TenantId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ProviderKey).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ApiBaseUrl).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ApiKey).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ApiSecret).IsRequired().HasMaxLength(500);
            entity.Property(e => e.WebhookSecret).IsRequired().HasMaxLength(500);
            entity.Property(e => e.VknTckn).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.BranchCode).HasMaxLength(50);
            entity.Property(e => e.SignMode).IsRequired();
            entity.Property(e => e.TimeoutSec).IsRequired();
            entity.Property(e => e.RetryCountOverride);
            entity.Property(e => e.CircuitTripThreshold);
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);
        });
        
        // Audit kayıtları konfigürasyonu
        modelBuilder.Entity<AuditEntry>(entity =>
        {
            entity.ToTable("AuditEntries");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.TableName);
            entity.HasIndex(e => e.RecordId);
            entity.HasIndex(e => e.Action);
            entity.HasIndex(e => e.TenantId);
            entity.Property(e => e.TableName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(20);
            entity.Property(e => e.OldValues);
            entity.Property(e => e.NewValues);
            entity.Property(e => e.ChangedColumns);
            entity.Property(e => e.UserId).HasMaxLength(100);
            entity.Property(e => e.UserName).HasMaxLength(200);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.CorrelationId).HasMaxLength(100);
        });
    }
}
