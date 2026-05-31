import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './layout/header/header.component';
import { LoadingOverlayComponent } from './shared/components/loading-overlay/loading-overlay.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, LoadingOverlayComponent],
  template: `
    <app-header />
    <main>
      <router-outlet />
    </main>
    <app-loading-overlay />
  `,
  styles: [
    `
      main {
        min-height: calc(100vh - 64px);
      }
    `,
  ],
})
export class AppComponent {}
