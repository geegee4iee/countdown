FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 58999
EXPOSE 44377

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY WebServices/KarmaPredictorService/KarmaPredictorService.csproj WebServices/KarmaPredictorService/
RUN dotnet restore WebServices/KarmaPredictorService/KarmaPredictorService.csproj
COPY . .
WORKDIR /src/WebServices/KarmaPredictorService
RUN dotnet build KarmaPredictorService.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish KarmaPredictorService.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "KarmaPredictorService.dll"]
