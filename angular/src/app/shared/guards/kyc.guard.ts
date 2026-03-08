import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';

/**
 * KYC guard - ensures user has completed KYC verification before accessing financial features.
 * Per Constitution I & spec US1: Users MUST complete KYC (national ID, photos, info) and receive
 * admin approval before accessing deposits, withdrawals, or investments.
 * Uses functional guard API (Angular 15+ recommended pattern).
 */
export const kycGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);

  // TODO: Connect to KycService to check actual KYC approval status
  // const kycService = inject(KycService);
  // const kycApproved = kycService.isApproved();

  const kycApproved = false; // Placeholder

  if (kycApproved) {
    return true;
  }

  // Redirect to KYC flow with return URL
  router.navigate(['/onboarding/kyc'], { queryParams: { returnUrl: state.url } });
  return false;
};
