using be_tabloidnews.Models;
using be_tabloidnews.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<NewsDatabaseSetting>(builder.Configuration.GetSection(nameof(NewsDatabaseSetting)));
builder.Services.AddSingleton<INewsDatabaseSettings>(sp=> sp.GetRequiredService<IOptions<NewsDatabaseSetting>>().Value);
builder.Services.Configure<UserDatabaseSetting>(builder.Configuration.GetSection(nameof(UserDatabaseSetting)));
builder.Services.AddSingleton<IUserDatabaseSettings>(sp=> sp.GetRequiredService<IOptions<UserDatabaseSetting>>().Value);
builder.Services.AddSingleton<IMongoClient>(s => new MongoClient(builder.Configuration.GetValue<string>("NewsDatabaseSetting:ConnectionString")));
builder.Services.AddScoped<IImageUploadService, ImageUploadService>();
builder.Services.AddScoped<INewsService,NewsService>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
