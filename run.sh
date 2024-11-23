#!/bin/bash

dotnet watch run --project src/server/Api/Api.csproj &

sleep 5

dotnet watch run --project src/client/BlazorApp/BlazorApp.csproj