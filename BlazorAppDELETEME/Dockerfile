FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BlazorAppDELETEME/BlazorAppDELETEME.csproj", "BlazorAppDELETEME/"]
RUN dotnet restore "BlazorAppDELETEME/BlazorAppDELETEME.csproj"
COPY . .
WORKDIR "/src/BlazorAppDELETEME"
RUN dotnet build "BlazorAppDELETEME.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlazorAppDELETEME.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorAppDELETEME.dll"]
