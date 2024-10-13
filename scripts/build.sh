#!/bin/bash

source ./scripts/shared.sh
dotnet build ./src/ElsaMina.Console/ElsaMina.Console.csproj ${BUILD_PROPERTIES} -c ${CONFIGURATION} -r ${RUNTIME_ID} --no-restore
