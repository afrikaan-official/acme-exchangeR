ARG PROJECT_NAME=default

FROM mcr.microsoft.com/dotnet/sdk AS build
WORKDIR /app
ARG PROJECT_NAME
COPY . ./
RUN dotnet restore ./API/$PROJECT_NAME/$PROJECT_NAME.csproj
RUN dotnet publish ./API/$PROJECT_NAME/$PROJECT_NAME.csproj -c Development -o out

FROM mcr.microsoft.com/dotnet/aspnet
ARG PROJECT_NAME
ENV PROJECT_NAME=${PROJECT_NAME}
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT dotnet ${PROJECT_NAME}.dll