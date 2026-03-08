import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

/**
 * Confirmation dialog component
 * Per Constitution I: All text in Arabic only
 */

export interface ConfirmationDialogData {
  title: string; // Arabic title
  message: string; // Arabic message
  confirmLabel?: string; // Default: "تأكيد" (Confirm)
  cancelLabel?: string; // Default: "إلغاء" (Cancel)
  confirmColor?: 'primary' | 'accent' | 'warn'; // Default: 'primary'
  isDangerous?: boolean; // If true, show warn color and require extra confirmation
}

@Component({
  selector: 'ri-confirmation-dialog',
  templateUrl: './confirmation-dialog.component.html',
  styleUrls: ['./confirmation-dialog.component.css']
})
export class ConfirmationDialogComponent {
  title: string;
  message: string;
  confirmLabel: string;
  cancelLabel: string;
  confirmColor: 'primary' | 'accent' | 'warn';
  isDangerous: boolean;

  constructor(
    public dialogRef: MatDialogRef<ConfirmationDialogComponent>,
    @Inject(MAT_DIALOG_DATA) data: ConfirmationDialogData
  ) {
    this.title = data.title;
    this.message = data.message;
    this.confirmLabel = data.confirmLabel || 'تأكيد';
    this.cancelLabel = data.cancelLabel || 'إلغاء';
    this.confirmColor = data.confirmColor || 'primary';
    this.isDangerous = data.isDangerous || false;
  }

  onConfirm() {
    this.dialogRef.close(true);
  }

  onCancel() {
    this.dialogRef.close(false);
  }
}
