#!/usr/bin/env bash
echo 'Starting container..'
docker-entrypoint.sh postgres > /dev/null 2>&1 &
sleep 20s
dotnet AccountsApi.dll
