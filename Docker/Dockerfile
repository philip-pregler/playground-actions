FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["../webapi/webapi.csproj", "webapi/"]
COPY ["../webapi.test/webapi.test.csproj", "webapi.test/"]
RUN dotnet restore "webapi/webapi.csproj"
RUN dotnet restore "webapi.test/webapi.test.csproj"
COPY .. .
WORKDIR "/src/webapi"
RUN dotnet build "webapi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "webapi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "webapi.dll"]