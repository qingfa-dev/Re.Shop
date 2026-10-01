#!/usr/bin/env bash
. "$(dirname "$0")/common.sh"

case "$PKG" in
  all)
    step "dotnet test"
    dotnet test "$SOLUTION"
    step "storefront tests"
    pnpm --filter "$STOREFRONT_PKG" run test:unit
    ;;
  api)
    step "api tests"
    dotnet test "$(project_test_path api)"
    ;;
  admin)
    fail "no test project exists for 'admin'"
    ;;
  storefront)
    step "storefront tests"
    pnpm --filter "$STOREFRONT_PKG" run test:unit
    ;;
esac
