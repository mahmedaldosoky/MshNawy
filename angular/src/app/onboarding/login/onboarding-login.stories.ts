import { Meta, StoryObj, moduleMetadata } from '@storybook/angular';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { OnboardingLoginComponent } from './onboarding-login.component';

const meta: Meta<OnboardingLoginComponent> = {
  title: 'Onboarding/Login',
  component: OnboardingLoginComponent,
  tags: ['autodocs'],
  decorators: [
    moduleMetadata({
      imports: [ReactiveFormsModule, HttpClientModule]
    }),
    (story) => ({
      template: `<div dir="rtl" style="padding: 2rem; background: #f7f1ec; min-height: 100vh;">${story.template}</div>`,
      props: story.props
    })
  ]
};

export default meta;

type Story = StoryObj<OnboardingLoginComponent>;

export const Default: Story = {};

export const ErrorState: Story = {
  args: {
    errorMessage: 'رمز التحقق غير صحيح أو منتهي الصلاحية.'
  }
};

export const CountdownActive: Story = {
  args: {
    countdownSeconds: 120
  }
};

export const Verifying: Story = {
  args: {
    isVerifying: true
  }
};

export const OtpLocked: Story = {
  args: {
    errorMessage: 'تم تجاوز عدد المحاولات المسموح به.'
  }
};
