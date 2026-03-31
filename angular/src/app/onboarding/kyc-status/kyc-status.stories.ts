import { Meta, StoryObj } from '@storybook/angular';
import { KycStatusComponent } from './kyc-status.component';

const meta: Meta<KycStatusComponent> = {
  title: 'Onboarding/KYC Status',
  component: KycStatusComponent,
  tags: ['autodocs'],
  decorators: [
    (story) => ({
      template: `<div dir="rtl" style="padding: 2rem; background: #f2f0ff; min-height: 100vh;">${story.template}</div>`,
      props: story.props
    })
  ]
};

export default meta;

type Story = StoryObj<KycStatusComponent>;

export const Draft: Story = {
  args: {
    autoLoad: false,
    status: {
      status: 'Draft'
    }
  }
};

export const Submitted: Story = {
  args: {
    autoLoad: false,
    status: {
      status: 'Submitted',
      submittedAt: '2026-03-08T10:00:00Z'
    }
  }
};

export const Rejected: Story = {
  args: {
    autoLoad: false,
    status: {
      status: 'Rejected',
      rejectionReason: 'الصورة غير واضحة'
    }
  }
};
