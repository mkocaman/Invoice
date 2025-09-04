CREATE TABLE IF NOT EXISTS "InvoiceAudits" (
    "Id" bigserial NOT NULL,
    "InvoiceId" character varying(64),
    "ExternalInvoiceNumber" character varying(128),
    "EventType" character varying(32) NOT NULL,
    "StatusFrom" character varying(32),
    "StatusTo" character varying(32),
    "SystemCode" character varying(8),
    "CorrelationId" character varying(64),
    "TraceId" character varying(64),
    "XmlPayload" text,
    "JsonPayload" text,
    "RequestBody" text,
    "ResponseBody" text,
    "RedactionNotes" character varying(256),
    "XmlSha256" character varying(64),
    "JsonSha256" character varying(64),
    "RequestSha256" character varying(64),
    "ResponseSha256" character varying(64),
    "Simulation" boolean NOT NULL,
    "Actor" character varying(128),
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    "Notes" character varying(512),
    CONSTRAINT "PK_InvoiceAudits" PRIMARY KEY ("Id")
);

CREATE INDEX IF NOT EXISTS "IX_InvoiceAudits_InvoiceId" ON "InvoiceAudits" ("InvoiceId");
CREATE INDEX IF NOT EXISTS "IX_InvoiceAudits_EventType_SystemCode" ON "InvoiceAudits" ("EventType", "SystemCode");
CREATE INDEX IF NOT EXISTS "IX_InvoiceAudits_CreatedAtUtc" ON "InvoiceAudits" ("CreatedAtUtc");
CREATE INDEX IF NOT EXISTS "IX_InvoiceAudits_Simulation" ON "InvoiceAudits" ("Simulation");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion") 
VALUES ('20250904101804_InitialCreate', '9.0.8');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion") 
VALUES ('20250904103056_Add_InvoiceAudit', '9.0.8');
