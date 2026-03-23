#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

APP_URL="${APP_URL:-http://localhost:5077}"

if ! command -v dotnet >/dev/null 2>&1; then
  echo "dotnet CLI is required. Install .NET 10 SDK and rerun."
  exit 1
fi

dotnet --info

dotnet restore EBSCore.sln

dotnet build EBSCore.sln -c Debug

# Launch app in background
DOTNET_ENVIRONMENT=Development dotnet run --project EBSCore.Web --urls "$APP_URL" > /tmp/ebscore-run.log 2>&1 &
APP_PID=$!
trap 'kill $APP_PID >/dev/null 2>&1 || true' EXIT

# Wait for app
for _ in {1..60}; do
  if curl -fsS "$APP_URL/login" >/dev/null 2>&1; then
    break
  fi
  sleep 1
done

# Basic endpoint checks
curl -fsS "$APP_URL/login" >/dev/null
curl -fsS "$APP_URL/api/CurrentUser/Login" >/dev/null

# Login flow check
LOGIN_RESPONSE=$(curl -fsS -X POST "$APP_URL/api/CurrentUser/Login" -H 'Content-Type: application/json' -d '{"userName":"admin","password":"admin123","keepMeSignedIn":false}')
if [[ -z "$LOGIN_RESPONSE" ]]; then
  echo "Login API returned empty response"
  exit 1
fi

echo "Smoke checks completed successfully."
