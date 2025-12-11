FROM mcr.microsoft.com/dotnet/sdk:10.0@sha256:d1823fecac3689a2eb959e02ee3bfe1c2142392808240039097ad70644566190 AS build
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0@sha256:eaa79205c3ade4792a7f7bf310a3aac51fe0e1d91c44e40f70b7c6423d475fe0
WORKDIR /App
COPY --from=build /App/out .

# Create Data directory for SQLite database with proper permissions
RUN mkdir -p /App/Data && chmod 777 /App/Data

ENTRYPOINT ["dotnet", "Corkboard.dll"]