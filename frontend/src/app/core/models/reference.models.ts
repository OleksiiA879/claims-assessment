import { ClaimStatus } from './claim.models';

export interface CauseOfLossCode {
  code: string;
  name: string;
  perilCategory: string;
}

export interface ClaimStatusReference {
  status: ClaimStatus;
  allowedTransitions: string[];
}

export interface PolicySearchResult {
  policyId: string;
  policyNumber: string;
  clientName: string;
  effectiveDate: string;
  expirationDate: string;
  status: string;
  coverageTypes: string;
}
