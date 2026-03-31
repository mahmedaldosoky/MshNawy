import { NgModule, LOCALE_ID } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { registerLocaleData } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import localeArEg from '@angular/common/locales/ar-EG';
import { AppComponent } from './app.component';
import { AppShellModule } from './shared/components/app-shell/app-shell.module';
import { AppRoutingModule } from './app-routing.module';
import { OnboardingLoginComponent } from './onboarding/login/onboarding-login.component';
import { OnboardingKycComponent } from './onboarding/kyc/onboarding-kyc.component';
import { KycStatusComponent } from './onboarding/kyc-status/kyc-status.component';
import { KycReviewComponent } from './admin/kyc-review/kyc-review.component';

// Register Arabic (Egypt) locale for DatePipe, CurrencyPipe, DecimalPipe
// Per Constitution I: All numbers, dates, and currency values MUST render in ar-EG locale
registerLocaleData(localeArEg);

@NgModule({
  declarations: [
    AppComponent,
    OnboardingLoginComponent,
    OnboardingKycComponent,
    KycStatusComponent,
    KycReviewComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    AppRoutingModule,
    AppShellModule
  ],
  providers: [
    { provide: LOCALE_ID, useValue: 'ar-EG' }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
