import { Component, Input } from '@angular/core';

/**
 * Empty state component - displays when a list or section has no content
 * Per Constitution I: All text in Arabic only
 */
@Component({
  selector: 'ri-empty-state',
  templateUrl: './empty-state.component.html',
  styleUrls: ['./empty-state.component.css']
})
export class EmptyStateComponent {
  @Input() icon: string = 'inbox'; // Material icon name
  @Input() title: string = 'لا توجد بيانات'; // Arabic: No data
  @Input() description: string = '';
  @Input() actionLabel?: string; // e.g., "إضافة جديد" (Add new)
  @Input() actionCallback?: () => void;

  onActionClick() {
    if (this.actionCallback) {
      this.actionCallback();
    }
  }
}
