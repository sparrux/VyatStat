import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface UserProfile {
  id: string;
  userName: string | null;
}

export interface UserClaims {
  isAdmin: boolean;
  readUsers: boolean;
  updateUserPermissions: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private authServerUrl = 'https://localhost:7019'; // Укажите порт вашего .NET API
  private clientId = 'angular-client';
  private redirectUri = window.location.origin + '/callback';

  constructor(private http: HttpClient) {}

  getProfile(): Observable<UserProfile> {
    const token = localStorage.getItem('access_token');

    const headers = new HttpHeaders({
      'Authorization': 'Bearer ' + token
    });

    return this.http.get<UserProfile>(`${this.authServerUrl}/profile`, { headers });
  }

  getUserPermissions(userId: string): Observable<UserClaims> {
    const token = localStorage.getItem('access_token');

    const headers = new HttpHeaders({
      'Authorization': 'Bearer ' + token
    });

    return this.http.get<UserClaims>(
      `${this.authServerUrl}/${userId}/permissions`,
      { headers }
    );
  }

  logout(): void {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('code_verifier');
  }

  isAuthenticated(): boolean {
    const token = localStorage.getItem('access_token');

    return !!token;
  }

  // 1. Генерация случайной строки для PKCE (Code Verifier)
  private generateVerifier(): string {
    const array = new Uint32Array(56);
    crypto.getRandomValues(array);
    return Array.from(array, dec => ('0' + dec.toString(16)).substr(-2)).join('');
  }

  // 2. Хэширование строки через SHA-256 (Code Challenge)
  private async generateChallenge(verifier: string): Promise<string> {
    const encoder = new TextEncoder();
    const data = encoder.encode(verifier);
    const hash = await crypto.subtle.digest('SHA-256', data);
    return btoa(String.fromCharCode(...new Uint8Array(hash)))
      .replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
  }

  register(username: string, password: string): Observable<any> {
    const url = `${this.authServerUrl}/register`;

    // Структура должна строго соответствовать вашему классу RegistrationRequest на бэкенде
    const body = {
      login: username,
      password: password
    };

    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    // Отправляем обычный JSON-запрос
    return this.http.post<any>(url, body, { headers });
  }

  // 3. Отправка логина/пароля на /connect/authorize
  async login(username: string, password: string): Promise<void> {
    const verifier = this.generateVerifier();
    localStorage.setItem('code_verifier', verifier);

    const challenge = await this.generateChallenge(verifier);

    // 1. Создаем объект с параметрами запроса
    const params: { [key: string]: string } = {
      'client_id': this.clientId,
      'response_type': 'code',
      'scope': 'openid profile offline_access',
      'redirect_uri': this.redirectUri,
      'code_challenge': challenge,
      'code_challenge_method': 'S256',
      'username': username,
      'password': password
    };

    // 2. Создаем виртуальную HTML-форму
    const form = document.createElement('form');
    form.method = 'POST';
    form.action = `${this.authServerUrl}/connect/authorize`; // Ссылка на бэкенд

    // 3. Наполняем форму скрытыми полями input
    for (const key in params) {
      if (params.hasOwnProperty(key)) {
        const hiddenField = document.createElement('input');
        hiddenField.type = 'hidden';
        hiddenField.name = key;
        hiddenField.value = params[key];
        form.appendChild(hiddenField);
      }
    }

    // 4. Добавляем форму на страницу и принудительно отправляем её
    document.body.appendChild(form);
    form.submit(); // Браузер сам сделает POST и сам перейдет по редиректу 302 на /callback
  }

  // 4. Обмен полученного кода на Access и Refresh токены
  exchangeCodeForToken(code: string): Observable<any> {
    const verifier = localStorage.getItem('code_verifier') || '';

    const body = new HttpParams()
      .set('client_id', this.clientId)
      .set('aud', 'vyatka-identity-api')
      .set('grant_type', 'authorization_code')
      .set('code', code)
      .set('redirect_uri', this.redirectUri)
      .set('code_verifier', verifier);

    const headers = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });

    return this.http.post(`${this.authServerUrl}/connect/token`, body.toString(), { headers });
  }
}
