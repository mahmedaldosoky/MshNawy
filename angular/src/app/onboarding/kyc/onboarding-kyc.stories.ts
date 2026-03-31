import { Meta, StoryObj, moduleMetadata } from '@storybook/angular';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { OnboardingKycComponent } from './onboarding-kyc.component';

const meta: Meta<OnboardingKycComponent> = {
  title: 'Onboarding/KYC',
  component: OnboardingKycComponent,
  tags: ['autodocs'],
  decorators: [
    moduleMetadata({
      imports: [ReactiveFormsModule, HttpClientModule]
    }),
    (story) => ({
      template: `<div dir="rtl" style="padding: 2rem; background: #eef6fb; min-height: 100vh;">${story.template}</div>`,
      props: story.props
    })
  ]
};

export default meta;

type Story = StoryObj<OnboardingKycComponent>;

export const StepOne: Story = {
  args: {
    currentStep: 1
  }
};

export const StepTwoWithUploads: Story = {
  args: {
    currentStep: 2,
    frontToken: 'token-front',
    backToken: 'token-back',
    selfieToken: 'token-selfie'
  }
};

export const StepThree: Story = {
  args: {
    currentStep: 3
  }
};

export const SubmissionError: Story = {
  args: {
    currentStep: 3,
    errorMessage: 'تعذر إرسال طلب التحقق.'
  }
};
