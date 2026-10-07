#!/usr/bin/env bash
# Builds the EDEA linux-x64 release tarball.
# Works on Linux and on Windows in Git Bash (needs dotnet and python on PATH;
# python is used for the tarball so the exec bit on the launcher survives).
set -euo pipefail

cd "$(dirname "$0")/../.."

VERSION=$(grep -oPm1 '(?<=<Version>)[0-9.]+' Directory.Build.props)
[ -n "$VERSION" ] || { echo "Could not read version from Directory.Build.props"; exit 1; }

PKG_NAME="EDEA-${VERSION}-linux-x64"
PKG_DIR="dist/${PKG_NAME}"
TARBALL="dist/${PKG_NAME}.tar.gz"

echo "==> dotnet publish (linux-x64, self-contained)"
dotnet publish src/EDEA.Avalonia/EDEA.Avalonia.csproj -p:PublishProfile=Linux

echo "==> Assembling package"
rm -rf "$PKG_DIR" "$TARBALL"
mkdir -p "$PKG_DIR"
cp -r publish-linux/. "$PKG_DIR/"
cp packaging/linux/edea.desktop packaging/linux/edea.png "$PKG_DIR/"

# Remove Windows-only artifacts from the Linux package if present.
rm -f "$PKG_DIR"/*.exe "$PKG_DIR"/portaudio.dll 2>/dev/null || true

echo "==> Creating tarball"
PKG_DIR="$PKG_DIR" PKG_NAME="$PKG_NAME" TARBALL="$TARBALL" python - <<'PYEOF'
import os, tarfile

pkg_dir = os.environ["PKG_DIR"].replace("\\", "/")
pkg_name = os.environ["PKG_NAME"]
tarball = os.environ["TARBALL"].replace("\\", "/")

with tarfile.open(tarball, "w:gz") as tf:
    for root, dirs, files in os.walk(pkg_dir):
        rel = os.path.relpath(root, pkg_dir)
        arc_root = pkg_name if rel == "." else f"{pkg_name}/{rel.replace(os.sep, '/')}"
        info = tf.gettarinfo(root, arcname=arc_root)
        info.mode = 0o755
        tf.addfile(info)
        for f in files:
            full = os.path.join(root, f)
            info = tf.gettarinfo(full, arcname=f"{arc_root}/{f}")
            # 755 for the launcher entry point and shared objects, 644 otherwise.
            info.mode = 0o755 if f == "EDEA.Avalonia" or f.endswith(".so") else 0o644
            with open(full, "rb") as fh:
                tf.addfile(info, fh)
PYEOF

echo "==> Done: $TARBALL"
