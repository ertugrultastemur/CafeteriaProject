# Build a�amas� i�in .NET 6 SDK g�r�nt�s�n� kullan
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

# �al��ma dizinini ayarla
WORKDIR /app/cafeteria

# ��z�m dosyas�n� kopyala ve restore i�lemini yap
COPY CafeteriaProject.sln ./
COPY WebApi/WebApi.csproj ./WebApi/
COPY Core/Core.csproj ./Core/
COPY Business/Business.csproj ./Business/
COPY Entity/Entity.csproj ./Entity/
COPY DataAccess/DataAccess.csproj ./DataAccess/

# Projelerin ba��ml�l�klar�n� ��z
RUN dotnet restore

# T�m kodlar� kopyala ve derle
COPY . ./
RUN dotnet publish WebApi/WebApi.csproj -c Release -o out

# Runtime a�amas� i�in .NET 6 ASP.NET g�r�nt�s�n� kullan
FROM mcr.microsoft.com/dotnet/aspnet:6.0

# �al��ma dizinini ayarla
WORKDIR /app/cafeteria

# Derlenmi� dosyalar� kopyala
COPY --from=build-env /app/cafeteria/out .

# Uygulamay� ba�lat
ENTRYPOINT ["dotnet", "WebApi.dll"]
