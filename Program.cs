using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>();

var app = builder.Build();
var configuration = app.Configuration;
ProductRepository.Init(configuration);


app.MapPost(
    "products/",
    (Product product) => {
        ProductRepository.Add(product);
        return Results.Created($"products/{product.Code}" , product.Code);
    }
);


app.MapGet(
    "products/{code}/",
    ([FromRoute] string code) => {
        var product = ProductRepository.GetBy(code);
        if(product == null)
            return Results.NotFound();
        return Results.Ok(product);
    }
);


app.MapPut(
    "products/",
    (Product product) => {
        var productSaved = ProductRepository.GetBy(product.Code);
        productSaved.Name = product.Name;
        return Results.Ok();
    }
);


app.MapDelete(
    "products/{code}",
    ([FromRoute] string code) => {
        var productSaved = ProductRepository.GetBy(code);
        ProductRepository.Remove(productSaved);
        return Results.Ok();
    }
);


if(app.Environment.IsStaging())
    app.MapGet(
        "configuration/database/",
        (IConfiguration configuration) => {
            return Results.Ok($"{configuration["database:connection"]}/{configuration["database:port"]}");
        }
    );


app.Run();


public static class ProductRepository {
    public static List<Product> Products { get; set; } = Products = new List<Product>();

    public static void Init(IConfiguration configuration){
        var products = configuration.GetSection("Products").Get<List<Product>>();
        Products = products;
    }

    public static void Add(Product product) => Products.Add(product);

    public static Product GetBy(string code) => Products.FirstOrDefault(p => p.Code == code);

    public static void Remove(Product product) => Products.Remove(product);
}


public class Category{
    public int Id { get; set; }
    public string Name { get; set; }
}


public class Tag{
    public int Id { get; set; }
    public string Name { get; set; }
    public int ProductId { get; set; }
}


public class Product{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public List<Tag> Tags { get; set; }
}


public class ApplicationDbContext: DbContext{
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder builder){
        builder.Entity<Product>().Property(p => p.Code).HasMaxLength(20).IsRequired();
        builder.Entity<Product>().Property(p => p.Name).HasMaxLength(120).IsRequired();
        builder.Entity<Product>().Property(p => p.Description).HasMaxLength(500).IsRequired(false);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer(
            "Server=localhost;Database=Products;User Id=sa;Password=@Sql2019;MultipleActiveResultSets=true;Encrypt=YES;TrustServerCertificate=YES"
        );
}
