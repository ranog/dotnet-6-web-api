using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["Database:SqlServer"]);

var app = builder.Build();
var configuration = app.Configuration;
ProductRepository.Init(configuration);


app.MapPost(
    "products/",
    (ProductRequest productRequest, ApplicationDbContext context) => {
        var category = context.Categories.Where(category => category.Id == productRequest.CategoryId).First();
        var product =  new Product{
            Code = productRequest.Code,
            Name = productRequest.Name,
            Description = productRequest.Description,
            Category = category
        };
        if(productRequest.Tags != null){
            product.Tags = new List<Tag>();
            foreach (var item in productRequest.Tags)
            {
                product.Tags.Add(new Tag{Name = item});
            }
        }

        context.Products.Add(product);
        context.SaveChanges();
        return Results.Created($"products/{product.Id}" , product.Id);
    }
);


app.MapGet(
    "products/{id}/",
    ([FromRoute] int id, ApplicationDbContext context) => {
        var product = context.Products
            .Include(product => product.Category)
            .Include(product => product.Tags)
            .Where(product => product.Id == id).First();

        if(product == null)
            return Results.NotFound();
        return Results.Ok(product);
    }
);


app.MapPut(
    "products/{id}/",
    ([FromRoute] int id, ProductRequest productRequest, ApplicationDbContext context) => {
        var product = context.Products
            .Include(product => product.Tags)
            .Where(product => product.Id == id).First();
        var category = context.Categories.Where(category => category.Id == productRequest.CategoryId).First();

        product.Code = productRequest.Code;
        product.Name = productRequest.Name;
        product.Description = productRequest.Description;
        product.Category = category;
        if(productRequest.Tags != null){
            product.Tags = new List<Tag>();
            foreach (var item in productRequest.Tags)
            {
                product.Tags.Add(new Tag{Name = item});
            }
        }

        context.SaveChanges();
        return Results.Ok();
    }
);


app.MapDelete(
    "products/{id}",
    ([FromRoute] int id, ApplicationDbContext context) => {
        var product = context.Products.Where(product => product.Id == id).First();

        context.Products.Remove(product);
        context.SaveChanges();
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
