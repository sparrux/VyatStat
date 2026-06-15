import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-callback',
  standalone: true,
  template: `<p>Авторизация... Пожалуйста, подождите.</p>`
})
export class CallbackComponent implements OnInit {
  constructor(
    private route: ActivatedRoute,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Извлекаем параметр 'code' из строки URL
    this.route.queryParams.subscribe(params => {
      const code = params['code'];
      if (code) {
        // Обмениваем его на токены
        this.authService.exchangeCodeForToken(code).subscribe({
          next: (tokens) => {
            this.authService.applyOAuthTokens(tokens);
            void this.router.navigate(['/account']);
          },
          error: (err) => console.error('Ошибка обмена кода на токен:', err)
        });
      }
    });
  }
}
