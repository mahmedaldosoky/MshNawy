import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppShellComponent } from './app-shell.component';

@NgModule({
  declarations: [AppShellComponent],
  imports: [CommonModule],
  exports: [AppShellComponent]
})
export class AppShellModule {}
