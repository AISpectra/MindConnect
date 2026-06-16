FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY MindConnect.csproj ./
RUN dotnet restore MindConnect.csproj

COPY . ./
RUN dotnet publish MindConnect.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT:-10000}
EXPOSE 10000

COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "MindConnect.dll"]
