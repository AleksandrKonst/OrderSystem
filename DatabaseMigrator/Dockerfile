FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy and restore the project
COPY ["DatabaseMigrator/DatabaseMigrator.csproj", "DatabaseMigrator/"]
RUN dotnet restore "DatabaseMigrator/DatabaseMigrator.csproj"

# Copy the rest of the code
COPY ["DatabaseMigrator/", "DatabaseMigrator/"]

# Build the project
RUN dotnet build "DatabaseMigrator/DatabaseMigrator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DatabaseMigrator/DatabaseMigrator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DatabaseMigrator.dll"] 