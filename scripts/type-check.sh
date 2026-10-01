#!/usr/bin/env bash
. "$(dirname "$0")/common.sh"

case "$PKG" in
  all)
    step "type-check"
    pnpm -r run type-check
    ;;
  storefront)
    step "storefront type-check"
    pnpm --filter "$STOREFRONT_PKG" run type-check
    ;;
  api|admin)
    fail "'$PKG' is not a TypeScript package"
    ;;
esac
