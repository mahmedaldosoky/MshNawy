import { Meta, StoryObj } from '@storybook/angular';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ConfirmationDialogComponent } from './confirmation-dialog.component';

const meta: Meta<ConfirmationDialogComponent> = {
  title: 'Shared/ConfirmationDialog',
  component: ConfirmationDialogComponent,
  tags: ['autodocs'],
  providers: [
    {
      provide: MatDialogRef,
      useValue: {
        close: (result: any) => console.log('Dialog closed with:', result)
      }
    }
  ]
};

export default meta;
type Story = StoryObj<ConfirmationDialogComponent>;

export const Default: Story = {
  args: {},
  providers: [
    {
      provide: MAT_DIALOG_DATA,
      useValue: {
        title: 'تأكيد الحذف',
        message: 'هل أنت متأكد من رغبتك في حذف هذا العنصر؟'
      }
    },
    {
      provide: MatDialogRef,
      useValue: {
        close: (result: any) => console.log('Dialog closed:', result)
      }
    }
  ]
};

export const DangerousAction: Story = {
  args: {},
  providers: [
    {
      provide: MAT_DIALOG_DATA,
      useValue: {
        title: 'تحذير',
        message: 'هل تريد حقاً إلغاء هذا الاستثمار؟ هذا الإجراء لا يمكن التراجع عنه.',
        confirmLabel: 'نعم، الغِ',
        cancelLabel: 'لا',
        confirmColor: 'warn',
        isDangerous: true
      }
    },
    {
      provide: MatDialogRef,
      useValue: {
        close: (result: any) => console.log('Dialog closed:', result)
      }
    }
  ]
};

export const ConfirmDeposit: Story = {
  args: {},
  providers: [
    {
      provide: MAT_DIALOG_DATA,
      useValue: {
        title: 'تأكيد الإيداع',
        message: 'هل تريد تأكيد إيداع 500 جنيه مصري؟',
        confirmLabel: 'نعم، أكد',
        cancelLabel: 'إلغاء'
      }
    },
    {
      provide: MatDialogRef,
      useValue: {
        close: (result: any) => console.log('Dialog closed:', result)
      }
    }
  ]
};

export const CustomLabels: Story = {
  args: {},
  providers: [
    {
      provide: MAT_DIALOG_DATA,
      useValue: {
        title: 'تأكيد العملية',
        message: 'يرجى التأكد من صحة البيانات المدخلة',
        confirmLabel: 'متابعة',
        cancelLabel: 'رجوع',
        confirmColor: 'primary'
      }
    },
    {
      provide: MatDialogRef,
      useValue: {
        close: (result: any) => console.log('Dialog closed:', result)
      }
    }
  ]
};
