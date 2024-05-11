FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Adapter/Driven/TechLanches.Producao.Adapter.AWS/TechLanches.Producao.Adapter.AWS.csproj", "Adapter/Driven/TechLanches.Producao.Adapter.AWS/"]
COPY ["Adapter/Driven/TechLanches.Producao.Adapter.RabbitMq/TechLanches.Producao.Adapter.RabbitMq.csproj", "Adapter/Driven/TechLanches.Producao.Adapter.RabbitMq/"]
COPY ["Adapter/Driver/TechLanches.Producao.Adapter.FilaPedidos/TechLanches.Producao.Adapter.FilaPedidos.csproj", "Adapter/Driver/TechLanches.Producao.Adapter.FilaPedidos/"]
COPY ["Core/TechLanches.Producao.Application/TechLanches.Producao.Application.csproj", "Core/TechLanches.Producao.Application/"]
COPY ["Core/TechLanches.Producao.Core/TechLanches.Producao.Core.csproj", "Core/TechLanches.Producao.Core/"]
COPY ["Core/TechLanches.Producao.Domain/TechLanches.Producao.Domain.csproj", "Core/TechLanches.Producao.Domain/"]
COPY ["Adapter/Driver/TechLanches.Producao.Adapter.API/TechLanches.Producao.Adapter.API.csproj", "Adapter/Driver/TechLanches.Producao.Adapter.API/"]
RUN dotnet restore "./Adapter/Driver/TechLanches.Producao.Adapter.API/TechLanches.Producao.Adapter.API.csproj"
COPY . .
WORKDIR "/src/Adapter/Driver/TechLanches.Producao.Adapter.API"
RUN dotnet build "./TechLanches.Producao.Adapter.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./TechLanches.Producao.Adapter.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechLanches.Producao.Adapter.API.dll"]