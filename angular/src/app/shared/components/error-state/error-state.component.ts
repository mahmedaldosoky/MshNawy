import { Component, Input } from '@angular/core';

/**
 * Error state component - displays when an error occurs
 * Per Constitution I: All text in Arabic only
 */
@Component({
  selector: 'ri-error-state',
  templateUrl: './error-state.component.html',
  styleUrls: ['./error-state.component.css']
})
export class ErrorStateComponent {
  @Input() errorCode: string = 'RealInvest:0001'; // Default error code
  @Input() errorMessage: string = 'حدث خطأ غير متوقع'; // Arabic: An unexpected error occurred
  @Input() details?: string;
  @Input() retryLabel: string = 'إعادة المحاولة'; // Arabic: Retry
  @Input() retryCallback?: () => void;
  @Input() showRetry: boolean = true;

  onRetry() {
    if (this.retryCallback) {
      this.retryCallback();
    }
  }
}
