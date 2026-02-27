#!/usr/bin/env bash
set -eu

cd ./ElsaV2/output

echo "Writing config file"
printf '%s' "$CONFIG_FILE" > config.json
printf '%s' "$SERVICE_ACCOUNT_CREDENTIALS" > service_account.client_secrets.json

echo "Current shell: PID=$$, PGID=$(ps -o pgid= -p $$ | tr -d ' ')"

echo "Checking existing instances of ElsaMina.Console"
if pids="$(pgrep -f ElsaMina.Console)"; then
  echo "Found PIDs: $pids"
  echo "Process details:"
  ps -p $pids -o pid,ppid,pgid,stat,args || true

  echo "Killing existing instances of ElsaMina.Console: $pids"
  # Ignore TERM/HUP in this shell in case the old process shares our process group
  trap '' TERM HUP
  kill $pids || true
  trap - TERM HUP
  echo "Kill signal sent"

  i=0
  while [ "$i" -lt 20 ]; do
    if ! pgrep -f ElsaMina.Console > /dev/null; then
      echo "All instances stopped after $((i)) * 0.5s"
      break
    fi
    sleep 0.5
    i=$((i + 1))
  done

  if pgrep -f ElsaMina.Console > /dev/null; then
    echo "Force killing remaining instances of ElsaMina.Console"
    pkill -KILL -f ElsaMina.Console || true
  fi
else
  echo "No existing instances found"
fi

echo "Setting permissions"
chmod u+x ElsaMina.Console

echo "Starting ElsaMina.Console"
setsid nohup ./ElsaMina.Console >> elsa.log 2>&1 < /dev/null &
NEW_PID=$!
echo "Launched with PID $NEW_PID"

sleep 5
echo "Checking if process is still running after 5s..."
if pgrep -f ElsaMina.Console > /dev/null; then
  echo "ElsaMina.Console started successfully"
  pgrep -f ElsaMina.Console | xargs ps -o pid,ppid,pgid,stat,args -p || true
else
  echo "Failed to start ElsaMina.Console"
  echo "Last 20 lines of elsa.log:"
  tail -20 elsa.log || true
  exit 1
fi