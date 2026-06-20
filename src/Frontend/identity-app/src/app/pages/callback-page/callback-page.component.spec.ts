import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { AuthService } from '../../services/auth.service';
import { CallbackPageComponent } from './callback-page.component';

describe('CallbackPageComponent', () => {
  let component: CallbackPageComponent;
  let fixture: ComponentFixture<CallbackPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CallbackPageComponent],
      providers: [
        provideRouter([
          { path: 'login', component: CallbackPageComponent },
          { path: 'account', component: CallbackPageComponent },
        ]),
        {
          provide: AuthService,
          useValue: {
            exchangeCodeForToken: () => of({ access_token: 'token' }),
            applyOAuthTokens: () => undefined,
          },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CallbackPageComponent);
    component = fixture.componentInstance;
  });

  it('should create', async () => {
    await fixture.whenStable();
    expect(component).toBeTruthy();
  });
});
