FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY /AccountsApi/*.csproj ./
RUN dotnet restore


# Copy everything else and build
COPY ./AccountsApi/ ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out .

# Copy test data
COPY tmp/ /tmp/
RUN ls -la /tmp/*

ENTRYPOINT ["dotnet", "AccountsApi.dll"]