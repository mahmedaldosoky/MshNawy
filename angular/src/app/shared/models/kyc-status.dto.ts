import { KycStatus } from './kyc-status';

export interface KycStatusDto {
  status: KycStatus;
  rejectionReason?: string | null;
  submittedAt?: string | null;
}
