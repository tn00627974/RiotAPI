using LolTeamTracker.Api;
using LolTeamTracker.Api.Services;
using Microsoft.OpenApi.Models; 
using System.Reflection;
using Westwind.AspNetCore.LiveReload;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddLiveReload();
builder.Services.AddHttpClient<RiotApiService>(); // 註冊 RiotApiService 以便在 MatchController 中使用
builder.Services.AddHttpClient<RiotDataDownloader>(); // 註冊 RiotDataDownloader 以便在 MatchController 中使用

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// Swagger 中介軟體
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Swagger 文件的 API資訊與描述
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "20250712v1",
        Title = "RIOT API",
        Description = "An ASP.NET Core Web API for LolTeamTracker", 
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact 
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });

    // Swagger API網址加入XML註解
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseLiveReload(); // 加這行
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseReDoc(options =>
    {
        options.RoutePrefix = "redoc"; // 最終網址 http://localhost:xxxx/redoc
    });
    app.UseDeveloperExceptionPage(); // 顯示詳細錯誤
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
