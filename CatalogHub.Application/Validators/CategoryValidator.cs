using CatalogHub.Domain.Models;

namespace CatalogHub.Application.Validators;

public static class CategoryValidator
{
    public static void ValidateForCreate(Category category)
    {
        if (string.IsNullOrWhiteSpace(category.Name))
            throw new ArgumentException("O nome da categoria é obrigatório.");
    }

    public static void ValidateForUpdate(Category category)
    {
        if (category.Id == Guid.Empty)
            throw new ArgumentException("Categoria inválida.");
        if (string.IsNullOrWhiteSpace(category.Name))
            throw new ArgumentException("O nome da categoria é obrigatório.");
    }
}
