FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BlazorAuthSample/BlazorAuthSample.csproj", "BlazorAuthSample/"]
RUN dotnet restore "BlazorAuthSample/BlazorAuthSample.csproj"
COPY . .
WORKDIR "/src/BlazorAuthSample"
RUN dotnet build "BlazorAuthSample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlazorAuthSample.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorAuthSample.dll"]
