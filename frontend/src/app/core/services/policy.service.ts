import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';
import { PolicySearchResult } from '../models/reference.models';

@Injectable({ providedIn: 'root' })
export class PolicyService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/api/policies`;

  search(query: string): Observable<PolicySearchResult[]> {
    const params = new HttpParams().set('q', query);
    return this.http.get<PolicySearchResult[]>(`${this.baseUrl}/search`, { params });
  }
}
