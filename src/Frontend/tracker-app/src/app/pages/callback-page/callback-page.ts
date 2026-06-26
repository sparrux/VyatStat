import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-callback-page',
  imports: [],
  templateUrl: './callback-page.html',
  styleUrl: './callback-page.scss',
})
export class CallbackPage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  ngOnInit(): void {
    this.route.queryParams.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((params) => {
      const code = params['code'];
      if (!code) {
        void this.authService.startAuthorizationFlow();
        return;
      }

      this.authService.exchangeCodeForToken(code).subscribe({
        next: (tokens) => {
          this.authService.applyOAuthTokens(tokens);
          void this.router.navigate(['/calendar']);
        },
        error: () => {
          void this.authService.startAuthorizationFlow();
        },
      });
    });
  }
}
