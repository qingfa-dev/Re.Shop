#!/usr/bin/env bash
. "$(dirname "$0")/common.sh"

case "$PKG" in
  all)
    step "dotnet build"
    dotnet build "$SOLUTION"
    step "frontend build"
    pnpm -r run build
    ;;
  api|admin)
    step "dotnet build $PKG"
    dotnet build "$(project_path "$PKG")"
    ;;
  storefront)
    step "storefront build"
    pnpm --filter "$STOREFRONT_PKG" run build
    ;;
esac
