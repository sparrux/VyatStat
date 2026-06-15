import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.scss',
})
export class LoginPageComponent {
  private readonly auth = inject(AuthService);

  protected username = '';
  protected password = '';
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

    this.isSubmitting.set(true);
    try {
      await this.auth.login(login, this.password);
    } catch {
      this.isSubmitting.set(false);
      this.submitError.set('Failed to login. Please try again.');
    }
  }
}
