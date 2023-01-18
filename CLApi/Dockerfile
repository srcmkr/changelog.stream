FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["CLApi/CLApi.csproj", "CLApi/"]
RUN dotnet restore "CLApi/CLApi.csproj"
COPY . .
WORKDIR "/src/CLApi"
RUN dotnet build "CLApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CLApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CLApi.dll"]
