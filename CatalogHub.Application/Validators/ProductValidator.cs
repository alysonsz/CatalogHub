using CatalogHub.Domain.Models;

namespace CatalogHub.Application.Validators;

public static class ProductValidator
{
    public static void ValidateForCreate(Product product)
    {
        if (string.IsNullOrWhiteSpace(product.Name))
            throw new ArgumentException("O nome do produto é obrigatório.");
        if (product.Price < 0)
            throw new ArgumentException("O preço do produto não pode ser negativo.");
        if (product.StockQuantity < 0)
            throw new ArgumentException("A quantidade em estoque não pode ser negativa.");
        if (product.CategoryId == Guid.Empty)
            throw new ArgumentException("Categoria inválida.");
    }

    public static void ValidateForUpdate(Product product)
    {
        if (product.Id == Guid.Empty)
            throw new ArgumentException("Produto inválido.");
        if (string.IsNullOrWhiteSpace(product.Name))
            throw new ArgumentException("O nome do produto é obrigatório.");
        if (product.Price < 0)
            throw new ArgumentException("O preço do produto não pode ser negativo.");
        if (product.StockQuantity < 0)
            throw new ArgumentException("A quantidade em estoque não pode ser negativa.");
        if (product.CategoryId == Guid.Empty)
            throw new ArgumentException("Categoria inválida.");
    }
}
