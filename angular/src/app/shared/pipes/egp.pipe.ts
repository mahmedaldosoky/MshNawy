import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'egp' })
export class EgpPipe implements PipeTransform {
  transform(value: number | null | undefined): string {
    if (value == null) return '-'
    return new Intl.NumberFormat('ar-EG', { style: 'currency', currency: 'EGP' }).format(value)
  }
}
