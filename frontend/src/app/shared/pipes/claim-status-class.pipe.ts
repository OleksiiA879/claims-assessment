import { Pipe, PipeTransform } from '@angular/core';
import { ClaimStatus } from '../../core/models/claim.models';

@Pipe({ name: 'claimStatusClass', standalone: true })
export class ClaimStatusClassPipe implements PipeTransform {
  transform(status: ClaimStatus | string): string {
    const normalized = String(status).toLowerCase().replace(/[^a-z]/g, '');
    return `status-badge status-${normalized}`;
  }
}
