using Infrastructure;
using Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.Configure<PostgresOptions>(
    builder.Configuration.GetSection(PostgresOptions.SectionName));

var postgresOptions = builder.Configuration
    .GetSection(PostgresOptions.SectionName)
    .Get<PostgresOptions>();

builder.Services.AddDbContext<EventManagementDbContext>(options =>
    options.UseNpgsql(postgresOptions!.ConnectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EventManagementDbContext>();
    await db.Database.MigrateAsync();
}


app.Run();
