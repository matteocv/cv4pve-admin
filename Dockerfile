# SPDX-FileCopyrightText: Copyright Corsinvest Srl
# SPDX-License-Identifier: AGPL-3.0-only

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
VOLUME ["/app/data"]

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/", "src/"]
RUN dotnet restore "src/Corsinvest.ProxmoxVE.Admin/Corsinvest.ProxmoxVE.Admin.csproj"
#WORKDIR "/src/Corsinvest.ProxmoxVE.Admin/."
RUN dotnet build "src/Corsinvest.ProxmoxVE.Admin/Corsinvest.ProxmoxVE.Admin.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/Corsinvest.ProxmoxVE.Admin/Corsinvest.ProxmoxVE.Admin.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV INDOCKER=1
ENV ASPNETCORE_URLS=http://+:5000

#fix file
RUN rm /app/appsettings.json

ENTRYPOINT ["dotnet", "Corsinvest.ProxmoxVE.Admin.dll"]