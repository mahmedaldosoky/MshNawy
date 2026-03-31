import { Injectable } from '@angular/core';

/**
 * Localization service — centralizes all Arabic UI strings.
 * Per Constitution I: No hardcoded Arabic strings in components.
 * Keys mirror the backend ar.json localization resource for consistency.
 */
@Injectable({ providedIn: 'root' })
export class LocalizationService {
  private readonly texts: Record<string, string> = {
    'Onboarding.InvalidPhone': 'يرجى إدخال رقم هاتف صحيح',
    'Onboarding.OtpSendFailed': 'تعذر إرسال رمز التحقق. حاول مرة أخرى.',
    'Onboarding.InvalidOtp': 'يرجى إدخال رمز تحقق صحيح',
    'Onboarding.OtpVerifyFailed': 'رمز التحقق غير صحيح أو منتهي الصلاحية.',
    'Kyc.InvalidFormData': 'يرجى إدخال جميع البيانات المطلوبة بشكل صحيح.',
    'Kyc.FileUploadFailed': 'تعذر رفع الملف. حاول مرة أخرى.',
    'Kyc.MissingImages': 'يرجى رفع جميع الصور المطلوبة قبل الإرسال.',
    'Kyc.SubmitSuccess': 'تم إرسال طلب التحقق بنجاح.',
    'Kyc.SubmitFailed': 'تعذر إرسال طلب التحقق.',
    'Kyc.StatusLoadFailed': 'تعذر تحميل حالة التحقق.',
    'Common.UnexpectedError': 'حدث خطأ غير متوقع',
    'Common.Retry': 'إعادة المحاولة',
    'AppName': 'مش ناوي',
  };

  instant(key: string): string {
    return this.texts[key] ?? key;
  }
}
