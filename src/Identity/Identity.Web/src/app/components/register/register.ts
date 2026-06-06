import { Component } from '@angular/core';
import { AuthService } from '../../services/auth';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, CommonModule], // Обязательно импортируем для форм и директив (*ngIf / *ngFor)
  templateUrl: './register.html',
  styleUrls: ['./register.scss']
})
export class RegisterComponent {
  username = '';
  password = '';
  errors: string[] = []; // Массив для хранения ошибок от ASP.NET Core

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit(event: Event) {
    event.preventDefault();
    this.errors = []; // Сбрасываем старые ошибки

    this.authService.register(this.username, this.password).subscribe({
      next: (response) => {
        alert(response.message || 'Регистрация успешна!');
        // Перенаправляем пользователя на страницу входа, чтобы он залогинился
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error('Ошибка регистрации:', err);

        // Обрабатываем ошибки ModelState (например, слишком слабый пароль от ASP.NET Identity)
        if (err.error && typeof err.error === 'object') {
          for (const key in err.error) {
            if (err.error.hasOwnProperty(key)) {
              // Добавляем описание ошибок в массив для вывода на экран
              this.errors.push(...err.error[key]);
            }
          }
        } else {
          this.errors.push('Произошла непредвиденная ошибка при регистрации.');
        }
      }
    });
  }
}
