import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';
import { CauseOfLossCode, ClaimStatusReference } from '../models/reference.models';

@Injectable({ providedIn: 'root' })
export class ReferenceService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/api/reference`;

  getCauseOfLossCodes(perilCategory?: string): Observable<CauseOfLossCode[]> {
    let params = new HttpParams();
    if (perilCategory) params = params.set('perilCategory', perilCategory);
    return this.http.get<CauseOfLossCode[]>(`${this.baseUrl}/cause-of-loss-codes`, { params });
  }

  getClaimStatuses(): Observable<ClaimStatusReference[]> {
    return this.http.get<ClaimStatusReference[]>(`${this.baseUrl}/claim-statuses`);
  }
}
