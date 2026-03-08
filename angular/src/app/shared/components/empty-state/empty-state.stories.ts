import { Meta, StoryObj } from '@storybook/angular';
import { EmptyStateComponent } from './empty-state.component';

const meta: Meta<EmptyStateComponent> = {
  title: 'Shared/EmptyState',
  component: EmptyStateComponent,
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
type Story = StoryObj<EmptyStateComponent>;

export const Default: Story = {
  args: {
    title: 'لا توجد بيانات',
    description: 'لم يتم العثور على أي عناصر لعرضها',
    icon: 'inbox'
  }
};

export const WithAction: Story = {
  args: {
    title: 'لا توجد استثمارات',
    description: 'ابدأ رحلة استثمارك الآن',
    icon: 'trending_up',
    actionLabel: 'استكشف العروض',
    actionCallback: () => alert('استكشف العروض')
  }
};

export const NoData: Story = {
  args: {
    title: 'لا توجد معاملات',
    icon: 'receipt_long'
  }
};

export const Mobile: Story = {
  args: {
    title: 'لا توجد بيانات',
    description: 'لم يتم العثور على أي عناصر',
    actionLabel: 'أضف جديد'
  },
  parameters: {
    viewport: {
      defaultViewport: 'mobile1'
    }
  }
};

export const RTL: Story = {
  args: {
    title: 'لا توجد نتائج بحث',
    description: 'حاول تغيير معايير البحث',
    icon: 'search'
  },
  decorators: [
    (story) => ({
      template: `<div dir="rtl">${story.template}</div>`,
      props: story.props
    })
  ]
};
