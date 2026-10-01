#!/usr/bin/env bash
# Shared helpers for every scripts/ action.
#
# Source this first and pass no arguments — it inherits the calling script's
# $1 so PKG is resolved here, in one place:
#
#   . "$(dirname "$0")/common.sh"

set -euo pipefail

# /usr/share/dotnet is not on PATH in this environment. Prepend it only when
# dotnet is otherwise unresolvable, so this is a no-op on a normal setup.
if ! command -v dotnet >/dev/null 2>&1 && [ -x /usr/share/dotnet/dotnet ]; then
  export PATH="/usr/share/dotnet:$PATH"
fi

SCRIPTS_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPTS_DIR/.." && pwd)"
SOLUTION="Re.Shop.slnx"
STOREFRONT_PKG="re-shop-storefront"

# The package argument, defaulting to the whole repo.
PKG="${1:-all}"

cd "$REPO_ROOT"

# --- output -----------------------------------------------------------------

step() { printf '\n== %s\n' "$*"; }
note() { printf '   %s\n' "$*"; }
fail() { printf 'error: %s\n' "$*" >&2; exit 1; }

# Actions that only make sense repo-wide fail loudly rather than silently
# ignoring the package argument.
require_workspace_wide() {
  if [ "$PKG" != "all" ]; then
    fail "'$1' is workspace-wide — run 'make $1' without a package"
  fi
}

# --- package maps -----------------------------------------------------------

# Path to a runnable package's project file. Returns 1 for frontend/all.
project_path() {
  case "$1" in
    api)   echo "app/api/Re.Shop.Api.csproj" ;;
    admin) echo "app/admin/Re.Shop.Admin.csproj" ;;
    *)     return 1 ;;
  esac
}

# Path to the test project covering a package, where one exists.
project_test_path() {
  case "$1" in
    api) echo "tests/Re.Shop.Api.Tests/Re.Shop.Api.Tests.csproj" ;;
    *)   return 1 ;;
  esac
}
