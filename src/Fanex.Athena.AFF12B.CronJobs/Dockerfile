#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Fanex.Athena.12B.CronJobs.WebHost/Fanex.Athena.12B.CronJobs.WebHost.csproj", "Fanex.Athena.12B.CronJobs.WebHost/"]
RUN dotnet restore "Fanex.Athena.12B.CronJobs.WebHost/Fanex.Athena.12B.CronJobs.WebHost.csproj"
COPY . .
WORKDIR "/src/Fanex.Athena.12B.CronJobs.WebHost"
RUN dotnet build "Fanex.Athena.12B.CronJobs.WebHost.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Fanex.Athena.12B.CronJobs.WebHost.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fanex.Athena.12B.CronJobs.WebHost.dll"]