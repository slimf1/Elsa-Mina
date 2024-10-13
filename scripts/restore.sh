#!/bin/bash

source ./scripts/shared.sh
dotnet restore -r ${RUNTIME_ID}
