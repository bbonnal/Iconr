#!/usr/bin/env bash
set -euo pipefail

usage() {
  cat << 'USAGE'
Usage:
  scripts/create-icon.sh <icon-name> [--open]

Arguments:
  <icon-name>   Kebab-case icon name (example: app-window)

Options:
  --open        Open generated SVG in Inkscape
USAGE
}

if [[ ${1:-} == "" || ${1:-} == "-h" || ${1:-} == "--help" ]]; then
  usage
  exit 0
fi

icon_name_raw="$1"
open_in_inkscape="false"
if [[ ${2:-} == "--open" || ${1:-} == "--open" ]]; then
  open_in_inkscape="true"
fi

icon_name="${icon_name_raw//_/-}"
if [[ ! "$icon_name" =~ ^[a-z0-9]+(-[a-z0-9]+)*$ ]]; then
  echo "Invalid icon name '$icon_name_raw'. Use kebab-case like 'my-icon'." >&2
  exit 1
fi

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
root_dir="$(cd "$script_dir/.." && pwd)"
icon_enum_file="$root_dir/Icon.cs"
template_path="$root_dir/Icons/template.svg"
target_path="$root_dir/Icons/$icon_name.svg"
enum_name="${icon_name//-/_}"

if [[ ! -f "$template_path" ]]; then
  echo "Missing template: $template_path" >&2
  exit 1
fi

if grep -qE "^[[:space:]]*$enum_name," "$icon_enum_file"; then
  echo "Icon name already exists in Icon.cs: $enum_name" >&2
  exit 1
fi

if [[ -f "$target_path" ]]; then
  echo "Icon file already exists: $target_path" >&2
  exit 1
fi

cp "$template_path" "$target_path"
echo "Created $target_path"

tmp_file="$(mktemp)"

awk -v new_name="$enum_name" '
  BEGIN {
    in_enum = 0;
    inserted = 0;
  }

  {
    if ($0 ~ /public enum Icon/) {
      in_enum = 1;
    }

    if (in_enum && !inserted && $0 ~ /^[ \t]*[a-z0-9_]+,/) {
      existing = $0;
      sub(/^[ \t]+/, "", existing);
      sub(/,.*/, "", existing);

      if (new_name < existing) {
        print "\t" new_name ",";
        inserted = 1;
      }
    }

    if (in_enum && !inserted && $0 ~ /^[ \t]*}/) {
      print "\t" new_name ",";
      inserted = 1;
    }

    print $0;
  }

  END {
    if (!inserted) {
      exit 2;
    }
  }
' "$icon_enum_file" > "$tmp_file"

mv "$tmp_file" "$icon_enum_file"
echo "Added '$enum_name' to Icon.cs"

if [[ "$open_in_inkscape" == "true" ]]; then
  if command -v inkscape > /dev/null 2>&1; then
    inkscape "$target_path" > /dev/null 2>&1 &
    disown || true
    echo "Opened generated icon in Inkscape"
  else
    echo "Inkscape not found in PATH; icon was created but not opened." >&2
    exit 1
  fi
fi

echo "Done"
