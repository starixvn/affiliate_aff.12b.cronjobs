FROM hub.nexdev.net/base/dotnet/core/sdk:6.0-alpine AS builder

WORKDIR "/src"

RUN dotnet nuget add source http://nuget.nexdev.net/api/v2

COPY ["src/Fanex.12B.CronJobs.Core/Fanex.12B.CronJobs.Core.csproj", "./Fanex.12B.CronJobs.Core/"]
RUN dotnet restore "Fanex.12B.CronJobs.Core/Fanex.12B.CronJobs.Core.csproj" -v n

COPY ["src/Fanex.12B.CronJobs.Ext.Jobs.Core/Fanex.12B.CronJobs.Ext.Jobs.Core.csproj", "./Fanex.12B.CronJobs.Ext.Jobs.Core/"]
RUN dotnet restore "Fanex.12B.CronJobs.Ext.Jobs.Core/Fanex.12B.CronJobs.Ext.Jobs.Core.csproj" -v n

COPY ["src/Fanex.12B.CronJobs.Ext.Jobs.WebHost/Fanex.12B.CronJobs.Ext.Jobs.WebHost.csproj", "./Fanex.12B.CronJobs.Ext.Jobs.WebHost/"]
RUN dotnet restore "Fanex.12B.CronJobs.Ext.Jobs.WebHost/Fanex.12B.CronJobs.Ext.Jobs.WebHost.csproj" -v n

COPY "/src" .

WORKDIR "/src/Fanex.12B.CronJobs.Ext.Jobs.WebHost"
RUN dotnet build "Fanex.12B.CronJobs.Ext.Jobs.WebHost.csproj" -c Release -o /app/build

RUN dotnet publish "Fanex.12B.CronJobs.Ext.Jobs.WebHost.csproj" -c Release -o /app/publish

FROM hub.nexdev.net/base/dotnet/core/aspnet:6.0-final

RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /usr/lib/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /usr/lib/ssl/openssl.cnf

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

WORKDIR "/app"

COPY --from=builder /app/publish .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.12B.CronJobs.Ext.Jobs.WebHost.dll"]