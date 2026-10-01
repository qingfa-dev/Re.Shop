#!/usr/bin/env bash
# Verify every folder README.md against guide/folder-readme-guide.md.
#
# Enforces:
#   §5   fixed section set and order, required sections present
#   §6.1 title line plus a one-line purpose under it
#   §6.3 Contains is a Path/Role/Notes table; child READMEs linked,
#        undocumented children marked '_(no README)_'
#   §6.4 Boundaries states both 'In scope' and 'Out of scope'
#   §6.11 Related names the parent and links child READMEs
#   §8   60–120 line budget, no dead relative links
#
# The repository root README is intentionally excluded.
#
# Run: make readme-check

set -u

. "$(dirname "$0")/common.sh"

require_workspace_wide readme-check

GUIDE="guide/folder-readme-guide.md"

[ -f "$GUIDE" ] || fail \
    "missing $GUIDE — there is nothing to validate against"

step "folder README check"

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------

MIN_LINES=60
MAX_LINES=120

SKIP_DIRS=(
    node_modules
    dist
    build
    bin
    obj
    .git
    coverage
    vendor
)

ORDER=(
    "Summary"
    "Contains"
    "Boundaries"
    "Assumptions"
    "Invariants"
    "Contracts"
    "Conventions"
    "Flows"
    "Reading Order"
    "Related"
    "Ownership"
    "Notes"
)

REQUIRED=(
    "Summary"
    "Contains"
    "Boundaries"
    "Assumptions"
    "Related"
)

errors=0
warnings=0
checked=0

# ---------------------------------------------------------------------------
# Output helpers
# ---------------------------------------------------------------------------

error() {
    printf 'error: %s: %s\n' "$1" "$2" >&2
    errors=$((errors + 1))
}

warn() {
    printf 'warning: %s: %s\n' "$1" "$2" >&2
    warnings=$((warnings + 1))
}

# ---------------------------------------------------------------------------
# Path helpers
# ---------------------------------------------------------------------------

should_skip_dir() {
    local dir="$1"
    local skip

    for skip in "${SKIP_DIRS[@]}"; do
        [[ "$dir" == "$skip" ]] && return 0
    done

    return 1
}

# ---------------------------------------------------------------------------
# Section helpers
# ---------------------------------------------------------------------------

get_section_body() {
    local file="$1"
    local section="$2"

    awk -v section="$section" '
        $0 == "## " section {
            found=1
            next
        }

        found && /^## / {
            exit
        }

        found {
            print
        }
    ' "$file"
}

get_headings() {
    grep -E '^## ' "$1" | sed 's/^## //'
}

# ---------------------------------------------------------------------------
# §6.1 — title and purpose
# ---------------------------------------------------------------------------

