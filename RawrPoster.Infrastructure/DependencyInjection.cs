

using Microsoft.Extensions.DependencyInjection;
using RawrPoster.Application.Services;

namespace RawrPoster.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Register your infrastructure services here
            // For example:
            // services.AddSingleton<ITelegramPublisher, TelegramPublisher>();
            services.AddTransient<PostService>();
            return services;
        }
    }
}
