FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Sample.Serilog.WebApi/Sample.Serilog.WebApi.csproj", "Sample.Serilog.WebApi/"]
COPY ["Sample.Serilog.WebApi.Core/Sample.Serilog.WebApi.Core.csproj", "Sample.Serilog.WebApi.Core/"]

RUN dotnet restore "Sample.Serilog.WebApi/Sample.Serilog.WebApi.csproj"
COPY . .
WORKDIR "/src/Sample.Serilog.WebApi"
RUN dotnet build "Sample.Serilog.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sample.Serilog.WebApi.csproj" -c Release -o /app/publish

FROM base AS final

ENV TZ=America/Sao_Paulo
ENV LANG pt-BR
ENV LANGUAGE pt-BR
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

WORKDIR /app

COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:80

CMD ASPNETCORE_URLS=http://*:$PORT dotnet Sample.Serilog.WebApi.dll