import { Component, OnDestroy } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { AuthResponseDto } from '../../shared/models/auth-response.dto';
import { SendOtpResponseDto } from '../../shared/models/send-otp-response.dto';
import { LocalizationService } from '../../shared/services/localization.service';
import { AuthService } from '../../shared/services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-onboarding-login',
  templateUrl: './onboarding-login.component.html',
  styleUrls: ['./onboarding-login.component.scss']
})
export class OnboardingLoginComponent implements OnDestroy {
  private readonly apiBase = environment.apiUrl;
  private timerId?: number;

  isSending = false;
  isVerifying = false;
  countdownSeconds = 0;
  errorMessage = '';
  authResult?: AuthResponseDto;

  form = this.fb.group({
    phoneDigits: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]],
    otpCode: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]]
  });

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private l: LocalizationService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnDestroy(): void {
    this.clearTimer();
  }

  sendOtp(): void {
    this.errorMessage = '';
    if (this.form.controls.phoneDigits.invalid) {
      this.errorMessage = this.l.instant('Onboarding.InvalidPhone');
      return;
    }

    const phoneNumber = `+20${this.form.controls.phoneDigits.value}`;
    this.isSending = true;
    this.http
      .post<SendOtpResponseDto>(`${this.apiBase}/app/auth/send-otp`, { phoneNumber })
      .subscribe({
        next: (result) => {
          this.startCountdown(result.expiresInSeconds);
          this.isSending = false;
        },
        error: () => {
          this.errorMessage = this.l.instant('Onboarding.OtpSendFailed');
          this.isSending = false;
        }
      });
  }

  verifyOtp(): void {
    this.errorMessage = '';
    if (this.form.controls.phoneDigits.invalid || this.form.controls.otpCode.invalid) {
      this.errorMessage = this.l.instant('Onboarding.InvalidOtp');
      return;
    }

    const phoneNumber = `+20${this.form.controls.phoneDigits.value}`;
    const otpCode = this.form.controls.otpCode.value ?? '';

    this.isVerifying = true;
    this.http
      .post<AuthResponseDto>(`${this.apiBase}/app/auth/verify-otp`, { phoneNumber, otpCode })
      .subscribe({
        next: (result) => {
          this.authResult = result;
          this.authService.storeAuth(result);
          this.isVerifying = false;
          this.router.navigate(['/onboarding/kyc-status']);
        },
        error: () => {
          this.errorMessage = this.l.instant('Onboarding.OtpVerifyFailed');
          this.isVerifying = false;
        }
      });
  }

  get formattedCountdown(): string {
    const minutes = Math.floor(this.countdownSeconds / 60);
    const seconds = this.countdownSeconds % 60;
    return `${minutes}:${seconds.toString().padStart(2, '0')}`;
  }

  private startCountdown(seconds: number): void {
    this.countdownSeconds = seconds;
    this.clearTimer();
    this.timerId = window.setInterval(() => {
      this.countdownSeconds = Math.max(0, this.countdownSeconds - 1);
      if (this.countdownSeconds === 0) {
        this.clearTimer();
      }
    }, 1000);
  }

  private clearTimer(): void {
    if (this.timerId) {
      window.clearInterval(this.timerId);
      this.timerId = undefined;
    }
  }
}
