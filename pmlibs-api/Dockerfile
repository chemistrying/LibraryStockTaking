# https://hub.docker.com/_/microsoft-dotnet
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG TARGETARCH
WORKDIR /aspnetapp

COPY *.csproj .
RUN dotnet restore -a $TARGETARCH

# copy everything else and build app
COPY . .
RUN dotnet publish -a $TARGETARCH -c release -o /app --no-restore
COPY ./files /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
EXPOSE 5052
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["./pmlibs"]