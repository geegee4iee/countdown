FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 50542
EXPOSE 44349

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Coundown.Spa.ReactJs/Coundown.Spa.ReactJs.csproj Coundown.Spa.ReactJs/
# set up node
ENV NODE_VERSION 8.9.4
ENV NODE_DOWNLOAD_SHA 21fb4690e349f82d708ae766def01d7fec1b085ce1f5ab30d9bda8ee126ca8fc
RUN curl -SL "https://nodejs.org/dist/v${NODE_VERSION}/node-v${NODE_VERSION}-linux-x64.tar.gz" --output nodejs.tar.gz \
    && echo "$NODE_DOWNLOAD_SHA nodejs.tar.gz" | sha256sum -c - \
    && tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \
    && rm nodejs.tar.gz \
    && ln -s /usr/local/bin/node /usr/local/bin/nodejs
RUN dotnet restore Coundown.Spa.ReactJs/Coundown.Spa.ReactJs.csproj -nowarn:nu1605
COPY . .
WORKDIR /src/Coundown.Spa.ReactJs
RUN dotnet build Coundown.Spa.ReactJs.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Coundown.Spa.ReactJs.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Coundown.Spa.ReactJs.dll"]
