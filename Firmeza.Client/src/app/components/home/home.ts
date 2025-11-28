import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
    selector: 'app-home',
    standalone: true,
    imports: [CommonModule, RouterModule],
    templateUrl: './home.html',
    styleUrls: ['./home.css']
})
export class HomeComponent {
    features = [
        {
            icon: '🏗️',
            title: 'Insumos de Construcción',
            description: 'Materiales de primera calidad para tus proyectos',
            items: ['Cemento', 'Acero', 'Agregados', 'Herramientas']
        },
        {
            icon: '🚜',
            title: 'Vehículos Industriales',
            description: 'Renta de maquinaria pesada y equipos especializados',
            items: ['Excavadoras', 'Grúas', 'Montacargas', 'Retroexcavadoras']
        },
        {
            icon: '⚡',
            title: 'Entrega Rápida',
            description: 'Servicio de entrega en tiempo récord',
            items: ['24/7 Disponible', 'Cobertura Nacional', 'Tracking en Tiempo Real']
        },
        {
            icon: '💰',
            title: 'Mejores Precios',
            description: 'Precios competitivos y planes de financiamiento',
            items: ['Descuentos por Volumen', 'Crédito Disponible', 'Sin Intereses']
        }
    ];

    stats = [
        { value: '500+', label: 'Productos' },
        { value: '1000+', label: 'Clientes Satisfechos' },
        { value: '50+', label: 'Vehículos Disponibles' },
        { value: '24/7', label: 'Soporte' }
    ];

    testimonials = [
        {
            name: 'Juan Pérez',
            role: 'Constructor',
            comment: 'Excelente servicio y productos de calidad. La renta de maquinaria es muy eficiente.',
            rating: 5
        },
        {
            name: 'María González',
            role: 'Ingeniera Civil',
            comment: 'Los mejores precios del mercado. Siempre encuentro lo que necesito.',
            rating: 5
        },
        {
            name: 'Carlos Ramírez',
            role: 'Contratista',
            comment: 'Entrega rápida y personal muy profesional. Totalmente recomendado.',
            rating: 5
        }
    ];
}
