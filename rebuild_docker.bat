rmdir /Q /S C:\_MyProjects\highloadcup_aspnet_core\AccountsApi\out 

dotnet publish -c Release -o out .\AccountsApi\AccountsApi.csproj

docker build -t aspnetcore .