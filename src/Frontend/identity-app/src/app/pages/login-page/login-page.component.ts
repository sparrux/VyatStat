import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.scss',
})
export class LoginPageComponent implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  protected username = '';
  protected password = '';
  protected readonly isSubmitting = signal(false);
  protected readonly submitError = signal<string | null>(null);

  ngOnInit(): void {
    if (this.auth.isAuthenticated()) {
      void this.router.navigate(['/account']);
      return;
    }

    const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
    if (!returnUrl) {
      void this.auth.startAuthorizationFlow();
    }
  }

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
      const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
      await this.auth.login(login, this.password, returnUrl);
    } catch {
      this.isSubmitting.set(false);
      this.submitError.set('Failed to login. Please try again.');
    }
  }
}
