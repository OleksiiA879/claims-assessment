import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';
import {
  AuditLog,
  ClaimDetail,
  ClaimSummary,
  ClaimsListParams,
  CreateClaimPartyRequest,
  CreateClaimRequest,
  CreateReserveRequest,
  PaginatedList,
  RejectReserveRequest,
  TransitionStatusRequest,
} from '../models/claim.models';

@Injectable({ providedIn: 'root' })
export class ClaimsService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/api/claims`;

  list(params: ClaimsListParams = {}): Observable<PaginatedList<ClaimSummary>> {
    let httpParams = new HttpParams();
    if (params.status) httpParams = httpParams.set('status', params.status);
    if (params.dateFrom) httpParams = httpParams.set('dateFrom', params.dateFrom);
    if (params.dateTo) httpParams = httpParams.set('dateTo', params.dateTo);
    if (params.causeOfLossCode)
      httpParams = httpParams.set('causeOfLossCode', params.causeOfLossCode);
    if (params.search) httpParams = httpParams.set('search', params.search);
    httpParams = httpParams.set('pageNumber', String(params.pageNumber ?? 1));
    httpParams = httpParams.set('pageSize', String(params.pageSize ?? 20));
    return this.http.get<PaginatedList<ClaimSummary>>(this.baseUrl, { params: httpParams });
  }

  getById(id: string): Observable<ClaimDetail> {
    return this.http.get<ClaimDetail>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateClaimRequest): Observable<ClaimDetail> {
    return this.http.post<ClaimDetail>(this.baseUrl, request);
  }

  transitionStatus(id: string, request: TransitionStatusRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}/status`, request);
  }

  getAudit(
    id: string,
    pageNumber = 1,
    pageSize = 50
  ): Observable<PaginatedList<AuditLog>> {
    const params = new HttpParams()
      .set('pageNumber', String(pageNumber))
      .set('pageSize', String(pageSize));
    return this.http.get<PaginatedList<AuditLog>>(`${this.baseUrl}/${id}/audit`, { params });
  }

  createReserve(claimId: string, request: CreateReserveRequest): Observable<unknown> {
    return this.http.post(`${this.baseUrl}/${claimId}/reserves`, request);
  }

  approveReserve(claimId: string, reserveHistoryId: string): Observable<void> {
    return this.http.post<void>(
      `${this.baseUrl}/${claimId}/reserves/${reserveHistoryId}/approve`,
      {}
    );
  }

  rejectReserve(
    claimId: string,
    reserveHistoryId: string,
    request: RejectReserveRequest
  ): Observable<void> {
    return this.http.post<void>(
      `${this.baseUrl}/${claimId}/reserves/${reserveHistoryId}/reject`,
      request
    );
  }

  retractReserve(claimId: string, reserveHistoryId: string, reason: string): Observable<void> {
    return this.http.post<void>(
      `${this.baseUrl}/${claimId}/reserves/${reserveHistoryId}/retract`,
      { reason }
    );
  }

  setManagerReserveOverride(claimId: string, enabled: boolean, reason: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${claimId}/reserves/manager-override`, {
      enabled,
      reason,
    });
  }

  addParty(claimId: string, request: CreateClaimPartyRequest): Observable<unknown> {
    return this.http.post(`${this.baseUrl}/${claimId}/parties`, request);
  }

  deleteParty(claimId: string, partyId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${claimId}/parties/${partyId}`);
  }

  uploadDocument(claimId: string, documentType: string, file: File): Observable<unknown> {
    const body = new FormData();
    body.append('documentType', documentType);
    body.append('file', file, file.name);
    return this.http.post(`${this.baseUrl}/${claimId}/documents`, body);
  }

  downloadDocument(claimId: string, documentId: string): Observable<Blob> {
    return this.http.get(
      `${this.baseUrl}/${claimId}/documents/${documentId}/download`,
      { responseType: 'blob' }
    );
  }
}
