import { Component, OnInit, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { ClaimsService } from '../../../core/services/claims.service';
import { ReferenceService } from '../../../core/services/reference.service';
import { ClaimStatus, ClaimSummary, PaginatedList } from '../../../core/models/claim.models';
import { CauseOfLossCode } from '../../../core/models/reference.models';
import { ClaimStatusClassPipe } from '../../../shared/pipes/claim-status-class.pipe';

@Component({
  selector: 'app-claims-list',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    DatePipe,
    CurrencyPipe,
    MatTableModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatIconModule,
    ClaimStatusClassPipe,
  ],
  templateUrl: './claims-list.component.html',
  styleUrl: './claims-list.component.scss',
})
export class ClaimsListComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly claimsService = inject(ClaimsService);
  private readonly referenceService = inject(ReferenceService);
  private readonly router = inject(Router);

  readonly displayedColumns = [
    'claimNumber',
    'policyNumber',
    'clientName',
    'lossDate',
    'causeOfLossCode',
    'status',
    'totalReserves',
    'actions',
  ];

  filterForm = this.fb.group({
    status: ['' as ClaimStatus | ''],
    dateFrom: [null as Date | null],
    dateTo: [null as Date | null],
    causeOfLossCode: [''],
  });

  data: ClaimSummary[] = [];
  totalCount = 0;
  pageSize = 20;
  pageIndex = 0;
  statuses: ClaimStatus[] = [];
  causeCodes: CauseOfLossCode[] = [];

  ngOnInit(): void {
    this.referenceService.getClaimStatuses().subscribe((refs) => {
      this.statuses = refs.map((r) => r.status);
    });
    this.referenceService.getCauseOfLossCodes().subscribe((codes) => {
      this.causeCodes = codes;
    });
    this.loadClaims();
  }

  applyFilters(): void {
    this.pageIndex = 0;
    this.loadClaims();
  }

  clearFilters(): void {
    this.filterForm.reset({
      status: '',
      dateFrom: null,
      dateTo: null,
      causeOfLossCode: '',
    });
    this.pageIndex = 0;
    this.loadClaims();
  }

  onPage(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadClaims();
  }

  viewClaim(id: string): void {
    this.router.navigate(['/claims', id]);
  }

  private loadClaims(): void {
    const v = this.filterForm.value;
    this.claimsService
      .list({
        status: v.status || undefined,
        dateFrom: v.dateFrom ? this.toIsoDate(v.dateFrom) : undefined,
        dateTo: v.dateTo ? this.toIsoDateEnd(v.dateTo) : undefined,
        causeOfLossCode: v.causeOfLossCode || undefined,
        pageNumber: this.pageIndex + 1,
        pageSize: this.pageSize,
      })
      .subscribe((result: PaginatedList<ClaimSummary>) => {
        this.data = result.items;
        this.totalCount = result.totalCount;
      });
  }

  private toIsoDate(d: Date): string {
    return new Date(Date.UTC(d.getFullYear(), d.getMonth(), d.getDate())).toISOString();
  }

  private toIsoDateEnd(d: Date): string {
    const end = new Date(Date.UTC(d.getFullYear(), d.getMonth(), d.getDate(), 23, 59, 59));
    return end.toISOString();
  }
}
