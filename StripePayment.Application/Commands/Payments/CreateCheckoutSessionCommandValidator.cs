using FluentValidation;

namespace StripePayment.Application.Commands.Payments;

public sealed class CreateCheckoutSessionCommandValidator : AbstractValidator<CreateCheckoutSessionCommand>
{
    public CreateCheckoutSessionCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required")
            .Must(BeValidGuid)
            .WithMessage("Customer ID must be a valid GUID");

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required")
            .Must(BeValidGuid)
            .WithMessage("Product ID must be a valid GUID");

        When(x => !string.IsNullOrEmpty(x.SuccessUrl), () =>
        {
            RuleFor(x => x.SuccessUrl)
                .Must(BeValidUrl!)
                .WithMessage("Success URL must be a valid URL");
        });

        When(x => !string.IsNullOrEmpty(x.CancelUrl), () =>
        {
            RuleFor(x => x.CancelUrl)
                .Must(BeValidUrl!)
                .WithMessage("Cancel URL must be a valid URL");
        });
    }

    private static bool BeValidGuid(string value)
    {
        return Guid.TryParse(value, out _);
    }

    private static bool BeValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}