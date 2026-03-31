import { Component } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { KycStatusDto } from '../../shared/models/kyc-status.dto';
import { KycUploadResponseDto } from '../../shared/models/kyc-upload-response.dto';
import { LocalizationService } from '../../shared/services/localization.service';

@Component({
  selector: 'app-onboarding-kyc',
  templateUrl: './onboarding-kyc.component.html',
  styleUrls: ['./onboarding-kyc.component.scss']
})
export class OnboardingKycComponent {
  private readonly apiBase = environment.apiUrl;

  currentStep = 1;
  errorMessage = '';
  successMessage = '';
  isSubmitting = false;

  frontToken = '';
  backToken = '';
  selfieToken = '';

  form = this.fb.group({
    fullNameArabic: ['', [Validators.required]],
    dateOfBirth: ['', [Validators.required]],
    nationalIdNumber: ['', [Validators.required, Validators.pattern(/^\d{14}$/)]]
  });

  constructor(private fb: FormBuilder, private http: HttpClient, private l: LocalizationService) {}

  nextStep(): void {
    this.errorMessage = '';
    if (this.currentStep === 1 && this.form.invalid) {
      this.errorMessage = this.l.instant('Kyc.InvalidFormData');
      return;
    }
    this.currentStep = Math.min(3, this.currentStep + 1);
  }

  previousStep(): void {
    this.currentStep = Math.max(1, this.currentStep - 1);
  }

  uploadFile(event: Event, fileType: 'NationalIdFront' | 'NationalIdBack' | 'Selfie'): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) {
      return;
    }

    this.errorMessage = '';
    const formData = new FormData();
    formData.append('file', file);
    formData.append('fileType', fileType);

    this.http
      .post<KycUploadResponseDto>(`${this.apiBase}/app/kyc/upload`, formData)
      .subscribe({
        next: (result) => {
          if (fileType === 'NationalIdFront') {
            this.frontToken = result.fileToken;
          } else if (fileType === 'NationalIdBack') {
            this.backToken = result.fileToken;
          } else {
            this.selfieToken = result.fileToken;
          }
        },
        error: () => {
          this.errorMessage = this.l.instant('Kyc.FileUploadFailed');
        }
      });
  }

  submitKyc(): void {
    this.errorMessage = '';
    this.successMessage = '';
    if (!this.frontToken || !this.backToken || !this.selfieToken) {
      this.errorMessage = this.l.instant('Kyc.MissingImages');
      return;
    }

    this.isSubmitting = true;
    const payload = {
      fullNameArabic: this.form.controls.fullNameArabic.value ?? '',
      dateOfBirth: this.form.controls.dateOfBirth.value ?? '',
      nationalIdNumber: this.form.controls.nationalIdNumber.value ?? '',
      nationalIdFrontToken: this.frontToken,
      nationalIdBackToken: this.backToken,
      selfieToken: this.selfieToken
    };

    this.http
      .post<KycStatusDto>(`${this.apiBase}/app/kyc/submit`, payload)
      .subscribe({
        next: () => {
          this.successMessage = this.l.instant('Kyc.SubmitSuccess');
          this.isSubmitting = false;
        },
        error: () => {
          this.errorMessage = this.l.instant('Kyc.SubmitFailed');
          this.isSubmitting = false;
        }
      });
  }
}
