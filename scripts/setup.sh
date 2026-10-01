#!/usr/bin/env bash
. "$(dirname "$0")/common.sh"

require_workspace_wide setup

step "install"
"$SCRIPTS_DIR/install.sh"

step "tools"
"$SCRIPTS_DIR/tools.sh"

step "dotnet restore"
dotnet restore --force-evaluate
