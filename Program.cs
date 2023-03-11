using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost(
    "save-product/",
    (Product product) => ProductRepository.Add(product)
);

app.MapGet(
    "get-product/{code}/",
    ([FromRoute] string code) => ProductRepository.GetBy(code)
);

app.MapPut(
    "edit-product/",
    (Product product) => {
        var productSaved = ProductRepository.GetBy(product.Code);
        productSaved.Name = product.Name;
    }
);

app.MapDelete(
    "delete-product/{code}",
    ([FromRoute] string code) => {
        var productSaved = ProductRepository.GetBy(code);
        ProductRepository.Remove(productSaved);
    }
);

app.Run();

public static class ProductRepository {
    public static List<Product> Products { get; set; }

    public static void Add(Product product){
        if(Products == null)
            Products = new List<Product>();

        Products.Add(product);
    }

    public static Product GetBy(string code){
        return Products.FirstOrDefault(p => p.Code == code);
    }

    public static void Remove(Product product){
        Products.Remove(product);
    }

}


public class Product{
    public string Code { get; set; }
    public string Name { get; set; }
}
