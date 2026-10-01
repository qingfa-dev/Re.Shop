#!/usr/bin/env bash
. "$(dirname "$0")/common.sh"

require_workspace_wide outdated

step "NuGet outdated"
dotnet list "$SOLUTION" package --outdated

# pnpm exits 1 when drift exists — that is the report, not a failure.
step "npm outdated"
pnpm outdated -r || true
