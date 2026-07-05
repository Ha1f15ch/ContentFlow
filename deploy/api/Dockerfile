FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ContentFlow.sln ./
COPY ContentFlow.Domain/ContentFlow.Domain.csproj ContentFlow.Domain/
COPY ContentFlow.Application/ContentFlow.Application.csproj ContentFlow.Application/
COPY ContentFlow.Infrastructure/ContentFlow.Infrastructure.csproj ContentFlow.Infrastructure/
COPY ContentFlow.Web/ContentFlow.Web.csproj ContentFlow.Web/

RUN dotnet restore ContentFlow.Web/ContentFlow.Web.csproj

COPY . .
RUN dotnet publish ContentFlow.Web/ContentFlow.Web.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*

RUN mkdir -p /app/wwwroot/uploads/avatars /app/Logs

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ContentFlow.Web.dll"]
