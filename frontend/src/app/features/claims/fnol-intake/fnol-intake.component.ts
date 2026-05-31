import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import {
  FormArray,
  FormBuilder,
  ReactiveFormsModule,
  Validators,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';
import { MatStepperModule } from '@angular/material/stepper';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { debounceTime, switchMap, of } from 'rxjs';
import { ClaimsService } from '../../../core/services/claims.service';
import { PolicyService } from '../../../core/services/policy.service';
import { ReferenceService } from '../../../core/services/reference.service';
import { CauseOfLossCode, PolicySearchResult } from '../../../core/models/reference.models';
import {
  CreateClaimRequest,
  PartyRole,
  PartyType,
  ReserveComponentType,
} from '../../../core/models/claim.models';

@Component({
  selector: 'app-fnol-intake',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatStepperModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatAutocompleteModule,
    MatSnackBarModule,
  ],
  templateUrl: './fnol-intake.component.html',
  styleUrl: './fnol-intake.component.scss',
})
export class FnolIntakeComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly claimsService = inject(ClaimsService);
  private readonly policyService = inject(PolicyService);
  private readonly referenceService = inject(ReferenceService);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);

  causeCodes: CauseOfLossCode[] = [];
  policyResults: PolicySearchResult[] = [];
  selectedPolicy: PolicySearchResult | null = null;

  step1Form = this.fb.group({
    policySearch: [''],
    policyId: [''],
    lossDate: [null as Date | null, Validators.required],
    causeOfLossCode: ['', Validators.required],
    lossDescription: ['', [Validators.required, Validators.minLength(10)]],
    lossLocation: [''],
    estimatedLossAmount: [null as number | null],
  });

  step2Form = this.fb.group({
    parties: this.fb.array([this.createPartyGroup(true)]),
  });

  step3Form = this.fb.group({
    riskObjects: this.fb.array([this.createRiskObjectGroup(true)]),
    includeReserve: [false],
    reserveComponent: ['Indemnity'],
    reserveAmount: [null as number | null],
    reserveReason: [''],
  });

  get parties(): FormArray {
    return this.step2Form.get('parties') as FormArray;
  }

  get riskObjects(): FormArray {
    return this.step3Form.get('riskObjects') as FormArray;
  }

  ngOnInit(): void {
    this.referenceService.getCauseOfLossCodes().subscribe((codes) => {
      this.causeCodes = codes;
    });

    this.step1Form
      .get('policySearch')!
      .valueChanges.pipe(
        debounceTime(300),
        switchMap((q) => (q && q.length >= 2 ? this.policyService.search(q) : of([])))
      )
      .subscribe((results) => {
        this.policyResults = results;
      });
  }

  createPartyGroup(required = false) {
    return this.fb.group({
      partyRole: ['Claimant', Validators.required],
      partyType: ['Person', Validators.required],
      firstName: [''],
      lastName: [''],
      companyName: [''],
      email: ['', Validators.email],
      phone: [''],
    }, { validators: required ? this.partyNameValidator : null });
  }

  createRiskObjectGroup(required = false) {
    return this.fb.group({
      assetType: ['', required ? Validators.required : []],
      assetDescription: ['', required ? Validators.required : []],
      damageDescription: [''],
      assetReference: [''],
      isPrimary: [required],
    });
  }

  partyNameValidator(group: AbstractControl): ValidationErrors | null {
    const type = group.get('partyType')?.value;
    if (type === 'Person') {
      const fn = group.get('firstName')?.value;
      const ln = group.get('lastName')?.value;
      if (!fn && !ln) return { nameRequired: true };
    } else if (type === 'Company' && !group.get('companyName')?.value) {
      return { companyRequired: true };
    }
    return null;
  }

  addParty(): void {
    this.parties.push(this.createPartyGroup());
  }

  removeParty(index: number): void {
    if (this.parties.length > 1) this.parties.removeAt(index);
  }

  addRiskObject(): void {
    this.riskObjects.push(this.createRiskObjectGroup());
  }

  removeRiskObject(index: number): void {
    if (this.riskObjects.length > 1) this.riskObjects.removeAt(index);
  }

  selectPolicy(policy: PolicySearchResult): void {
    this.selectedPolicy = policy;
    this.step1Form.patchValue({
      policyId: policy.policyId,
      policySearch: `${policy.policyNumber} — ${policy.clientName}`,
    });
    this.policyResults = [];
  }

  displayPolicy(p: PolicySearchResult): string {
    return p ? `${p.policyNumber} — ${p.clientName}` : '';
  }

  submit(): void {
    if (this.step1Form.invalid || this.step2Form.invalid || this.step3Form.invalid) {
      this.step1Form.markAllAsTouched();
      this.step2Form.markAllAsTouched();
      this.step3Form.markAllAsTouched();
      return;
    }

    const s1 = this.step1Form.value;
    const s2 = this.step2Form.value;
    const s3 = this.step3Form.value;
    const lossDate = s1.lossDate!;
    const request: CreateClaimRequest = {
      policyId: s1.policyId || undefined,
      lossDate: new Date(
        Date.UTC(lossDate.getFullYear(), lossDate.getMonth(), lossDate.getDate())
      ).toISOString(),
      lossDescription: s1.lossDescription!,
      causeOfLossCode: s1.causeOfLossCode!,
      lossLocation: s1.lossLocation || undefined,
      estimatedLossAmount: s1.estimatedLossAmount ?? undefined,
      parties: (s2.parties ?? []).map((p) => ({
        partyRole: p.partyRole! as PartyRole,
        partyType: p.partyType! as PartyType,
        firstName: p.firstName || undefined,
        lastName: p.lastName || undefined,
        companyName: p.companyName || undefined,
        email: p.email || undefined,
        phone: p.phone || undefined,
      })),
      riskObjects: (s3.riskObjects ?? []).map((r) => ({
        assetType: r.assetType!,
        assetDescription: r.assetDescription!,
        damageDescription: r.damageDescription || undefined,
        assetReference: r.assetReference || undefined,
        isPrimary: !!r.isPrimary,
      })),
    };

    if (s3.includeReserve && s3.reserveAmount && s3.reserveReason) {
      request.initialReserve = {
        component: s3.reserveComponent as ReserveComponentType,
        amount: s3.reserveAmount,
        changeReason: s3.reserveReason,
      };
    }

    this.claimsService.create(request).subscribe({
      next: (claim) => {
        this.snackBar.open('Claim created successfully', 'OK', { duration: 3000 });
        this.router.navigate(['/claims', claim.id]);
      },
    });
  }
}
