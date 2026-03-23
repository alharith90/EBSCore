#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

if ! command -v dotnet >/dev/null 2>&1; then
  echo "dotnet CLI is required. Install .NET 10 SDK and rerun."
  exit 1
fi

dotnet --info

dotnet restore EBSCore.sln

dotnet build EBSCore.sln -c Debug

echo "Starting app on http://localhost:5077"
dotnet run --project EBSCore.Web --urls "http://localhost:5077"
