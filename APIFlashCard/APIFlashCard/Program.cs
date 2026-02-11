using Microsoft.EntityFrameworkCore;
using APIFlashCard.Data;
using APIFlashCard.Middleware;
using Microsoft.Extensions.Hosting;
using Coravel;
using Coravel.Scheduling.Schedule;
using APIFlashCard.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<FlashCardDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("FiszkiApp")
    )
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScheduler();
builder.Services.AddTransient<DeleteFalseQrTokenJob>();

builder.Services.AddAuthentication("MyCookie")
    .AddCookie("MyCookie", options =>
    {
        options.LoginPath = "/html/login.html";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.Services.UseScheduler(scheduler =>
{
    scheduler.Schedule<DeleteFalseQrTokenJob>().Hourly();
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ErrorLoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    context.Response.Headers["Cache-Control"] = "no-store";

    if (context.Request.Path.HasValue && context.Request.Path.Value.EndsWith("/html/index.html"))
    {
        if (!context.User.Identity.IsAuthenticated)
        {
            context.Response.Redirect("/html/login.html");
            return;
        }
    }

    await next();
});

app.UseStaticFiles();

app.MapControllers();

app.Run();