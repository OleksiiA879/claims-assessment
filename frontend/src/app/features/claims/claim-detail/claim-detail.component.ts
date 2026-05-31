import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { ClaimsService } from '../../../core/services/claims.service';
import { AuthService } from '../../../core/services/auth.service';
import { ReferenceService } from '../../../core/services/reference.service';
import {
  AuditLog,
  ClaimDetail,
  ClaimStatus,
  ReserveTransaction,
} from '../../../core/models/claim.models';
import { ClaimStatusReference } from '../../../core/models/reference.models';
import { ClaimStatusClassPipe } from '../../../shared/pipes/claim-status-class.pipe';

@Component({
  selector: 'app-claim-detail',
  standalone: true,
  imports: [
    RouterLink,
    FormsModule,
    ReactiveFormsModule,
    DatePipe,
    CurrencyPipe,
    MatTabsModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatSnackBarModule,
    MatIconModule,
    MatPaginatorModule,
    ClaimStatusClassPipe,
  ],
  templateUrl: './claim-detail.component.html',
  styleUrl: './claim-detail.component.scss',
})
export class ClaimDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly claimsService = inject(ClaimsService);
  private readonly referenceService = inject(ReferenceService);
  readonly auth = inject(AuthService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly fb = inject(FormBuilder);

  claim = signal<ClaimDetail | null>(null);
  auditLogs = signal<AuditLog[]>([]);
  auditTotal = 0;
  auditPageIndex = 0;
  auditPageSize = 20;
  statusRefs: ClaimStatusReference[] = [];
  allowedTransitions: string[] = [];

  partyColumns = ['displayName', 'partyRole', 'partyType', 'email', 'phone'];
  documentColumns = ['documentName', 'documentType', 'uploadedAt', 'fileSizeBytes'];
  auditColumns = ['createdAt', 'eventType', 'description', 'oldValue', 'newValue'];

  statusForm = this.fb.group({
    targetStatus: ['', Validators.required],
    reason: [''],
  });

  reserveForm = this.fb.group({
    component: ['Indemnity', Validators.required],
    amount: [null as number | null, [Validators.required, Validators.min(0.01)]],
    changeReason: ['', Validators.required],
  });

  rejectReason = '';

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.loadClaim(id);
    this.loadAudit(id);
    this.referenceService.getClaimStatuses().subscribe((refs) => {
      this.statusRefs = refs;
      this.updateAllowedTransitions();
    });
  }

  loadClaim(id: string): void {
    this.claimsService.getById(id).subscribe((c) => {
      this.claim.set(c);
      this.updateAllowedTransitions();
    });
  }

  loadAudit(id: string): void {
    this.claimsService
      .getAudit(id, this.auditPageIndex + 1, this.auditPageSize)
      .subscribe((result) => {
        this.auditLogs.set(result.items);
        this.auditTotal = result.totalCount;
      });
  }

  onAuditPage(event: PageEvent): void {
    this.auditPageIndex = event.pageIndex;
    this.auditPageSize = event.pageSize;
    const id = this.route.snapshot.paramMap.get('id')!;
    this.loadAudit(id);
  }

  transitionStatus(): void {
    const claim = this.claim();
    if (!claim || this.statusForm.invalid) return;
    const v = this.statusForm.value;
    this.claimsService
      .transitionStatus(claim.id, {
        targetStatus: v.targetStatus as ClaimStatus,
        reason: v.reason || undefined,
      })
      .subscribe({
        next: () => {
          this.snackBar.open('Status updated', 'OK', { duration: 3000 });
          this.loadClaim(claim.id);
        },
      });
  }

  submitReserve(): void {
    const claim = this.claim();
    if (!claim || this.reserveForm.invalid) return;
    const v = this.reserveForm.value;
    this.claimsService
      .createReserve(claim.id, {
        component: v.component as 'Indemnity',
        amount: v.amount!,
        changeReason: v.changeReason!,
      })
      .subscribe({
        next: () => {
          this.snackBar.open('Reserve submitted', 'OK', { duration: 3000 });
          this.reserveForm.reset({ component: 'Indemnity' });
          this.loadClaim(claim.id);
        },
      });
  }

  approveReserve(tx: ReserveTransaction): void {
    const claim = this.claim();
    if (!claim) return;
    this.claimsService.approveReserve(claim.id, tx.id).subscribe({
      next: () => {
        this.snackBar.open('Reserve approved', 'OK', { duration: 3000 });
        this.loadClaim(claim.id);
      },
    });
  }

  rejectReserve(tx: ReserveTransaction): void {
    const claim = this.claim();
    if (!claim || !this.rejectReason.trim()) {
      this.snackBar.open('Enter a rejection reason', 'OK', { duration: 3000 });
      return;
    }
    this.claimsService
      .rejectReserve(claim.id, tx.id, { rejectionReason: this.rejectReason })
      .subscribe({
        next: () => {
          this.snackBar.open('Reserve rejected', 'OK', { duration: 3000 });
          this.rejectReason = '';
          this.loadClaim(claim.id);
        },
      });
  }

  pendingTransactions(): ReserveTransaction[] {
    const claim = this.claim();
    if (!claim) return [];
    return claim.reserves.flatMap((r) =>
      r.transactions.filter((t) => t.approvalStatus === 'PendingApproval')
    );
  }

  formatBytes(bytes: number): string {
    if (bytes < 1024) return `${bytes} B`;
    return `${(bytes / 1024).toFixed(1)} KB`;
  }

  private updateAllowedTransitions(): void {
    const claim = this.claim();
    if (!claim) return;
    const ref = this.statusRefs.find((r) => r.status === claim.status);
    this.allowedTransitions = ref?.allowedTransitions ?? [];
    this.statusForm.patchValue({ targetStatus: '' });
  }
}
