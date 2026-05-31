import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { MockUser } from '../../core/models/auth.models';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    MatToolbarModule,
    MatButtonModule,
    MatSelectModule,
    MatFormFieldModule,
    FormsModule,
    RouterLink,
    RouterLinkActive,
  ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  readonly auth = inject(AuthService);

  selectedUsername =
    this.auth.mockUsers.find((u) => u.role === this.auth.role())?.username ??
    this.auth.mockUsers[0].username;

  onRoleChange(username: string): void {
    const user = this.auth.mockUsers.find((u) => u.username === username);
    if (user) {
      this.auth.switchUser(user).subscribe();
    }
  }

  trackUser(_index: number, user: MockUser): string {
    return user.username;
  }
}
