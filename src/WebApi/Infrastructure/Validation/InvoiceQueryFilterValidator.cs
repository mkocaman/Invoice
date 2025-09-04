// Türkçe: Sayfalama ve filtre doğrulaması.
using FluentValidation;
using WebApi.Contracts.Invoices;

namespace WebApi.Infrastructure.Validation
{
    public sealed class InvoiceQueryFilterValidator : AbstractValidator<InvoiceQueryFilter>
    {
        public InvoiceQueryFilterValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1).WithMessage("Sayfa numarası 1 veya daha büyük olmalıdır.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 200).WithMessage("Sayfa boyutu 1 ile 200 arasında olmalıdır.");

            // Türkçe: Sort/Filter için basit uzunluk kısıtları
            RuleFor(x => x.Sort).MaximumLength(64);
            RuleFor(x => x.Filter).MaximumLength(256);
        }
    }
}
