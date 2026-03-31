import { Meta, StoryObj } from '@storybook/angular';
import { ErrorStateComponent } from './error-state.component';

const meta: Meta<ErrorStateComponent> = {
  title: 'Shared/ErrorState',
  component: ErrorStateComponent,
  tags: ['autodocs'],
  decorators: [
    (story) => ({
      template: `<div dir="rtl" style="padding: 2rem; background: #f5f5f5; min-height: 100vh;">
        ${story.template}
      </div>`,
      props: story.props
    })
  ]
};

export default meta;
type Story = StoryObj<ErrorStateComponent>;

export const Default: Story = {
  args: {
    errorCode: 'MshNawy:0001',
    errorMessage: 'حدث خطأ غير متوقع',
    showRetry: true
  }
};

export const InsufficientBalance: Story = {
  args: {
    errorCode: 'MshNawy:0300',
    errorMessage: 'رصيد غير كافي',
    details: 'الرصيد المتاح ليس كافياً لإكمال هذه العملية',
    showRetry: false
  }
};

export const KycNotApproved: Story = {
  args: {
    errorCode: 'MshNawy:0201',
    errorMessage: 'لم يتم التحقق من الهوية بعد',
    details: 'يجب أن تكمل التحقق من الهوية أولاً قبل الاستثمار',
    showRetry: false
  }
};

export const WithRetry: Story = {
  args: {
    errorCode: 'MshNawy:0101',
    errorMessage: 'فشل إرسال رمز التحقق',
    details: 'حاول مرة أخرى',
    retryLabel: 'إعادة المحاولة',
    showRetry: true,
    retryCallback: () => alert('إعادة المحاولة')
  }
};

export const Mobile: Story = {
  args: {
    errorCode: 'MshNawy:0002',
    errorMessage: 'خطأ في المدخلات',
    details: 'تحقق من البيانات المدخلة',
    showRetry: true
  },
  parameters: {
    viewport: {
      defaultViewport: 'mobile1'
    }
  }
};

export const RTL: Story = {
  args: {
    errorCode: 'MshNawy:0500',
    errorMessage: 'فشلت عملية السحب',
    details: 'يرجى التواصل مع الدعم الفني'
  }
};
