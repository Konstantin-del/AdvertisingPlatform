
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY ["AdvertisingPlatform.sln", "."]
COPY ["AdvertisingPlatform.Presentation/AdvertisingPlatform.Presentation.csproj", "AdvertisingPlatform.Presentation/"]
COPY ["AdvertisingPlatform.Application/AdvertisingPlatform.Application.csproj", "AdvertisingPlatform.Application/"]

RUN dotnet restore "AdvertisingPlatform.Presentation/AdvertisingPlatform.Presentation.csproj"

COPY . .

WORKDIR "/src/AdvertisingPlatform.Presentation"
RUN dotnet build "AdvertisingPlatform.Presentation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AdvertisingPlatform.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "AdvertisingPlatform.Presentation.dll"]


