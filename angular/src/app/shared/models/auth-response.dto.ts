import { KycStatus } from './kyc-status';

export interface AuthResponseDto {
  accessToken: string;
  userId: string;
  kycStatus: KycStatus;
}
