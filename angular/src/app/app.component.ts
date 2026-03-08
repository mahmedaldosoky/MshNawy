import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  template: `<ri-app-shell><router-outlet></router-outlet><div class="container">مرحباً بكم في RealInvest (Mock)</div></ri-app-shell>`,
  styles: [`.container{padding:1rem}`]
})
export class AppComponent {}
