-- Türkçe: DLQ/Retry detayları için Outbox/Inbox genişletmeleri (opsiyonel; zaten Attempt sütunu vardı)
ALTER TABLE "OutboxMessages" ADD COLUMN IF NOT EXISTS "LastError" text NULL;
ALTER TABLE "InboxMessages"  ADD COLUMN IF NOT EXISTS "Headers"   text NULL;
