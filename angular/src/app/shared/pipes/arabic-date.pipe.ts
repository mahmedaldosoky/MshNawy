import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'arabicDate' })
export class ArabicDatePipe implements PipeTransform {
  transform(value: string | Date | null | undefined): string {
    if (!value) return '-'
    const d = typeof value === 'string' ? new Date(value) : value
    return new Intl.DateTimeFormat('ar-EG', { dateStyle: 'medium' }).format(d as Date)
  }
}
