using Microsoft.EntityFrameworkCore;
using APIFlashCard.Data;
using APIFlashCard.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//Webio.pl Db
//builder.Services.AddDbContext<FlashCardDbContext>(options =>
//    options.UseMySql(
//        builder.Configuration.GetConnectionString("WebioDb"),
//        new MySqlServerVersion(new Version(5, 7, 32))
//    )
//);

// LocalDb
builder.Services.AddDbContext<FlashCardDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("FiszkiApp")
    )
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ErrorLoggingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();