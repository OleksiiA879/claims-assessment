export type ClaimStatus =
  | 'Draft'
  | 'Open'
  | 'UnderInvestigation'
  | 'PendingPayment'
  | 'Closed'
  | 'Reopened'
  | 'Withdrawn'
  | 'SlaBreached';

export type PartyRole = 'Claimant' | 'Insured' | 'ThirdParty' | 'Witness' | 'Attorney';
export type PartyType = 'Person' | 'Company';
export type ReserveComponentType = 'Indemnity' | 'Expense' | 'ALAE' | 'SubrogationRecoverable';
export type ReserveApprovalStatus =
  | 'AutoApproved'
  | 'PendingApproval'
  | 'Approved'
  | 'Rejected'
  | 'Cancelled';
export type ReservePostingStatus = 'Pending' | 'Posted' | 'Failed' | 'Cancelled';
export type ReserveTransactionType = 'Add' | 'Adjust' | 'Reverse';
export type ValidationSeverity = 'Critical' | 'Warning';

export interface PaginatedList<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

export interface ClaimSummary {
  id: string;
  claimNumber: string;
  policyNumber?: string;
  clientName?: string;
  lossDate?: string;
  causeOfLossCode?: string;
  status: ClaimStatus;
  totalReserves: number;
}

export interface Party {
  id: string;
  partyRole: PartyRole;
  partyType: PartyType;
  displayName: string;
  email?: string;
  phone?: string;
  isActive: boolean;
}

export interface RiskObject {
  id: string;
  assetType: string;
  assetDescription: string;
  damageDescription?: string;
  isPrimary: boolean;
}

export interface ReserveTransaction {
  id: string;
  transactionType: ReserveTransactionType;
  amount: number;
  approvalStatus: ReserveApprovalStatus;
  postingStatus: ReservePostingStatus;
  changeReason: string;
  submittedByUserId?: string;
  approvedByUserId?: string;
  createdAt: string;
}

export interface ReserveSummary {
  componentId: string;
  component: ReserveComponentType;
  currentAmount: number;
  pendingAmount: number;
  transactions: ReserveTransaction[];
}

export interface Document {
  id: string;
  documentName: string;
  documentType: string;
  uploadedAt: string;
  fileSizeBytes: number;
  downloadUrl?: string;
}

export interface ValidationIssue {
  code: string;
  message: string;
  severity: ValidationSeverity;
  isAcknowledged: boolean;
}

export interface ClaimDetail {
  id: string;
  claimNumber: string;
  status: ClaimStatus;
  policyNumber?: string;
  clientName?: string;
  policyId?: string;
  lossDate?: string;
  lossDescription?: string;
  lossLocation?: string;
  causeOfLossCode?: string;
  assignedHandlerId?: string;
  managerOverrideForReserves: boolean;
  parties: Party[];
  riskObjects: RiskObject[];
  reserves: ReserveSummary[];
  documents: Document[];
  validationIssues: ValidationIssue[];
}

export interface AuditLog {
  id: string;
  eventType: string;
  description: string;
  createdAt: string;
  createdByUserId?: string;
  oldValue?: string;
  newValue?: string;
}

export interface ClaimsListParams {
  status?: ClaimStatus;
  dateFrom?: string;
  dateTo?: string;
  causeOfLossCode?: string;
  search?: string;
  pageNumber?: number;
  pageSize?: number;
}

export interface TransitionStatusRequest {
  targetStatus: ClaimStatus;
  reason?: string;
}

export interface CreateClaimPartyRequest {
  partyRole: PartyRole;
  partyType: PartyType;
  firstName?: string;
  lastName?: string;
  companyName?: string;
  email?: string;
  phone?: string;
}

export interface CreateRiskObjectRequest {
  assetType: string;
  assetDescription: string;
  damageDescription?: string;
  assetReference?: string;
  isPrimary: boolean;
}

export interface InitialReserveRequest {
  component: ReserveComponentType;
  amount: number;
  changeReason: string;
}

export interface CreateClaimRequest {
  policyId?: string;
  lossDate: string;
  lossDescription: string;
  causeOfLossCode: string;
  lossLocation?: string;
  estimatedLossAmount?: number;
  parties: CreateClaimPartyRequest[];
  riskObjects: CreateRiskObjectRequest[];
  initialReserve?: InitialReserveRequest;
}

export interface CreateReserveRequest {
  component: ReserveComponentType;
  amount: number;
  changeReason: string;
  transactionType?: ReserveTransactionType;
}

export interface RejectReserveRequest {
  rejectionReason: string;
}
