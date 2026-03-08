// Storybook preview configuration placeholder for RTL support
export const parameters = {
  layout: 'fullscreen'
}

export const decorators = [
  (Story) => {
    document.documentElement.setAttribute('dir', 'rtl')
    return Story()
  }
]
