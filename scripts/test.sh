#!/bin/bash

source ./scripts/shared.sh
dotnet test ElsaMina.slnx --no-restore --verbosity normal --logger "trx;LogFileName=test-results.trx"
