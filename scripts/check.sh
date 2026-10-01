#!/usr/bin/env bash
. "$(dirname "$0")/common.sh"

# check is cross-cutting: a partial check is not a check.
if [ "$PKG" != "all" ]; then
  note "'check' runs against every package, ignoring '$PKG'"
fi

"$SCRIPTS_DIR/build.sh" all
"$SCRIPTS_DIR/test.sh" all
"$SCRIPTS_DIR/lint.sh" all
"$SCRIPTS_DIR/type-check.sh" all
"$SCRIPTS_DIR/readme-check.sh" all

step "all checks passed"
