using Application.Infrastructure;
using DataAccess;
using DataAccess.Infrastructure;
using DataAccess.Seeding;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["ConnectionStrings:DbContext"]!;

builder.Services.AddDbContext<MeterReadingsDbContext>(options => options.UseSqlServer(connectionString));

ConfigureDataAccess.ConfigureServices(builder.Services);
ConfigureApplication.ConfigureServices(builder.Services);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    SeedData.Seed(app.Services.CreateScope().ServiceProvider);
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
