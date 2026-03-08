import { NgModule, LOCALE_ID } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { registerLocaleData } from '@angular/common';
import localeArEg from '@angular/common/locales/ar-EG';
import { AppComponent } from './app.component';
import { AppShellModule } from './shared/components/app-shell/app-shell.module';

// Register Arabic (Egypt) locale for DatePipe, CurrencyPipe, DecimalPipe
// Per Constitution I: All numbers, dates, and currency values MUST render in ar-EG locale
registerLocaleData(localeArEg);

@NgModule({
  declarations: [AppComponent],
  imports: [BrowserModule, AppShellModule],
  providers: [
    { provide: LOCALE_ID, useValue: 'ar-EG' }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
