using Microsoft.EntityFrameworkCore;
using APIFlashCard.Data;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Konfiguracja Kestrel do u�ywania zar�wno HTTP, jak i HTTPS
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5278); // Port HTTP
    options.ListenAnyIP(7190, listenOptions => // Port HTTPS
    {
        listenOptions.UseHttps(); // Aktywuj HTTPS
    });
});

// Add services to the container.
builder.Services.AddControllers();

// Dodaj konfiguracj� dla Entity Framework Core i po��czenia z baz� danych
builder.Services.AddDbContext<FlashCardDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(5, 7, 32))
    )
);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Usu� t� lini�, je�li nie chcesz korzysta� z HTTPS

app.UseCors("AllowAllOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();
