import { rest } from 'msw';
import { seed } from '../data/seed';

const otpLength = 6;
const otpExpiresInSeconds = 180;
const mockJwt = 'mock-jwt-token';

let kycStatus: 'Draft' | 'Submitted' | 'UnderReview' | 'Approved' | 'Rejected' | 'NeedsResubmission' = 'Draft';
let kycRejectionReason: string | null = null;
let kycSubmittedAt: string | null = null;

const makeFileToken = (fileType: string) => `kyc-${fileType.toLowerCase()}-token`;

export const identityHandlers = [
  rest.post('/api/app/auth/send-otp', async (req, res, ctx) => {
    const body = (await req.json()) as { phoneNumber?: string };
    if (!body.phoneNumber || !body.phoneNumber.startsWith('+20')) {
      return res(
        ctx.status(400),
        ctx.json({ error: { code: 'MshNawy:0100', message: 'رقم الهاتف غير صالح' } })
      );
    }

    return res(
      ctx.status(200),
      ctx.json({ expiresInSeconds: otpExpiresInSeconds, attemptsRemaining: 4 })
    );
  }),

  rest.post('/api/app/auth/verify-otp', async (req, res, ctx) => {
    const body = (await req.json()) as { phoneNumber?: string; otpCode?: string };
    const otp = body.otpCode ?? '';
    const isValid = otp.length === otpLength && /^\d+$/.test(otp);
    if (!isValid) {
      return res(
        ctx.status(400),
        ctx.json({ error: { code: 'MshNawy:0102', message: 'رمز التحقق غير صحيح' } })
      );
    }

    const userId = seed.users[0]?.id ?? 'user-1';
    return res(
      ctx.status(200),
      ctx.json({
        accessToken: mockJwt,
        userId,
        kycStatus
      })
    );
  }),

  rest.get('/api/app/kyc/status', (req, res, ctx) => {
    return res(
      ctx.status(200),
      ctx.json({
        status: kycStatus,
        rejectionReason: kycRejectionReason,
        submittedAt: kycSubmittedAt
      })
    );
  }),

  rest.post('/api/app/kyc/upload', async (req, res, ctx) => {
    const formData = await req.formData();
    const fileType = String(formData.get('fileType') ?? 'Unknown');
    if (!fileType || fileType === 'Unknown') {
      return res(
        ctx.status(400),
        ctx.json({ error: { code: 'MshNawy:0207', message: 'نوع الملف غير صالح' } })
      );
    }

    return res(
      ctx.status(200),
      ctx.json({ fileToken: makeFileToken(fileType), fileType })
    );
  }),

  rest.post('/api/app/kyc/submit', async (req, res, ctx) => {
    const body = (await req.json()) as {
      fullNameArabic?: string;
      nationalIdNumber?: string;
      nationalIdFrontToken?: string;
      nationalIdBackToken?: string;
      selfieToken?: string;
    };

    if (!body.fullNameArabic || !body.nationalIdNumber) {
      return res(
        ctx.status(400),
        ctx.json({ error: { code: 'MshNawy:0205', message: 'بيانات الهوية غير مكتملة' } })
      );
    }

    if (!body.nationalIdFrontToken || !body.nationalIdBackToken || !body.selfieToken) {
      return res(
        ctx.status(400),
        ctx.json({ error: { code: 'MshNawy:0206', message: 'ملفات الهوية غير مكتملة' } })
      );
    }

    kycStatus = 'Submitted';
    kycRejectionReason = null;
    kycSubmittedAt = new Date().toISOString();

    return res(
      ctx.status(200),
      ctx.json({ status: kycStatus, submittedAt: kycSubmittedAt })
    );
  }),

  rest.get('/api/app/kyc/image/:token', (req, res, ctx) => {
    return res(ctx.status(200), ctx.set('Content-Type', 'image/png'), ctx.body(''));
  })
];
