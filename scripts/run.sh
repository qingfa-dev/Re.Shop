#!/usr/bin/env bash
. "$(dirname "$0")/common.sh"

case "$PKG" in
  all)
    step "Aspire: postgres + api + admin + storefront"
    exec dotnet run --project aspire/Re.AppHost
    ;;
  api|admin)
    step "run $PKG"
    exec dotnet run --project "$(project_path "$PKG")"
    ;;
  storefront)
    step "storefront dev server (vite)"
    exec pnpm --filter "$STOREFRONT_PKG" run dev
    ;;
esac
