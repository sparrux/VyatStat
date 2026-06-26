import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-callback-page',
  standalone: true,
  templateUrl: './callback-page.component.html',
  styleUrl: './callback-page.component.scss',
})
export class CallbackPageComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  ngOnInit(): void {
    this.route.queryParams.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((params) => {
      if (params['error']) {
        this.authService.clearOAuthTransientState();
        void this.authService.startAuthorizationFlow();
        return;
      }

      const code = params['code'];
      const state = params['state'];
      if (!code || !this.authService.validateAndConsumeOAuthState(state)) {
        void this.authService.startAuthorizationFlow();
        return;
      }

      this.authService.exchangeCodeForToken(code).subscribe({
        next: (tokens) => {
          this.authService.applyOAuthTokens(tokens);
          void this.router.navigate(['/account']);
        },
        error: () => {
          void this.authService.startAuthorizationFlow();
        },
      });
    });
  }
}
