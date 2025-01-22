Start-Process -NoNewWindow -FilePath "dotnet" -ArgumentList "watch run --project NTiersP4.API/NTiersP4.API.csproj"

Start-Sleep -Seconds 5

dotnet watch run --project NTiersP4.UI/NTiersP4.UI.csproj