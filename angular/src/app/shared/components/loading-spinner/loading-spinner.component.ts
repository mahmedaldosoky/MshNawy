import { Component } from '@angular/core';

@Component({
  selector: 'ri-loading-spinner',
  template: `<div class="ri-spinner">Loading...</div>`,
  styles: [`.ri-spinner{display:flex;align-items:center;justify-content:center;padding:1rem}`]
})
export class LoadingSpinnerComponent {}
