#!/usr/bin/env bash
. "$(dirname "$0")/common.sh"

require_workspace_wide install

step "pnpm install"
pnpm install
