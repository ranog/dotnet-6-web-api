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
