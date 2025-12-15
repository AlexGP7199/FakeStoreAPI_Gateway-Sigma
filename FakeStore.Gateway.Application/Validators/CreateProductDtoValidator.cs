using FluentValidation;
using FakeStore.Gateway.Application.DTOs;

namespace FakeStore.Gateway.Application.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título es requerido")
            .MaximumLength(200).WithMessage("El título no puede exceder 200 caracteres");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("El precio debe ser mayor a 0");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es requerida")
            .MaximumLength(1000).WithMessage("La descripción no puede exceder 1000 caracteres");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("La categoría es requerida")
            .MaximumLength(100).WithMessage("La categoría no puede exceder 100 caracteres");

        RuleFor(x => x.Image)
            .NotEmpty().WithMessage("La imagen es requerida")
            .Must(BeAValidUrl).WithMessage("La URL de la imagen no es válida");
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
