using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using ContentFlow.Application.Common.Behaviors;
using FluentValidation;

namespace ContentFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly()); // FluentValidation
        
        // регистрация сервисов
        
        // регистрация pipeline behavior
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        
        return services;
    }
}