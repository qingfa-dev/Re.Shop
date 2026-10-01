#!/usr/bin/env bash
. "$(dirname "$0")/common.sh"

case "$PKG" in
  all)
    step "dotnet format (verify)"
    dotnet format "$SOLUTION" --verify-no-changes
    step "frontend lint"
    pnpm -r run lint
    ;;
  api|admin)
    step "dotnet format (verify) $PKG"
    dotnet format "$(project_path "$PKG")" --verify-no-changes
    ;;
  storefront)
    step "storefront lint"
    pnpm --filter "$STOREFRONT_PKG" run lint
    ;;
esac
