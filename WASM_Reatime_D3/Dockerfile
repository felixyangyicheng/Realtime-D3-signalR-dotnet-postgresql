#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview-alpine as build
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV DOTNET_URLS=http://+:80
COPY . .
RUN dotnet restore
RUN dotnet publish -o /app/published-app

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview-alpine as runtime
WORKDIR /app
COPY --from=build /app/published-app /app
ENTRYPOINT ["dotnet", "/app/WASM_Reatime_D3.dll"]