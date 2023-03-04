using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapPost("/", () => new {username="Bath Towel", age="42"});
app.MapGet(
    "add-header/",
    (HttpResponse response) => {
        response.Headers.Add("Number", "42");
        return new {username="Bath Towel", age="42"};
    }
);

app.MapPost(
    "save-product/",
    (Product product) => {
        return product.code + " - " + product.name;
    }
);


//api.app.com/users?data-start={date}&date-end={date}
app.MapGet(
    "get-product/",
    ([FromQuery] string date_start, [FromQuery] string date_end) => {
        return date_start + " - " + date_end;
    }
);
//api.app.com/user/{code}
app.MapGet(
    "get-product/{code}/",
    ([FromRoute] string code) => {
        return code;
    }
);

app.MapGet(
    "get-product-by-header/",
    (HttpRequest request) => {
        return request.Headers["product_code"].ToString();
    }
);

app.Run();


public class Product{
    public string? code { get; set; }
    public string? name { get; set; }
}
