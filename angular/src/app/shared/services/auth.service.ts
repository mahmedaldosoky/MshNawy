import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { AuthResponseDto } from '../models/auth-response.dto';
import { KycStatus } from '../models/kyc-status';
import { KycStatusDto } from '../models/kyc-status.dto';

const TOKEN_KEY = 'msn_access_token';
const USER_ID_KEY = 'msn_user_id';
const KYC_STATUS_KEY = 'msn_kyc_status';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiBase = environment.apiUrl;

  constructor(private http: HttpClient) {}

  storeAuth(result: AuthResponseDto): void {
    localStorage.setItem(TOKEN_KEY, result.accessToken);
    localStorage.setItem(USER_ID_KEY, result.userId);
    localStorage.setItem(KYC_STATUS_KEY, result.kycStatus);
  }

  getAccessToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  getUserId(): string | null {
    return localStorage.getItem(USER_ID_KEY);
  }

  getStoredKycStatus(): KycStatus | null {
    return localStorage.getItem(KYC_STATUS_KEY) as KycStatus | null;
  }

  isAuthenticated(): boolean {
    const token = this.getAccessToken();
    if (!token) {
      return false;
    }
    return !this.isTokenExpired(token);
  }

  clearAuth(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_ID_KEY);
    localStorage.removeItem(KYC_STATUS_KEY);
  }

  checkKycApproved(): Observable<boolean> {
    if (!this.isAuthenticated()) {
      return of(false);
    }

    const stored = this.getStoredKycStatus();
    if (stored === 'Approved') {
      return of(true);
    }

    return this.http.get<KycStatusDto>(`${this.apiBase}/app/kyc/status`).pipe(
      map(result => {
        localStorage.setItem(KYC_STATUS_KEY, result.status);
        return result.status === 'Approved';
      }),
      catchError(() => of(false))
    );
  }

  private isTokenExpired(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const exp = payload.exp;
      if (!exp) {
        return false;
      }
      return Date.now() >= exp * 1000;
    } catch {
      return true;
    }
  }
}
