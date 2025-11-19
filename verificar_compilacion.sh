#!/bin/bash
# Script de verificación de compilación del proyecto Firmeza

echo "======================================"
echo "VERIFICACIÓN DE COMPILACIÓN - FIRMEZA"
echo "======================================"
echo ""

# Cambiar al directorio del proyecto
cd "/home/Coder/Escritorio/David Vargas Varela_NO BORRAR/Project_ASP.NET/Firmeza"

# Limpiar compilaciones anteriores
echo "1. Limpiando compilaciones anteriores..."
dotnet clean -q 2>/dev/null

echo ""
echo "2. Compilando proyecto..."
RESULT=$(dotnet build --configuration Debug 2>&1)
BUILD_STATUS=$?

echo ""
echo "3. Verificando resultado de compilación..."

if [ $BUILD_STATUS -eq 0 ]; then
    echo "✅ COMPILACIÓN EXITOSA"
    echo ""
    echo "Verificando archivos DLL generados:"
    
    # Verificar Firmeza.Application.dll
    if [ -f "Firmeza.Application/bin/Debug/net8.0/Firmeza.Application.dll" ]; then
        echo "✅ Firmeza.Application.dll"
    fi
    
    # Verificar Firmeza.Domain.dll
    if [ -f "Firmeza.Domain/bin/Debug/net8.0/Firmeza.Domain.dll" ]; then
        echo "✅ Firmeza.Domain.dll"
    fi
    
    # Verificar Firmeza.Infrastructure.dll
    if [ -f "Firmeza.Infrastructure/bin/Debug/net8.0/Firmeza.Infrastructure.dll" ]; then
        echo "✅ Firmeza.Infrastructure.dll"
    fi
    
    # Verificar Firmeza.web.dll
    if [ -f "Firmeza.web/bin/Debug/net8.0/Firmeza.web.dll" ]; then
        echo "✅ Firmeza.web.dll"
    fi
    
    echo ""
    echo "======================================"
    echo "ESTADO: LISTO PARA PRODUCCIÓN ✅"
    echo "======================================"
else
    echo "❌ COMPILACIÓN FALLIDA"
    echo ""
    echo "Mostrando errores:"
    echo "$RESULT" | grep -E "error|ERROR"
    exit 1
fi

