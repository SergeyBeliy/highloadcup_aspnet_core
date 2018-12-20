FROM microsoft/dotnet:sdk

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY . .

RUN dotnet build .\\AccountsApi\\AccountsApi.csproj

EXPOSE 80

CMD  ["dotnet", "AccountsApi/bin/Debug/netcoreapp2.2/AccountsApi.dll"] 
