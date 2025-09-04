CREATE TABLE IF NOT EXISTS "InvoiceStatusHistory" (
    "Id" bigserial NOT NULL,
    "InvoiceId" character varying(64) NOT NULL,
    "ExternalInvoiceNumber" character varying(128),
    "EventType" character varying(32) NOT NULL,
    "StatusFrom" character varying(32),
    "StatusTo" character varying(32),
    "SystemCode" character varying(8),
    "OccurredAtUtc" timestamp with time zone NOT NULL,
    "LatencyMs" bigint,
    "EventKey" character varying(128),
    "Simulation" boolean NOT NULL,
    CONSTRAINT "PK_InvoiceStatusHistory" PRIMARY KEY ("Id")
);

CREATE INDEX IF NOT EXISTS "IX_InvoiceStatusHistory_InvoiceId" ON "InvoiceStatusHistory" ("InvoiceId");
CREATE INDEX IF NOT EXISTS "IX_InvoiceStatusHistory_EventType_SystemCode" ON "InvoiceStatusHistory" ("EventType", "SystemCode");
CREATE INDEX IF NOT EXISTS "IX_InvoiceStatusHistory_OccurredAtUtc" ON "InvoiceStatusHistory" ("OccurredAtUtc");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_InvoiceStatusHistory_InvoiceId_EventKey" ON "InvoiceStatusHistory" ("InvoiceId", "EventKey") WHERE "EventKey" IS NOT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion") 
VALUES ('20250904101931_Add_InvoiceStatusHistory', '9.0.8');
