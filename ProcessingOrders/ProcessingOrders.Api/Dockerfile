FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["ProcessingOrders.Api/ProcessingOrders.Api.csproj", "ProcessingOrders.Api/"]
COPY ["ProcessingOrders.Application/ProcessingOrders.Application.csproj", "ProcessingOrders.Application/"]
COPY ["ProcessingOrders.CoreDomain/ProcessingOrders.CoreDomain.csproj", "ProcessingOrders.CoreDomain/"]
COPY ["ProcessingOrders.Infrastructure/ProcessingOrders.Infrastructure.csproj", "ProcessingOrders.Infrastructure/"]
COPY ["ProcessingOrders.Persistence/ProcessingOrders.Persistence.csproj", "ProcessingOrders.Persistence/"]

RUN dotnet restore "ProcessingOrders.Api/ProcessingOrders.Api.csproj"

COPY . .
WORKDIR "/src/ProcessingOrders.Api"
RUN dotnet build "ProcessingOrders.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProcessingOrders.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProcessingOrders.Api.dll"] 