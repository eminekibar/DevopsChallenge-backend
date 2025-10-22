# 1. Build aşaması (SDK ile derleme)
# SDK imajını kullanarak build aşamasını başlat
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Çalışma dizinini ayarla
WORKDIR /src 
# Proje dosyasını kopyala
COPY *.csproj ./ 
# Bağımlılıkları geri yükle
RUN dotnet restore backend.csproj
# Tüm kaynak kodunu kopyala
COPY . .
# Uygulamayı yayınla (publish)
RUN dotnet publish backend.csproj -c Release -o /app/publish

# 2. Çalıştırma aşaması (daha küçük runtime imajı)
# Runtime imajını kullanarak çalıştırma aşamasını başlat
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
# Çalışma dizinini ayarla
WORKDIR /app
# Build aşamasından yayınlanan dosyaları kopyala
COPY --from=build /app/publish .
# Ortam değişkenlerini ayarla
ENV ASPNETCORE_URLS http://*:8080
# Uygulama ortamını Development olarak ayarla
ENV ASPNETCORE_ENVIRONMENT Development
# Uygulamayı başlat
ENTRYPOINT ["dotnet", "backend.dll"]
