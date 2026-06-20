import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './register-page.component.html',
  styleUrl: './register-page.component.scss',
})
export class RegisterPageComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  protected username = '';
  protected password = '';
  protected repeatPassword = '';
  protected readonly isSubmitting = signal(false);
  protected readonly submitError = signal<string | null>(null);

  protected async onSubmit(event: Event): Promise<void> {
    event.preventDefault();
    this.submitError.set(null);

    const login = this.username.trim();
    if (!login || !this.password) {
      this.submitError.set('Enter your login and password');
      return;
    }

    if (this.password !== this.repeatPassword) {
      this.submitError.set('Passwords do not match');
      return;
    }

    this.isSubmitting.set(true);
    try {
      await firstValueFrom(this.auth.register(login, this.password));
      await this.router.navigate(['/login']);
    } catch {
      this.isSubmitting.set(false);
      this.submitError.set('Registration failed. Please try again.');
    }
  }
}
