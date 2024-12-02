# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS with-node
RUN apt-get update
RUN apt-get install curl
RUN curl -sL https://deb.nodesource.com/setup_20.x | bash
RUN apt-get -y install nodejs
RUN npm install -g @angular/cli

FROM with-node AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["sdt.client/nuget.config", "sdt.client/"]
COPY ["sdt.client/sdt.client.esproj", "sdt.client/"]
COPY ["SDT.Server/SDT.LApi.csproj", "SDT.Server/"]
COPY ["SDT.LBl/SDT.LBl.csproj", "SDT.LBl/"]
COPY ["SDT.Data/SDT.LData.csproj", "SDT.Data/"]
COPY ["SDT.LHelpers/SDT.LHelpers.csproj", "SDT.LHelpers/"]
COPY ["SDT.LRepositories/SDT.LRepositories.csproj", "SDT.LRepositories/"]

RUN dotnet restore "./SDT.Server/SDT.LApi.csproj"
COPY . .
WORKDIR "/src/SDT.Server"
RUN dotnet build "./SDT.LApi.csproj" -c $BUILD_CONFIGURATION -o /app/build


# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SDT.LApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false



# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_LOGGING__CONSOLE__DISABLECOLORS=false

ENV MY_DOMAIN=levnikolaevich.ai

RUN BUILD_DATE=$(date +%Y%m%d:%H%M) && \
    echo "Build date is ${BUILD_DATE}" && \
    export APP_VERSION=${BUILD_DATE} && \
    echo "APP_VERSION=${BUILD_DATE}" > /tmp/buildinfo

ENTRYPOINT ["dotnet", "SDT.LApi.dll"]
