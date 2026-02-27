#!/usr/bin/env bash
set -eu

cd ./ElsaV2/output

echo "Writing config file"
printf '%s' "$CONFIG_FILE" > config.json
printf '%s' "$SERVICE_ACCOUNT_CREDENTIALS" > service_account.client_secrets.json

echo "Checking existing instances of ElsaMina.Console"
if pids="$(pgrep -x ElsaMina.Console)"; then
  echo "Killing existing instances of ElsaMina.Console: $pids"
  kill $pids

  i=0
  while [ "$i" -lt 20 ]; do
    if ! pgrep -x ElsaMina.Console > /dev/null; then
      break
    fi
    sleep 0.5
    i=$((i + 1))
  done

  if pgrep -x ElsaMina.Console > /dev/null; then
    echo "Force killing remaining instances of ElsaMina.Console"
    pkill -KILL -x ElsaMina.Console
  fi
fi

echo "Setting permissions"
chmod u+x ElsaMina.Console

echo "Starting ElsaMina.Console"
nohup ./ElsaMina.Console >> elsa.log 2>&1 < /dev/null &

sleep 5
if pgrep -f ElsaMina.Console > /dev/null; then
  echo "ElsaMina.Console started successfully"
else
  echo "Failed to start ElsaMina.Console"
  exit 1
fi
