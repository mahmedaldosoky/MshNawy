import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OnboardingLoginComponent } from './onboarding/login/onboarding-login.component';
import { OnboardingKycComponent } from './onboarding/kyc/onboarding-kyc.component';
import { KycStatusComponent } from './onboarding/kyc-status/kyc-status.component';
import { KycReviewComponent } from './admin/kyc-review/kyc-review.component';
import { authGuard } from './shared/guards/auth.guard';
import { kycGuard } from './shared/guards/kyc.guard';

const routes: Routes = [
  { path: '', redirectTo: 'onboarding/login', pathMatch: 'full' },

  // Public routes
  { path: 'onboarding/login', component: OnboardingLoginComponent },

  // Auth-protected routes
  { path: 'onboarding/kyc', component: OnboardingKycComponent, canActivate: [authGuard] },
  { path: 'onboarding/kyc-status', component: KycStatusComponent, canActivate: [authGuard] },

  // KYC-protected routes (financial features — added in future phases)
  // { path: 'wallet', canActivate: [authGuard, kycGuard], ... },
  // { path: 'offerings', canActivate: [authGuard, kycGuard], ... },
  // { path: 'portfolio', canActivate: [authGuard, kycGuard], ... },
  // { path: 'subscription', canActivate: [authGuard, kycGuard], ... },

  // Admin routes
  { path: 'admin/kyc-review', component: KycReviewComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
