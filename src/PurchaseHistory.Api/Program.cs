using PurchaseHistory.Domain.Interfaces;
using PurchaseHistory.Application.UseCases.GetProducts;
using PurchaseHistory.Application.UseCases.UploadCouponHtml;
using PurchaseHistory.Infrastructure.Parsers;
using PurchaseHistory.Infrastructure.Repositories;
using PurchaseHistory.Infrastructure.Data;
using PurchaseHistory.Domain.Interfaces.Repositories;
using PurchaseHistory.Application.UseCases.SearchProducts;
using PurchaseHistory.Application.UseCases.GetProductDetails;
using PurchaseHistory.Application.UseCases.ApplyProductNormalization;
using PurchaseHistory.Domain.Interfaces.Services;
using PurchaseHistory.Infrastructure.Services;


var builder = WebApplication.CreateBuilder(args);

#region Services

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

#endregion

#region Infrastructure

builder.Services.AddScoped<DbConnectionFactory>();
builder.Services.AddScoped<ICouponHtmlParser, NfceHtmlParser>();
builder.Services.AddScoped<IStoreRepository, StoreRepository>();
builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();
builder.Services.AddScoped<IPurchaseItemRepository, PurchaseItemRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductNormalizationMappingRepository, ProductNormalizationMappingRepository>();
builder.Services.AddScoped<IProductKeywordSubstitutionRepository, ProductKeywordSubstitutionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICouponImportRepository, CouponImportRepository>();
builder.Services.AddScoped<IProductNormalizationService, ProductNormalizationService>();


#endregion

#region UseCases

builder.Services.AddScoped<UploadCouponHtmlUseCase>();
builder.Services.AddScoped<GetProductsUseCase>();
builder.Services.AddScoped<SearchProductsUseCase>();
builder.Services.AddScoped<GetProductDetailsUseCase>();
builder.Services.AddScoped<ApplyProductNormalizationUseCase>();

#endregion

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFlutter",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var port = Environment.GetEnvironmentVariable("PORT") ?? "5299";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();

#region Middleware

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseCors("AllowFlutter");

app.UseAuthorization();

#endregion

#region Endpoints

app.MapControllers();

#endregion

app.Run();