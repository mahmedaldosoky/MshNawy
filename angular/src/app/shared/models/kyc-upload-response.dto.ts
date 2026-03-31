export interface KycUploadResponseDto {
  fileToken: string;
  fileType: 'NationalIdFront' | 'NationalIdBack' | 'Selfie';
}
