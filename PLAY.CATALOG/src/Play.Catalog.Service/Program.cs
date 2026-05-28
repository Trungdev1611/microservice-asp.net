using Play.Catalog.Service.Entities;
using Play.Common.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMongo().AddMongoRepository<Item>("items");


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

