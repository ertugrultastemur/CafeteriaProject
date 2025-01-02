# Build aþamasý için .NET 6 SDK görüntüsünü kullan
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

# Çalýþma dizinini ayarla
WORKDIR /app/cafeteria

# Çözüm dosyasýný kopyala ve restore iþlemini yap
COPY CafeteriaProject.sln ./
COPY WebApi/WebApi.csproj ./WebApi/
COPY Core/Core.csproj ./Core/
COPY Business/Business.csproj ./Business/
COPY Entity/Entity.csproj ./Entity/
COPY DataAccess/DataAccess.csproj ./DataAccess/

# Projelerin baðýmlýlýklarýný çöz
RUN dotnet restore

# Tüm kodlarý kopyala ve derle
COPY . ./
RUN dotnet publish WebApi/WebApi.csproj -c Release -o out

# Runtime aþamasý için .NET 6 ASP.NET görüntüsünü kullan
FROM mcr.microsoft.com/dotnet/aspnet:6.0

# Çalýþma dizinini ayarla
WORKDIR /app/cafeteria

# Derlenmiþ dosyalarý kopyala
COPY --from=build-env /app/cafeteria/out .

# Uygulamayý baþlat
ENTRYPOINT ["dotnet", "WebApi.dll"]
