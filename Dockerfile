# ======================
# 1) Build / Publish
# ======================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

ARG BUILD_CONFIGURATION=Release

# Copiamos la solución
COPY BolivarianoBank.sln ./

# Copiamos SOLO los csproj (mejora la caché del restore)
COPY ApiCore/*.csproj ApiCore/
COPY BankCore.Integration/*.csproj BankCore.Integration/
COPY Common.WebApi/*.csproj Common.WebApi/
COPY LogicApi/*.csproj LogicApi/
COPY Persistence/*.csproj Persistence/

# Restauramos dependencias del proyecto ejecutable
RUN dotnet restore "ApiCore/ApiCore.csproj"

# Copiamos el resto del código
COPY . ./

# Publicamos el ejecutable (framework-dependent, recomendado para contenedores)
RUN dotnet publish "ApiCore/ApiCore.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# ======================
# 2) Runtime
# ======================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# En imágenes oficiales recientes, ASP.NET Core típicamente escucha en 8080 por defecto
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "ApiCore.dll"]