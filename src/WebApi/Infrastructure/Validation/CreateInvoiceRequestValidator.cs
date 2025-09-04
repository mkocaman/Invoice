// Türkçe: CreateInvoiceRequest için doğrulama kuralları.
using FluentValidation;
using WebApi.Contracts.Invoices;

namespace WebApi.Infrastructure.Validation
{
    public sealed class CreateInvoiceRequestValidator : AbstractValidator<CreateInvoiceRequest>
    {
        public CreateInvoiceRequestValidator()
        {
            // Türkçe: Müşteri kimliği zorunlu
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("Müşteri kimliği zorunludur.")
                .MaximumLength(64).WithMessage("Müşteri kimliği en fazla 64 karakter olabilir.");

            // Türkçe: Para birimi 3 karakter ISO kodu
            RuleFor(x => x.Currency)
                .NotEmpty().Length(3).WithMessage("Para birimi ISO 4217 3 karakter olmalıdır.");

            // Türkçe: Tutar pozitif olmalı
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Tutar 0'dan büyük olmalıdır.");
        }
    }
}
