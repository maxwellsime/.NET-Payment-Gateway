FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
ARG BUILD_CONFIGURATION=Release
COPY ["src/PaymentGateway.Api/PaymentGateway.Api.csproj", "PaymentGateway.Api/"]
RUN dotnet restore "PaymentGateway.Api/PaymentGateway.Api.csproj"

COPY ["src/PaymentGateway.Api", "PaymentGateway.Api/"]
WORKDIR /src/PaymentGateway.Api
RUN dotnet build "PaymentGateway.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaymentGateway.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_HTTP_PORTS=5000
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "PaymentGateway.Api.dll"]