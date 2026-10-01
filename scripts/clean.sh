#!/usr/bin/env bash
. "$(dirname "$0")/common.sh"

require_workspace_wide clean

step "dotnet clean"
dotnet clean "$SOLUTION"

# node_modules is deliberately left alone: deleting it costs a full re-download
# for no safety gain. dist/ and *.tsbuildinfo are regenerable caches.
step "frontend output"
rm -rf app/storefront/dist
find . -name '*.tsbuildinfo' -not -path './.git/*' -delete
