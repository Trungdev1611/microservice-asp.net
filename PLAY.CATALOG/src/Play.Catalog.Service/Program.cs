using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Catalog.Service.Repositories;
using Play.Catalog.Service.Setting;

var builder = WebApplication.CreateBuilder(args);


// 1. Đăng ký MongoClient (Kết nối đến Server Docker)
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    // Đọc cụm "MongoDbSettings" từ appsettings.json
    var mongoDbSettings = builder.Configuration.GetSection(("MongoDBSettingJSON")).Get<MongoDBSetting>()!;
    return new MongoClient(mongoDbSettings.ConnectionString);
});

// 2. Đăng ký IMongoDatabase (Chọc vào Database tên là "Catalog")
builder.Services.AddSingleton<IMongoDatabase>(serviceProvider =>
{
    // Đọc cụm "ServiceSettings" để lấy chữ "Catalog"
    var serviceSettings = builder.Configuration.GetSection(("ServiceSettingJSON")).Get<ServiceSetting>()!;
    var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();

    // Ném chữ "Catalog" vào đây để chốt tên DB
    return mongoClient.GetDatabase(serviceSettings.ServiceName);
});

builder.Services.AddSingleton<IItemRepository, ItemRepository>();

//Serializer data Guid và date cho dễ đọc
BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = true; // không tự động cắt đuôi async
});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Lệnh này kích hoạt giao diện Web Swagger tại đường dẫn: /swagger
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Play Catalog API v1");
        options.RoutePrefix = "swagger"; // Định nghĩa đường dẫn truy cập (localhost:xxxx/swagger)
    });
}

app.UseHttpsRedirection();


// DÒNG 2: Kích hoạt tính năng định tuyến cho Controller (Routing)
// ============================================================
app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
