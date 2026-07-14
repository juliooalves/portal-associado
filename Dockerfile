FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source
COPY Directory.Build.props ./
COPY src/ProAuto.PortalAssociado.Web/ProAuto.PortalAssociado.Web.csproj src/ProAuto.PortalAssociado.Web/
RUN dotnet restore src/ProAuto.PortalAssociado.Web/ProAuto.PortalAssociado.Web.csproj
COPY src/ src/
RUN dotnet publish src/ProAuto.PortalAssociado.Web/ProAuto.PortalAssociado.Web.csproj -c Release -o /app --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
RUN mkdir /keys && chown app /keys
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
USER app
ENTRYPOINT ["dotnet", "ProAuto.PortalAssociado.Web.dll"]
