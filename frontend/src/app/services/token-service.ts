import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Injectable({
  providedIn: 'root',
})
export class TokenService {
  private readonly TOKEN_KEY = 'jwt_token';
  private platformId = inject(PLATFORM_ID);

  public isBrowser(): boolean {
    return isPlatformBrowser(this.platformId);
  }

  setToken(token: string): void {
    if (this.isBrowser()) {
      localStorage.setItem(this.TOKEN_KEY, token);
    }
  }

  getToken(): string | null {
    if (!this.isBrowser()) return null;
    return localStorage.getItem(this.TOKEN_KEY);
  }

  removeToken(): void {
    if (this.isBrowser()) {
      localStorage.removeItem(this.TOKEN_KEY);
    }
  }

  // ✅ Check login using token expiry
  isLoggedIn(): boolean {
    const decoded = this.decodeToken();
    if (!decoded || !decoded.exp) return false;

    const now = Math.floor(Date.now() / 1000);
    return decoded.exp > now;
  }

  // ================= JWT DECODE =================
  decodeToken(): any | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      return JSON.parse(atob(token.split('.')[1]));
    } catch {
      return null;
    }
  }

  // ================= HELPERS =================
  getUserRole(): string | null {
    const decoded = this.decodeToken();
    return (
      decoded?.role ||
      decoded?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
      null
    );
  }

  getUserId(): number {
    const decoded = this.decodeToken();
    if (!decoded) return 0;

    return Number(
      decoded.nameid ||
      decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ||
      0
    );
  }

  getPatientId(): number {
    const id = this.getUserId();
    return id ? Number(id) : 0;
  }

  getRole(): string | null {
    return this.getUserRole();
  }
}