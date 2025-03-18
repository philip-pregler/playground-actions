# Etapa 1: Build (compilación)
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Establecer el directorio de trabajo en el contenedor
WORKDIR /src

# Copiar los archivos .csproj de WebAPI y WebAPITest, y restaurar dependencias
COPY webapi/webapi.csproj webapi/
COPY webapi.test/webapi.test.csproj webapi.test/
RUN dotnet restore webapi/webapi.csproj
RUN dotnet restore webapi.test/webapi.test.csproj

# Copiar todo el código fuente
COPY . .

# Publicar la aplicación WebAPI en la carpeta bin/Release
RUN dotnet publish "webapi/webapi.csproj" -c Release -o /app/publish

# Ejecutar las pruebas
RUN dotnet test "webapi.test/webapi.test.csproj" --no-build --verbosity normal

# Etapa 2: Ejecutar la WebAPI
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:9.0 AS base

# Establecer el directorio de trabajo en la imagen base
WORKDIR /app

# Copiar los archivos compilados desde la etapa de build
COPY --from=build /app/publish .

# Exponer el puerto donde la API estará disponible
EXPOSE 80

# Configurar el comando para ejecutar la WebAPI
ENTRYPOINT ["dotnet", "webapi.dll"]