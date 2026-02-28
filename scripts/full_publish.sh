#!/usr/bin/env bash
set -euo pipefail

echo "=== Restore ==="
./scripts/restore.sh

echo "=== Build ==="
./scripts/build.sh

echo "=== Test ==="
./scripts/test.sh

echo "=== Publish ==="
./scripts/publish.sh

echo "=== Done ==="
