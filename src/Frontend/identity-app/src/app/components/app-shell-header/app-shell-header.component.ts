import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { NavigationEnd, Router, RouterLink } from '@angular/router';
import { filter, firstValueFrom, Subscription } from 'rxjs';
import { AuthService, UserProfile } from '../../services/auth';

@Component({
  selector: 'app-shell-header',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './app-shell-header.component.html',
  styleUrl: './app-shell-header.component.scss',
})
export class AppShellHeaderComponent implements OnInit, OnDestroy {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  private routerSub: Subscription | null = null;

  protected readonly profile = signal<UserProfile | null>(null);
  protected readonly isVisible = signal(this.shouldShowHeader());

  ngOnInit(): void {
    void this.syncHeader();

    this.routerSub = this.router.events
      .pipe(filter((event): event is NavigationEnd => event instanceof NavigationEnd))
      .subscribe(() => {
        void this.syncHeader();
      });
  }

  ngOnDestroy(): void {
    this.routerSub?.unsubscribe();
  }

  protected displayInitials(userName: string | null | undefined): string {
    const name = userName?.trim();
    if (!name) {
      return '?';
    }
    const parts = name.split(/\s+/).filter(Boolean);
    if (parts.length >= 2) {
      return `${parts[0]![0]!}${parts[1]![0]!}`.toUpperCase();
    }
    return name.slice(0, 2).toUpperCase();
  }

  protected onLogout(): void {
    this.auth.logout();
    void this.router.navigate(['/login']);
  }

  private shouldShowHeader(): boolean {
    const path = this.router.url.split('?')[0]?.split('#')[0] ?? '';
    return path === '/account' || path === '/dashboard';
  }

  private async syncHeader(): Promise<void> {
    const visible = this.shouldShowHeader();
    this.isVisible.set(visible);

    if (!visible) {
      this.profile.set(null);
      return;
    }

    try {
      const profile = await firstValueFrom(this.auth.getProfile());
      this.profile.set(profile);
    } catch {
      this.profile.set(null);
    }
  }
}
