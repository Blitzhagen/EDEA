#!/usr/bin/env bash
# Builds the EDEA linux-x64 AppImage.
# Requires dotnet and appimagetool (https://github.com/AppImage/appimagetool,
# expected on PATH or via the APPIMAGETOOL env var). FUSE is needed to run
# appimagetool itself; the produced AppImage also requires FUSE at runtime
# (or --appimage-extract-and-run).
set -euo pipefail

cd "$(dirname "$0")/../.."

VERSION=$(grep -oPm1 '(?<=<Version>)[0-9.]+' Directory.Build.props)
[ -n "$VERSION" ] || { echo "Could not read version from Directory.Build.props"; exit 1; }

APPIMAGETOOL=${APPIMAGETOOL:-appimagetool}
APP_DIR="dist/EDEA.AppDir"
OUT="dist/EDEA-${VERSION}-x86_64.AppImage"

echo "==> dotnet publish (linux-x64, self-contained)"
dotnet publish src/EDEA.Avalonia/EDEA.Avalonia.csproj -p:PublishProfile=Linux

echo "==> Assembling AppDir"
rm -rf "$APP_DIR"
mkdir -p "$APP_DIR/usr/bin"
cp -r publish-linux/. "$APP_DIR/usr/bin/"

cp packaging/linux/edea.desktop "$APP_DIR/edea.desktop"
cp packaging/linux/edea.png "$APP_DIR/edea.png"
ln -s usr/bin/EDEA.Avalonia "$APP_DIR/AppRun"

echo "==> appimagetool"
ARCH=x86_64 "$APPIMAGETOOL" "$APP_DIR" "$OUT"

echo "==> Done: $OUT"
