FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 59253
EXPOSE 44367

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY WebServices/IdentityServer/IdentityServer.csproj WebServices/IdentityServer/
RUN dotnet restore WebServices/IdentityServer/IdentityServer.csproj
COPY . .
WORKDIR /src/WebServices/IdentityServer
RUN dotnet build IdentityServer.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish IdentityServer.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "IdentityServer.dll"]
