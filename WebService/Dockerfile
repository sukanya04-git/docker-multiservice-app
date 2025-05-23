# Use official .NET 8 ASP.NET runtime image as the base for the final app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
ENV DOTNET_URLS=http://+:80

# Use official .NET 8 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore 
COPY . .
RUN dotnet publish -c Release -o /app/publish 

FROM base AS final
WORKDIR /app

# Create non-root user and switch to it
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
USER appuser

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebService.dll"]
