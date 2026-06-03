FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY PurchaseHistory.sln ./
COPY src/PurchaseHistory.Domain/PurchaseHistory.Domain.csproj src/PurchaseHistory.Domain/
COPY src/PurchaseHistory.Application/PurchaseHistory.Application.csproj src/PurchaseHistory.Application/
COPY src/PurchaseHistory.Infrastructure/PurchaseHistory.Infrastructure.csproj src/PurchaseHistory.Infrastructure/
COPY src/PurchaseHistory.Api/PurchaseHistory.Api.csproj src/PurchaseHistory.Api/

RUN dotnet restore src/PurchaseHistory.Api/PurchaseHistory.Api.csproj

COPY . .
RUN dotnet publish src/PurchaseHistory.Api/PurchaseHistory.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 5299

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PurchaseHistory.Api.dll"]
