import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { KycReviewDecision } from '../../shared/models/kyc-review-decision';
import { KycStatus } from '../../shared/models/kyc-status';

interface AdminKycSubmission {
  userId: string;
  phoneNumber: string;
  fullNameArabic?: string | null;
  status: KycStatus;
  submittedAt?: string | null;
  rejectionReason?: string | null;
  nationalIdFrontToken?: string | null;
  nationalIdBackToken?: string | null;
  selfieToken?: string | null;
}

@Component({
  selector: 'app-kyc-review',
  templateUrl: './kyc-review.component.html',
  styleUrls: ['./kyc-review.component.scss']
})
export class KycReviewComponent implements OnInit {
  private readonly apiBase = environment.apiUrl;

  status = 'Submitted';
  isLoading = false;
  errorMessage = '';
  items: AdminKycSubmission[] = [];

  statusOptions = ['Submitted', 'UnderReview', 'Rejected', 'NeedsResubmission', 'Approved'];

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.http
      .get<{ items: AdminKycSubmission[] }>(
        `${this.apiBase}/app/admin/kyc?status=${this.status}&skipCount=0&maxResultCount=50`
      )
      .subscribe({
        next: (result) => {
          this.items = result.items;
          this.isLoading = false;
        },
        error: () => {
          this.errorMessage = 'تعذر تحميل طلبات التحقق.';
          this.isLoading = false;
        }
      });
  }

  moveToUnderReview(userId: string): void {
    this.http.post(`${this.apiBase}/app/admin/kyc/${userId}/under-review`, {}).subscribe({
      next: () => this.load(),
      error: () => (this.errorMessage = 'تعذر تحديث الحالة.')
    });
  }

  approve(userId: string): void {
    this.review(userId, 'Approve');
  }

  reject(userId: string): void {
    const reason = window.prompt('سبب الرفض') ?? '';
    this.review(userId, 'Reject', reason);
  }

  requestResubmission(userId: string): void {
    const reason = window.prompt('سبب إعادة الإرسال') ?? '';
    this.review(userId, 'NeedsResubmission', reason);
  }

  private review(userId: string, decision: KycReviewDecision, reason?: string): void {
    this.http
      .post(`${this.apiBase}/app/admin/kyc/${userId}/review`, { decision, reason })
      .subscribe({
        next: () => this.load(),
        error: () => (this.errorMessage = 'تعذر إرسال قرار المراجعة.')
      });
  }
}
