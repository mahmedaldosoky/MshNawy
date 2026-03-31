import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { map, tap } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

/**
 * KYC guard - ensures user has completed KYC verification before accessing financial features.
 * Per Constitution I & spec US1: Users MUST complete KYC and receive admin approval
 * before accessing deposits, withdrawals, or investments.
 */
export const kycGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const authService = inject(AuthService);

  return authService.checkKycApproved().pipe(
    tap(approved => {
      if (!approved) {
        router.navigate(['/onboarding/kyc'], { queryParams: { returnUrl: state.url } });
      }
    })
  );
};
