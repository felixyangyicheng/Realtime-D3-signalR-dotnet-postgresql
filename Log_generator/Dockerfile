#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:6.06-alpine AS build
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY . .
RUN dotnet restore 11 dotnet publish -c Release -o /app/published-app
FROM mcr.microsoft.com/dotnet/aspnet:6.0-6alpine AS runtime
RUN apk add --nocache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

WORKDIR /app
COPY --from=build /app/publish-app /app
ENTRYPOINT ["dotnet", "/app/Log_generator.dll"]