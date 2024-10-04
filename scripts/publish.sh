#!/usr/bin/env bash

source shared.sh
dotnet publish ./src/ElsaMina.Console/ElsaMina.Console.csproj ${BUILD_PROPERTIES} -c ${CONFIGURATION} -r ${RUNTIME_ID} --no-restore --no-build -o ./output
