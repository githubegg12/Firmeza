import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService, RegisterRequest } from '../../services/auth';

@Component({
  selector: 'app-register',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {
  private authService = inject(AuthService);
  private router = inject(Router);

  formData: RegisterRequest = {
    firstName: '',
    lastName: '',
    email: '',
    documentId: '',
    phoneNumber: '',
    address: '',
    password: '',
    confirmPassword: ''
  };

  loading = signal(false);
  error = signal<string | null>(null);
  success = signal(false);

  onSubmit() {
    // Validate all fields
    if (!this.formData.firstName || !this.formData.lastName || !this.formData.email ||
      !this.formData.documentId || !this.formData.phoneNumber || !this.formData.address ||
      !this.formData.password || !this.formData.confirmPassword) {
      this.error.set('Por favor complete todos los campos');
      return;
    }

    // Validate password match
    if (this.formData.password !== this.formData.confirmPassword) {
      this.error.set('Las contraseñas no coinciden');
      return;
    }

    // Validate password length
    if (this.formData.password.length < 6) {
      this.error.set('La contraseña debe tener al menos 6 caracteres');
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    this.authService.register(this.formData).subscribe({
      next: (response) => {
        this.loading.set(false);
        if (response.success) {
          this.success.set(true);
          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 2000);
        } else {
          this.error.set(response.message || 'Error al registrarse');
        }
      },
      error: (err) => {
        this.loading.set(false);
        console.error('Registration error:', err);
        this.error.set(err.error?.message || 'Error al registrarse. Por favor intenta de nuevo.');
      }
    });
  }
}
