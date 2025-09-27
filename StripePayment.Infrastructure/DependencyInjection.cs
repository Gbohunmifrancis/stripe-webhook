using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StripePayment.Application.Interfaces;
using StripePayment.Infrastructure.Configuration;
using StripePayment.Infrastructure.Persistence;
using StripePayment.Infrastructure.Services;

namespace StripePayment.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database - Use InMemory for demo purposes (LocalDB not available)
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("StripePaymentDb"));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        // Stripe Configuration
        services.Configure<StripeOptions>(
            configuration.GetSection(StripeOptions.SectionName));

        // Payment Service
        services.AddScoped<IPaymentService, StripePaymentService>();

        return services;
    }
}