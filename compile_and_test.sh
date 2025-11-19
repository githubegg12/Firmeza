#!/bin/bash
# Complete compilation and testing script for Firmeza project

PROJECT_PATH="/home/Coder/Escritorio/David Vargas Varela_NO BORRAR/Project_ASP.NET/Firmeza"
cd "$PROJECT_PATH"

echo "=========================================="
echo "FIRMEZA PROJECT - COMPLETE BUILD TEST"
echo "=========================================="
echo ""

# Step 1: Clean
echo "[1/5] Cleaning previous builds..."
dotnet clean -q 2>/dev/null
echo "✓ Clean completed"
echo ""

# Step 2: Restore
echo "[2/5] Restoring NuGet packages..."
dotnet restore -q 2>/dev/null
echo "✓ Restore completed"
echo ""

# Step 3: Build
echo "[3/5] Building solution..."
BUILD_OUTPUT=$(dotnet build --configuration Debug --no-restore 2>&1)
BUILD_EXIT_CODE=$?

if [ $BUILD_EXIT_CODE -eq 0 ]; then
    echo "✓ Build SUCCESSFUL"
else
    echo "✗ Build FAILED"
    echo ""
    echo "Errors:"
    echo "$BUILD_OUTPUT" | grep -i "error"
    exit 1
fi
echo ""

# Step 4: Verify DLLs
echo "[4/5] Verifying compiled DLLs..."
DLLS=(
    "Firmeza.Domain/bin/Debug/net8.0/Firmeza.Domain.dll"
    "Firmeza.Application/bin/Debug/net8.0/Firmeza.Application.dll"
    "Firmeza.Infrastructure/bin/Debug/net8.0/Firmeza.Infrastructure.dll"
    "Firmeza.web/bin/Debug/net8.0/Firmeza.web.dll"
)

for dll in "${DLLS[@]}"; do
    if [ -f "$dll" ]; then
        echo "✓ $dll"
    else
        echo "✗ $dll - NOT FOUND"
        exit 1
    fi
done
echo ""

# Step 5: Summary
echo "[5/5] Build Summary"
echo "=========================================="
echo "Compilation Status: SUCCESS ✓"
echo "Configuration: Debug"
echo "Target Framework: net8.0"
echo ""
echo "All DLLs generated successfully!"
echo "=========================================="
echo ""
echo "Ready to run: dotnet run"

