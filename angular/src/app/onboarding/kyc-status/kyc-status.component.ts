import { Component, Input, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { KycStatusDto } from '../../shared/models/kyc-status.dto';
import { LocalizationService } from '../../shared/services/localization.service';

@Component({
  selector: 'app-kyc-status',
  templateUrl: './kyc-status.component.html',
  styleUrls: ['./kyc-status.component.scss']
})
export class KycStatusComponent implements OnInit {
  private readonly apiBase = environment.apiUrl;

  @Input() status?: KycStatusDto;
  @Input() autoLoad = true;
  errorMessage = '';

  constructor(private http: HttpClient, private l: LocalizationService) {}

  ngOnInit(): void {
    if (!this.autoLoad || this.status) {
      return;
    }

    this.http.get<KycStatusDto>(`${this.apiBase}/app/kyc/status`).subscribe({
      next: (result) => {
        this.status = result;
      },
      error: () => {
        this.errorMessage = this.l.instant('Kyc.StatusLoadFailed');
      }
    });
  }
}
