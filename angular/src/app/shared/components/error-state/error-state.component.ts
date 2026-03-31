import { Component, Input, OnInit } from '@angular/core';
import { LocalizationService } from '../../services/localization.service';

/**
 * Error state component - displays when an error occurs
 * Per Constitution I: All text in Arabic only
 */
@Component({
  selector: 'msn-error-state',
  templateUrl: './error-state.component.html',
  styleUrls: ['./error-state.component.css']
})
export class ErrorStateComponent implements OnInit {
  @Input() errorCode: string = 'MshNawy:0001';
  @Input() errorMessage?: string;
  @Input() details?: string;
  @Input() retryLabel?: string;
  @Input() retryCallback?: () => void;
  @Input() showRetry: boolean = true;

  constructor(private l: LocalizationService) {}

  ngOnInit(): void {
    if (!this.errorMessage) {
      this.errorMessage = this.l.instant('Common.UnexpectedError');
    }
    if (!this.retryLabel) {
      this.retryLabel = this.l.instant('Common.Retry');
    }
  }

  onRetry() {
    if (this.retryCallback) {
      this.retryCallback();
    }
  }
}
