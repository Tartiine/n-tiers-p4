#!/bin/bash

dotnet watch run --project NTiersP4.API/NTiersP4.API.csproj &

sleep 5

dotnet watch run --project NTiersP4.UI/NTiersP4.UI.csproj