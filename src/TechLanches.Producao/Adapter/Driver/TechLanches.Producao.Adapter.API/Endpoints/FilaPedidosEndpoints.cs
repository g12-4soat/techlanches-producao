﻿using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using TechLanches.Producao.Adapter.API.Constantes;
using TechLanches.Producao.Application.Controllers.Interfaces;
using TechLanches.Producao.Application.DTOs;
using TechLanches.Producao.Domain.Enums;

namespace TechLanches.Producao.Adapter.API.Endpoints
{
    public static class FilaPedidosEndpoints
    {
        public static void MapFilaPedidoEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("api/filapedidos", RetornarFilaPedidos)
               .WithTags(EndpointTagConstantes.TAG_FILA_PEDIDO)
               .WithMetadata(new SwaggerOperationAttribute(summary: "Obter todos os pedidos da fila", description: "Retorna todos os pedidos contidos na fila"))
               .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.OK, type: typeof(List<PedidoResponseDTO>), description: "Pedidos da fila encontrados com sucesso"))
               .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.BadRequest, type: typeof(ErrorResponseDTO), description: "Requisição inválida"))
               .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.NotFound, type: typeof(ErrorResponseDTO), description: "Pedidos da fila não encontrados"))
               .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.InternalServerError, type: typeof(ErrorResponseDTO), description: "Erro no servidor interno"));
        }

        private static async Task<IResult> RetornarFilaPedidos(
            [FromServices] IFilaPedidoController filaPedidoController)
        {
            var pedidos = await filaPedidoController.BuscarPorStatus(StatusPedido.PedidoEmPreparacao);
            return pedidos is not null
                ? Results.Ok(pedidos)
                : Results.BadRequest(new ErrorResponseDTO { MensagemErro = "Erro ao retornar fila pedido.", StatusCode = HttpStatusCode.BadRequest });
        }
    }
}
