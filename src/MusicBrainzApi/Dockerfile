#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["MusicBrainzApi/MusicBrainzApi.csproj", "MusicBrainzApi/"]
COPY ["Interfaces/Interfaces.csproj", "Interfaces/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Services/Services.csproj", "Services/"]
RUN dotnet restore "MusicBrainzApi/MusicBrainzApi.csproj"
COPY . .
WORKDIR "/src/MusicBrainzApi"
RUN dotnet build "MusicBrainzApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MusicBrainzApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MusicBrainzApi.dll"]