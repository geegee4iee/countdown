FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 58998
EXPOSE 44376
RUN if [ ! -f nodejs.tar.gz ]; then curl -SL "https://nodejs.org/dist/v8.11.3/node-v8.11.3-linux-x64.tar.gz" --output nodejs.tar.gz; fi
RUN tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \
    && ln -s /usr/local/bin/node /usr/local/bin/nodejs

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Coundown.Spa.ReactJs/Coundown.Spa.ReactJs.csproj Coundown.Spa.ReactJs/
COPY Countdown.ML.Core/Countdown.ML.Core.csproj Countdown.ML.Core/
COPY Countdown.Core/Countdown.Core.csproj Countdown.Core/
RUN dotnet restore Coundown.Spa.ReactJs/Coundown.Spa.ReactJs.csproj
COPY . .
WORKDIR /src/Coundown.Spa.ReactJs
RUN dotnet build Coundown.Spa.ReactJs.csproj -c Release -o /app
ENV NODE_VERSION 8.9.4
ENV NODE_DOWNLOAD_SHA 21fb4690e349f82d708ae766def01d7fec1b085ce1f5ab30d9bda8ee126ca8fc
RUN if [ ! -f nodejs.tar.gz ]; then curl -SL "https://nodejs.org/dist/v8.11.3/node-v8.11.3-linux-x64.tar.gz" --output nodejs.tar.gz; fi
RUN tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \
    && ln -s /usr/local/bin/node /usr/local/bin/nodejs

FROM build AS publish
RUN dotnet publish Coundown.Spa.ReactJs.csproj -c Release -o /app

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS final
WORKDIR /app
EXPOSE 58998
EXPOSE 44376
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Coundown.Spa.ReactJs.dll"]
