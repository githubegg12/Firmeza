#!/bin/bash
# Script para limpiar archivos antiguos después de la reorganización

# ARCHIVOS A ELIMINAR - Controllers antiguos
echo "Eliminando controllers antiguos..."
rm -f "/home/Coder/Escritorio/David Vargas Varela_NO BORRAR/Project_ASP.NET/Firmeza/Firmeza.web/Controllers/AccountController.cs"
rm -f "/home/Coder/Escritorio/David Vargas Varela_NO BORRAR/Project_ASP.NET/Firmeza/Firmeza.web/Controllers/AdminController.cs"
rm -f "/home/Coder/Escritorio/David Vargas Varela_NO BORRAR/Project_ASP.NET/Firmeza/Firmeza.web/Controllers/BulkImportController.cs"
rm -f "/home/Coder/Escritorio/David Vargas Varela_NO BORRAR/Project_ASP.NET/Firmeza/Firmeza.web/Controllers/ClientController.cs"
rm -f "/home/Coder/Escritorio/David Vargas Varela_NO BORRAR/Project_ASP.NET/Firmeza/Firmeza.web/Controllers/ErrorHandlingController.cs"
rm -f "/home/Coder/Escritorio/David Vargas Varela_NO BORRAR/Project_ASP.NET/Firmeza/Firmeza.web/Controllers/SalesController.cs"

# ARCHIVOS A ELIMINAR - ViewModels antiguos
echo "Eliminando ViewModels antiguos..."
rm -f "/home/Coder/Escritorio/David Vargas Varela_NO BORRAR/Project_ASP.NET/Firmeza/Firmeza.web/ViewModels/ClientViewModel.cs"
rm -f "/home/Coder/Escritorio/David Vargas Varela_NO BORRAR/Project_ASP.NET/Firmeza/Firmeza.web/ViewModels/LoginViewModel.cs"
rm -f "/home/Coder/Escritorio/David Vargas Varela_NO BORRAR/Project_ASP.NET/Firmeza/Firmeza.web/ViewModels/RegisterViewModel.cs"

# ARCHIVOS A ELIMINAR - Services antiguos
echo "Eliminando Services antiguos..."
rm -f "/home/Coder/Escritorio/David Vargas Varela_NO BORRAR/Project_ASP.NET/Firmeza/Firmeza.Application/Services/CreateProductService.cs"
rm -f "/home/Coder/Escritorio/David Vargas Varela_NO BORRAR/Project_ASP.NET/Firmeza/Firmeza.Application/Services/ReadProductService.cs"
rm -f "/home/Coder/Escritorio/David Vargas Varela_NO BORRAR/Project_ASP.NET/Firmeza/Firmeza.Application/Services/UpdateProductService.cs"
rm -f "/home/Coder/Escritorio/David Vargas Varela_NO BORRAR/Project_ASP.NET/Firmeza/Firmeza.Application/Services/DeleteProductService.cs"

# ARCHIVOS A ELIMINAR - Interfaces antigas
echo "Eliminando Interfaces antigas..."
rm -f "/home/Coder/Escritorio/David Vargas Varela_NO BORRAR/Project_ASP.NET/Firmeza/Firmeza.Application/Interfaces/IAuthService.cs"
rm -f "/home/Coder/Escritorio/David Vargas Varela_NO BORRAR/Project_ASP.NET/Firmeza/Firmeza.Application/Interfaces/IBulkImportService.cs"
rm -f "/home/Coder/Escritorio/David Vargas Varela_NO BORRAR/Project_ASP.NET/Firmeza/Firmeza.Application/Interfaces/IPdfService.cs"
rm -f "/home/Coder/Escritorio/David Vargas Varela_NO BORRAR/Project_ASP.NET/Firmeza/Firmeza.Application/Interfaces/ICreateProductService.cs"
rm -f "/home/Coder/Escritorio/David Vargas Varela_NO BORRAR/Project_ASP.NET/Firmeza/Firmeza.Application/Interfaces/IReadProductService.cs"
rm -f "/home/Coder/Escritorio/David Vargas Varela_NO BORRAR/Project_ASP.NET/Firmeza/Firmeza.Application/Interfaces/IUpdateProductService.cs"
rm -f "/home/Coder/Escritorio/David Vargas Varela_NO BORRAR/Project_ASP.NET/Firmeza/Firmeza.Application/Interfaces/IDeleteProductService.cs"
rm -f "/home/Coder/Escritorio/David Vargas Varela_NO BORRAR/Project_ASP.NET/Firmeza/Firmeza.Application/Interfaces/IDbInitializer.cs"

# ARCHIVOS A ELIMINAR - Placeholder
echo "Eliminando placeholders..."
rm -f "/home/Coder/Escritorio/David Vargas Varela_NO BORRAR/Project_ASP.NET/Firmeza/Firmeza.Infrastructure/Services/.placeholder"

echo "✅ Limpieza completada exitosamente"

