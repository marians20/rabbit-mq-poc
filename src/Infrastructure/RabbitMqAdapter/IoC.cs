using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RabbitMqAdapter
{
    public static class IoC
    {
        public static IServiceCollection AddRabbitMqDataAdapter(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection("RabbitMqSettings").Get<RabbitMqSettings>();
            services.AddSingleton(settings);
            return services.AddScoped<IRabbitMqAdapter, RabbitMqAdapter>();
        }
    }
}
