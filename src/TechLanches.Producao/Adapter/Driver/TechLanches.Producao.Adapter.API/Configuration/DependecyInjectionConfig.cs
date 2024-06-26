﻿using Amazon.Lambda;
using TechLanches.Producao.Adapter.RabbitMq.Messaging;
using TechLanches.Producao.Application.Controllers;
using TechLanches.Producao.Application.Controllers.Interfaces;

namespace TechLanches.Producao.Adapter.API.Configuration
{
    public static class DependecyInjectionConfig
    {
        public static void AddDependencyInjectionConfiguration(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IPedidoController, PedidoController>();
            services.AddSingleton<IFilaPedidoController, FilaPedidoController>();
            services.AddSingleton<IRabbitMqService, RabbitMqService>();
            services.AddSingleton<IAmazonLambda, AmazonLambdaClient>();
        }
    }
}
