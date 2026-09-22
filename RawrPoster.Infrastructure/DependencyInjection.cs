

using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using RawrPoster.Application.Interfaces;
using RawrPoster.Application.Services;
using RawrPoster.Infrastructure.FuzzySearch;
using RawrPoster.Infrastructure.Persistence;
using RawrPoster.Infrastructure.Services;
using RawrPoster.Infrastructure.Storage;
using RawrPoster.Infrastructure.Telegram;
using Serilog;
using Telegram.Bot;

namespace RawrPoster.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RawrPoster");
            Directory.CreateDirectory(appData);
            services.AddDbContext<RawrPosterDbContext>(options => options.UseSqlite($"Data Source={Path.Combine(appData, "rawrposter.db")}"));
            services.AddHttpClient<IImageSourceSearch, FuzzySearchImageSourceSearch>(client => client.Timeout = TimeSpan.FromSeconds(30));
            services.AddSingleton<IFileStorage, LocalFileStorage>();
            services.AddScoped<ITemplateService, TemplateService>();
            services.AddScoped<IHashtagService, HashtagService>();
            services.AddSingleton<ITelegramPublisher, TelegramPublisher>();
            services.AddTransient<PostService>();
            return services;
        }
    }
}
