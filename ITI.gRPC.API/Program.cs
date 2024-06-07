
using ITI.gRPC.API.Interfaces;
using ITI.gRPC.API.Protos;
using ITI.gRPC.API.Services;

namespace ITI.gRPC.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<ApiProductService>();
            builder.Services.AddGrpcClient<productService.productServiceClient>(options =>
            {
                options.Address = new Uri("https://localhost:7213");
            }).AddCallCredentials((context, metadata, serviceProvider) =>
            {
                var apiKeyProvider = serviceProvider.GetRequiredService<IApiKeyProviderService>();
                var apiKey = apiKeyProvider.GetApiKey();
                metadata.Add("X-Api-Key", apiKey);
                return Task.CompletedTask;
            });
            builder.Services.AddScoped<IApiKeyProviderService, ApiKeyProviderService>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
