using Microsoft.EntityFrameworkCore;
using APIFlashCard.Data;
using APIFlashCard.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<FlashCardDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("AzureDb")
    )
);

//builder.Services.AddDbContext<FlashCardDbContext>(options =>
//    options.UseSqlServer(
//        builder.Configuration.GetConnectionString("FiszkiApp")
//    )
//);

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

app.UseStaticFiles();

app.UseMiddleware<ErrorLoggingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();