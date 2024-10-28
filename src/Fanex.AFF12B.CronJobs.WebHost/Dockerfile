ARG CI_REGISTRY_IMAGE

FROM $CI_REGISTRY_IMAGE:builder AS deps

RUN mkdir -p /src/packages

FROM hub.nexdev.net/base/dotnet/core/sdk:3.1 AS builder

ENV NUGET_PACKAGES=/src/packages

COPY --from=deps $NUGET_PACKAGES/. $NUGET_PACKAGES/

WORKDIR "/src"

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2

COPY ["src/Fanex.AFF12B.CronJobs.Core/Fanex.AFF12B.CronJobs.Core.csproj", "./Fanex.AFF12B.CronJobs.Core/"]
RUN dotnet restore -v n -r linux-x64 "./Fanex.AFF12B.CronJobs.Core/Fanex.AFF12B.CronJobs.Core.csproj"

COPY ["src/Fanex.AFF12B.CronJobs.GeneralJobs.Core/Fanex.AFF12B.CronJobs.GeneralJobs.Core.csproj", "./Fanex.AFF12B.CronJobs.GeneralJobs.Core/"]
RUN dotnet restore -v n -r linux-x64 "./Fanex.AFF12B.CronJobs.GeneralJobs.Core/Fanex.AFF12B.CronJobs.GeneralJobs.Core.csproj"

COPY ["src/Fanex.AFF12B.CronJobs.VendorJobs.Core/Fanex.AFF12B.CronJobs.VendorJobs.Core.csproj", "./Fanex.AFF12B.CronJobs.VendorJobs.Core/"]
RUN dotnet restore -v n -r linux-x64 "./Fanex.AFF12B.CronJobs.VendorJobs.Core/Fanex.AFF12B.CronJobs.VendorJobs.Core.csproj"

COPY ["src/Fanex.AFF12B.CronJobs.WebHost/Fanex.AFF12B.CronJobs.WebHost.csproj", "./Fanex.AFF12B.CronJobs.WebHost/"]
RUN dotnet restore -v n -r linux-x64 "./Fanex.AFF12B.CronJobs.WebHost/Fanex.AFF12B.CronJobs.WebHost.csproj"

COPY "/src" .

WORKDIR "/src/Fanex.AFF12B.CronJobs.WebHost"

RUN dotnet publish -r linux-x64 --self-contained false -o /app/publish

FROM hub.nexdev.net/base/dotnet/core/aspnet:3.1.32-alpine

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