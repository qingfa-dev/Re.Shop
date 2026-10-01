#!/usr/bin/env bash
. "$(dirname "$0")/common.sh"

require_workspace_wide tools

step "dotnet tool restore"
dotnet tool restore
