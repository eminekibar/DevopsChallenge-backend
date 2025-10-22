# 1. Build aşaması (SDK ile derleme)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src 
COPY *.csproj ./ 
RUN dotnet restore backend.csproj
COPY . .
RUN dotnet publish backend.csproj -c Release -o /app/publish

# 2. Çalıştırma aşaması (daha küçük runtime imajı)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS http://*:52369
ENV ASPNETCORE_ENVIRONMENT Development
ENTRYPOINT ["dotnet", "backend.dll"]
