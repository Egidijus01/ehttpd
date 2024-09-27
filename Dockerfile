FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

WORKDIR /App

COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -r linux-x64 --self-contained -o out
COPY bin/Release/net8.0/linux-x64/csh_server.dll ./
COPY bin/Release/net8.0/linux-x64/csh_server ./

RUN apk add --no-cache \
    libstdc++ \
    libgcc \
    libcurl

RUN chmod +x csh_server

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime

WORKDIR /App

COPY --from=build-env /App/out .

EXPOSE 80

CMD ["dotnet", "csh_server.dll"]  # Change this if necessary
