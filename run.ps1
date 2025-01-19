Start-Process -NoNewWindow -FilePath "dotnet" -ArgumentList "watch run --project src/server/Api/Api.csproj"

Start-Sleep -Seconds 5

dotnet watch run --project src/client/BlazorApp/BlazorApp.csproj --verbosity detailed