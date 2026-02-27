# ---- build frontend ----
FROM node:20-alpine AS web-build
WORKDIR /web

# copy only manifests first (better cache)
COPY BowlingApp/ClientApp/package*.json ./
RUN npm ci

# copy the rest and build
COPY BowlingApp/ClientApp/ ./
RUN npm run build


# ---- build backend ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS api-build
WORKDIR /src

COPY BowlingApp/*.csproj ./BowlingApp/
RUN dotnet restore ./BowlingApp/BowlingApp.csproj

COPY BowlingApp/ ./BowlingApp/
RUN dotnet publish ./BowlingApp/BowlingApp.csproj -c Release -o /app/publish


# ---- runtime (single container) ----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# ASP.NET app
COPY --from=api-build /app/publish ./

# static files from Vite build output (dist)
COPY --from=web-build /web/dist ./wwwroot

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "BowlingApp.dll"]