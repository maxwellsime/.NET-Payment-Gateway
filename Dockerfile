FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
ARG BUILD_CONFIGURATION=Release
COPY ["src/PaymentGateway.Api/PaymentGateway.Api.csproj", "PaymentGateway.Api/"]
RUN dotnet restore "PaymentGateway.Api/PaymentGateway.Api.csproj"

COPY ["src/PaymentGateway.Api", "PaymentGateway.Api/"]
WORKDIR /src/PaymentGateway.Api
RUN dotnet build "PaymentGateway.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PaymentGateway.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 5000
EXPOSE 7092
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentGateway.Api.dll"]