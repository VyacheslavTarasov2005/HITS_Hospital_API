FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["HITS_API_1.Presentation/HITS_API_1.Presentation.csproj", "HITS_API_1.Presentation/"]
COPY ["HITS_API_1.Application/HITS_API_1.Application.csproj", "HITS_API_1.Application/"]
COPY ["HITS_API_1.Domain/HITS_API_1.Domain.csproj", "HITS_API_1.Domain/"]
COPY ["HITS_API_1.Infrastructure/HITS_API_1.Infrastructure.csproj", "HITS_API_1.Infrastructure/"]
RUN dotnet restore "HITS_API_1.Presentation/HITS_API_1.Presentation.csproj"
COPY . .
WORKDIR "/src/HITS_API_1.Presentation"
RUN dotnet build "HITS_API_1.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "HITS_API_1.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HITS_API_1.Presentation.dll"]
