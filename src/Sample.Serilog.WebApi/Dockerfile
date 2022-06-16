#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Sample.Serilog.WebApi/Sample.Serilog.WebApi.csproj", "src/Sample.Serilog.WebApi/"]
COPY ["src/Sample.Serilog.WebApi.Core/Sample.Serilog.WebApi.Core.csproj", "src/Sample.Serilog.WebApi.Core/"]
RUN dotnet restore "src/Sample.Serilog.WebApi/Sample.Serilog.WebApi.csproj"
COPY . .
WORKDIR "/src/src/Sample.Serilog.WebApi"
RUN dotnet build "Sample.Serilog.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sample.Serilog.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sample.Serilog.WebApi.dll"]