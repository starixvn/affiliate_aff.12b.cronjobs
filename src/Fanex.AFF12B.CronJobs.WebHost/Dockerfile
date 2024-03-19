FROM hub.nexdev.net/base/dotnet/core/sdk:3.1 AS builder

WORKDIR "/src"

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2

COPY ["src/Fanex.AFF12B.CronJobs.Core/Fanex.AFF12B.CronJobs.Core.csproj", "./Fanex.AFF12B.CronJobs.Core/"]
RUN dotnet restore "Fanex.AFF12B.CronJobs.Core/Fanex.AFF12B.CronJobs.Core.csproj" -v n

COPY ["src/Fanex.AFF12B.CronJobs.WebHost/Fanex.AFF12B.CronJobs.WebHost.csproj", "./Fanex.AFF12B.CronJobs.WebHost/"]
RUN dotnet restore "Fanex.AFF12B.CronJobs.WebHost/Fanex.AFF12B.CronJobs.WebHost.csproj" -v n

COPY "/src" .

WORKDIR "/src/Fanex.AFF12B.CronJobs.WebHost"
RUN dotnet build "Fanex.AFF12B.CronJobs.WebHost.csproj" -c Release -o /app/build

RUN dotnet publish "Fanex.AFF12B.CronJobs.WebHost.csproj" -c Release -o /app/publish

FROM hub.nexdev.net/base/dotnet/core/aspnet:3.1.32-alpine

RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /usr/lib/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /usr/lib/ssl/openssl.cnf

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

# Don’t run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser
#

WORKDIR "/app"

COPY --from=builder /app/publish .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.AFF12B.CronJobs.WebHost.dll"]