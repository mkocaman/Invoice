-- Türkçe: Phase 7 tabloları
CREATE TABLE IF NOT EXISTS "InvoiceWorkflows"(
  "Id" uuid PRIMARY KEY,
  "InvoiceId" varchar(64) NOT NULL,
  "CountryCode" varchar(2) NOT NULL,
  "ProviderKey" varchar(32),
  "Status" varchar(32) NOT NULL,
  "CreatedAtUtc" timestamptz NOT NULL,
  "LastUpdatedUtc" timestamptz NULL,
  "LastError" varchar(256) NULL
);
CREATE INDEX IF NOT EXISTS "IX_InvoiceWorkflows_InvoiceId" ON "InvoiceWorkflows"("InvoiceId");

CREATE TABLE IF NOT EXISTS "OutboxMessages"(
  "Id" bigserial PRIMARY KEY,
  "AggregateId" varchar(64) NOT NULL,
  "Type" varchar(64) NOT NULL,
  "Payload" text NOT NULL,
  "CreatedAtUtc" timestamptz NOT NULL,
  "SentAtUtc" timestamptz NULL,
  "Attempt" int NOT NULL DEFAULT 0,
  "Locked" boolean NOT NULL DEFAULT false,
  "LockedUntilUtc" timestamptz NULL
);
CREATE INDEX IF NOT EXISTS "IX_Outbox_Scan" ON "OutboxMessages"("SentAtUtc","Locked","LockedUntilUtc");

CREATE TABLE IF NOT EXISTS "InboxMessages"(
  "Id" bigserial PRIMARY KEY,
  "MessageId" varchar(128) NOT NULL,
  "Type" varchar(64) NOT NULL,
  "Payload" text NOT NULL,
  "ReceivedAtUtc" timestamptz NOT NULL,
  "ProcessedAtUtc" timestamptz NULL,
  "Succeeded" boolean NOT NULL DEFAULT false,
  "Error" varchar(256) NULL
);
CREATE UNIQUE INDEX IF NOT EXISTS "UX_Inbox_MessageId" ON "InboxMessages"("MessageId");

CREATE TABLE IF NOT EXISTS "IdempotencyKeys"(
  "Id" bigserial PRIMARY KEY,
  "Key" varchar(128) NOT NULL,
  "Scope" varchar(64) NOT NULL,
  "Response" text NOT NULL,
  "CreatedAtUtc" timestamptz NOT NULL
);
CREATE UNIQUE INDEX IF NOT EXISTS "UX_Idem_KeyScope" ON "IdempotencyKeys"("Key","Scope");
