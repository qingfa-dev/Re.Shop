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
    while IFS= read -r project; do
      dotnet test "$project"
    done < <(project_test_paths api)
    ;;
  admin)
    fail "no test project exists for 'admin'"
    ;;
  storefront)
    step "storefront tests"
    pnpm --filter "$STOREFRONT_PKG" run test:unit
    ;;
esac
