import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';

/**
 * Auth guard - ensures user is authenticated via OTP verification.
 * Per Constitution I: Only authenticated Egyptian users with verified phone numbers can access features.
 * Uses functional guard API (Angular 15+ recommended pattern).
 */
export const authGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);

  // TODO: Connect to AuthService to check actual authentication status
  // const authService = inject(AuthService);
  // const isAuthenticated = authService.isAuthenticated();

  const isAuthenticated = false; // Placeholder

  if (isAuthenticated) {
    return true;
  }

  // Redirect to login with return URL for post-login navigation
  router.navigate(['/onboarding/login'], { queryParams: { returnUrl: state.url } });
  return false;
};