check_title() {
    local path="$1"
    local line1 line2 line3

    line1=$(sed -n '1p' "$path")
    line2=$(sed -n '2p' "$path")
    line3=$(sed -n '3p' "$path")

    if [[ ! "$line1" =~ ^#\  ]]; then
        error "$path" "line 1 must be '# <folder name>'"
        return
    fi

    if [[ -z "${line3//[[:space:]]/}" ]]; then
        error "$path" "missing the one-line purpose under the title"
        return
    fi

    if [[ ! "$line3" =~ _[a-z]+\ ·\ [a-z]+_ ]]; then
        warn "$path" "purpose line has no 'kind · role' subtitle"
    fi
}

# ---------------------------------------------------------------------------
# §5 — fixed sections, order, required sections
# ---------------------------------------------------------------------------

check_sections() {
    local path="$1"

    mapfile -t headings < <(get_headings "$path")

    # Check for unknown headings.
    local heading known
    for heading in "${headings[@]}"; do
        known=false

        for expected in "${ORDER[@]}"; do
            if [[ "$heading" == "$expected" ]]; then
                known=true
                break
            fi
        done

        if [[ "$known" == false ]]; then
            error "$path" "ad-hoc section '$heading' — use the fixed set"
        fi
    done

    # Check ordering.
    local previous_rank=-1
    local rank
    local i

    for heading in "${headings[@]}"; do
        rank=-1

        for i in "${!ORDER[@]}"; do
            if [[ "${ORDER[$i]}" == "$heading" ]]; then
                rank="$i"
                break
            fi
        done

        [[ "$rank" -lt 0 ]] && continue

        if [[ "$rank" -lt "$previous_rank" ]]; then
            error "$path" "sections out of order"
            break
        fi

        previous_rank="$rank"
    done

    # Required sections.
    local required found

    for required in "${REQUIRED[@]}"; do
        found=false

        for heading in "${headings[@]}"; do
            if [[ "$heading" == "$required" ]]; then
                found=true
                break
            fi
        done

        if [[ "$found" == false ]]; then
            error "$path" "missing required section '## $required'"
        fi
    done

    # Line budget.
    local count
    count=$(wc -l < "$path")

    if (( count > MAX_LINES )); then
        error "$path" \
            "$count lines — over the $MAX_LINES-line budget"
    elif (( count < MIN_LINES )); then
        warn "$path" \
            "$count lines — under the $MIN_LINES-line target"
    fi
}

# ---------------------------------------------------------------------------
# §8 — relative links must resolve
# ---------------------------------------------------------------------------

check_links() {
    local path="$1"
    local directory link target resolved

    directory=$(dirname "$path")

    # Extract Markdown links:
    #   [text](target)
    #
    # Ignore external URLs and mailto links.
    while IFS= read -r link; do
        [[ -z "$link" ]] && continue

        case "$link" in
            http://*|https://*|mailto:*)
                continue
                ;;
        esac

        target="${link%%#*}"

        [[ -z "$target" ]] && continue

        resolved="$directory/$target"

        if [[ ! -e "$resolved" ]]; then
            error "$path" "dead relative link '$link'"
        fi
    done < <(
        grep -oE '\]\([^)]*\)' "$path" |
            sed -E 's/^\]\((.*)\)$/\1/'
    )
}

# ---------------------------------------------------------------------------
# §6.3 — Contains
# ---------------------------------------------------------------------------

check_contains() {
    local path="$1"
    local body
    local rows
    local first_row
    local row
    local path_cell
    local notes
    local entry
    local folder
    local has_readme
    local tick='`'

    body=$(get_section_body "$path" "Contains")

    if [[ -z "${body//[[:space:]]/}" ]]; then
        error "$path" "## Contains has no table"
        return
    fi

    mapfile -t rows < <(
        printf '%s\n' "$body" | grep '^|' || true
    )

    if (( ${#rows[@]} < 2 )); then
        error "$path" \
            "## Contains must be a '| Path | Role | Notes |' table"
        return
    fi

    first_row="${rows[0]}"

    if [[ "$first_row" != *"Path"* ||
          "$first_row" != *"Role"* ||
          "$first_row" != *"Notes"* ]]; then
        error "$path" \
            "## Contains must be a '| Path | Role | Notes |' table"
        return
    fi

    # Skip header + separator.
    local index
    for (( index=2; index<${#rows[@]}; index++ )); do
        row="${rows[$index]}"

        IFS='|' read -ra cells <<< "${row#|}"

        if (( ${#cells[@]} < 3 )); then
            error "$path" "malformed table row: $row"
            continue
        fi

        path_cell="${cells[0]}"
        notes="${cells[2]}"

        # Trim whitespace.
        path_cell="${path_cell#"${path_cell%%[![:space:]]*}"}"
        path_cell="${path_cell%"${path_cell##*[![:space:]]}"}"

        notes="${notes#"${notes%%[![:space:]]*}"}"
        notes="${notes%"${notes##*[![:space:]]}"}"

        # Linked child:
        # [`api/`](api/README.md)
        if [[ "$path_cell" == *"]("* ]]; then
            continue
        fi

        # A cell may name one path or several comma-separated paths; either
        # way it opens with a backtick.
        if [[ "${path_cell:0:1}" != "$tick" ]]; then
            error "$path" \
                "Contains path cell must be backticked: $path_cell"
            continue
        fi

        entry="${path_cell#\`}"
        entry="${entry%\`}"

        # Only directory entries are checked here.
        [[ "$entry" != */ ]] && continue

        folder="$(dirname "$path")/${entry%/}"

        if [[ ! -d "$folder" ]]; then
            error "$path" \
                "Contains lists '$entry', which does not exist"
            continue
        fi

        if [[ -f "$folder/README.md" ]]; then
            has_readme=true
        else
            has_readme=false
        fi

        if [[ "$has_readme" == true ]]; then
            if [[ "$row" != *"]("* ]]; then
                error "$path" \
                    "child '$entry' has a README — link it from Contains"
            fi
        else
            if [[ "$notes" != *"no README"* ]]; then
                error "$path" \
                    "child '$entry' has no README — mark it '_(no README)_'"
            fi
        fi
    done
}

# ---------------------------------------------------------------------------
# §6.4 — Boundaries
# ---------------------------------------------------------------------------

check_boundaries() {
    local path="$1"
    local body

    body=$(get_section_body "$path" "Boundaries")

    if [[ "$body" != *"In scope"* ]]; then
        error "$path" \
            "## Boundaries must state 'In scope'"
    fi

    if [[ "$body" != *"Out of scope"* ]]; then
        error "$path" \
            "## Boundaries must state 'Out of scope'"
    fi
}

# ---------------------------------------------------------------------------
# §6.11 / §11 — Related
# ---------------------------------------------------------------------------

check_related() {
    local path="$1"
    local body
    local folder
    local child
    local name

    body=$(get_section_body "$path" "Related")

    if [[ "$body" != *"- Parent:"* ]]; then
        error "$path" \
            "## Related must name '- Parent:' (orphan README)"
    fi

    folder=$(dirname "$path")

    for child in "$folder"/*/; do
        [[ ! -d "$child" ]] && continue

        name=$(basename "$child")

        if should_skip_dir "$name"; then
            continue
        fi

        if [[ -f "$child/README.md" ]]; then
            if [[ "$body" != *"$name/README.md"* ]]; then
                error "$path" \
                    "child '$name/' has a README — link it from Related"
            fi
        fi
    done
}

# ---------------------------------------------------------------------------
# Find README files
# ---------------------------------------------------------------------------

find_readmes() {
    find . \
        \( \
            -type d \( \
                -name node_modules \
                -o -name dist \
                -o -name build \
                -o -name bin \
                -o -name obj \
                -o -name .git \
                -o -name coverage \
                -o -name vendor \
            \) -prune \
        \) \
        -o \
        -type f \
        -name README.md \
        -print |
        sort
}

# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------

while IFS= read -r path; do
    # Root README is out of scope.
    if [[ "$path" == "./README.md" || "$path" == "README.md" ]]; then
        continue
    fi

    checked=$((checked + 1))

    check_title "$path"
    check_sections "$path"
    check_links "$path"
    check_contains "$path"
    check_boundaries "$path"
    check_related "$path"

done < <(find_readmes)

printf '   %d README.md checked against %s — %d error(s), %d warning(s)\n' \
    "$checked" \
    "$GUIDE" \
    "$errors" \
    "$warnings"

(( errors == 0 ))