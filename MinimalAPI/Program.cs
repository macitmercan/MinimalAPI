// Minimal API - Giriş ve Swagger Kullanımı

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();

//Db Bağlantısı İçin Ekledik.
builder.Services.AddDbContext<CustomerDb>(options => options.UseInMemoryDatabase("CustomerDb"));

builder.Services.AddSwaggerGen();

var app = builder.Build();

//appsettings.json dosyasından JWT'yi okuduk.
app.Logger.LogInformation($"{app.Configuration["JWT:Key"]}");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minimal API V1");
});

//GET ISLEMLERI
app.MapGet("/", () => "Hello World !");

app.MapGet("/customers", async (CustomerDb db) => await db.Customers.ToListAsync());

app.MapGet("/customers/{id}", async (CustomerDb db, int id) => await db.Customers.FindAsync(id));

//POST ISLEMI
app.MapPost("/customers", async (CustomerDb db, Customer cus) =>
{
    await db.Customers.AddAsync(cus);
    await db.SaveChangesAsync();

    return Results.Created($"/customers/{cus.Id}", cus);
});


//UPDATE ISLEMI
app.MapPut("/customers/{id}", async (CustomerDb db, Customer newcustomer, int id) =>
{
    var current = await db.Customers.FindAsync(id);

    if (current == null)
    {
        return Results.NotFound();
    }

    current.Name = newcustomer.Name;
    await db.SaveChangesAsync();

    return Results.NoContent();

});

//SILME ISLEMI
app.MapDelete("/customers/{id}", async (CustomerDb db, int id) =>
{
    var current = await db.Customers.FindAsync(id);

    if (current == null)
    {
        return Results.NotFound();
    }

    db.Customers.Remove(current);
    await db.SaveChangesAsync();

    return Results.Ok();

});

app.Run();


//IN MEMORY DB
class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
}

class CustomerDb : DbContext
{
    public CustomerDb(DbContextOptions options) : base(options) { }

    public DbSet<Customer> Customers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.UseInMemoryDatabase("CustomerDb");
    }
}
