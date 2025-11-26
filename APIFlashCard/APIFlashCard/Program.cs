using Microsoft.EntityFrameworkCore;
using APIFlashCard.Data;
using APIFlashCard.Middleware;
using Microsoft.Extensions.Hosting;
using Coravel;
using Coravel.Scheduling.Schedule;
using APIFlashCard.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//builder.Services.AddDbContext<FlashCardDbContext>(options =>
//    options.UseSqlServer(
//        builder.Configuration.GetConnectionString("AzureDb")
//    )
//);

builder.Services.AddDbContext<FlashCardDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("GoogleDb")
    )
);

//builder.Services.AddDbContext<FlashCardDbContext>(options =>
//    options.UseSqlServer(
//        builder.Configuration.GetConnectionString("FiszkiApp")
//    )
//);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScheduler();
builder.Services.AddTransient<DeleteFalseQrTokenJob>();

var app = builder.Build();

app.Services.UseScheduler(scheduler =>
{
    scheduler.Schedule<DeleteFalseQrTokenJob>().Hourly();
    //scheduler.Schedule<DeleteFalseQrTokenJob>().EveryMinute();
});

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