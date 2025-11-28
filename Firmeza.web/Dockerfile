FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Firmeza.sln", "./"]
COPY ["Firmeza.API/Firmeza.API.csproj", "Firmeza.API/"]
COPY ["Firmeza.Application/Firmeza.Application.csproj", "Firmeza.Application/"]
COPY ["Firmeza.Domain/Firmeza.Domain.csproj", "Firmeza.Domain/"]
COPY ["Firmeza.Infrastructure/Firmeza.Infrastructure.csproj", "Firmeza.Infrastructure/"]
COPY ["Firmeza.Identity/Firmeza.Identity.csproj", "Firmeza.Identity/"]
COPY ["Firmeza.web/Firmeza.web.csproj", "Firmeza.web/"]
COPY ["Firmeza.Tests/Firmeza.Tests.csproj", "Firmeza.Tests/"]

RUN dotnet restore "Firmeza.web/Firmeza.web.csproj"
COPY . .
WORKDIR "/src/Firmeza.web"
RUN dotnet build "Firmeza.web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Firmeza.web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Firmeza.web.dll"]
