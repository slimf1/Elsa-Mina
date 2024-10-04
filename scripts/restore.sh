#!/usr/bin/env bash

source ./scripts/shared.sh
dotnet restore -r "${RUNTIME_ID}" 
