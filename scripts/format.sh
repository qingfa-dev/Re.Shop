#!/usr/bin/env bash
. "$(dirname "$0")/common.sh"

case "$PKG" in
  all)
    step "dotnet format"
    dotnet format "$SOLUTION"
    step "storefront format"
    pnpm --filter "$STOREFRONT_PKG" run format
    ;;
  api|admin)
    step "dotnet format $PKG"
    dotnet format "$(project_path "$PKG")"
    ;;
  storefront)
    step "storefront format"
    pnpm --filter "$STOREFRONT_PKG" run format
    ;;
esac
