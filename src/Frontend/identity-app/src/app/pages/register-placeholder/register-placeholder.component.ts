import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-register-placeholder',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="register-placeholder">
      <p class="register-placeholder__text">Регистрация скоро будет доступна.</p>
      <a routerLink="/login" class="register-placeholder__link">Ко входу</a>
    </div>
  `,
  styles: `
    .register-placeholder {
      flex: 1 1 auto;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      gap: 1rem;
      padding: 2rem;
      font-family: var(--vt-font-family-default);
    }
    .register-placeholder__link {
      color: var(--vt-action-primary);
      font-weight: 600;
    }
  `,
})
export class RegisterPlaceholderComponent {}
