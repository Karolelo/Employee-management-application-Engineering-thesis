import { Component,PipeTransform,Pipe } from '@angular/core';
import {AuthService} from '../../modules/login/services/auth_managment/auth.service';
import {CommonModule } from '@angular/common'
import {DashboardModule} from '../../modules/dashboard/dashboard.module';

@Component({
  selector: 'app-debug',
  template: `
    <div style="padding: 20px; border: 2px solid red;">
      <h2>🔧 Debug Refresh Token</h2>

      <button (click)="testRefreshToken()" class="btn btn-primary">
        Test Refresh Token
      </button>

      <button (click)="testGetToken()" class="btn btn-secondary ms-2">
        Test Get Token
      </button>

      <hr>

      <h4>Response:</h4>
      <pre>{{ response | json }}</pre>

      <h4>Error:</h4>
      <pre style="color: red;">{{ error }}</pre>

      <h4>LocalStorage:</h4>
      <pre>{{ storageInfo | json }}</pre>
      <h3>Bez display: block (źle)</h3>
      <div class="container">
        Tekst po canvasie
        <canvas class="bez-block" style="background: lightblue;"></canvas>
      </div>

      <h3>Z display: block (dobrze)</h3>
      <div class="container">
        Tekst po canvasie
        <canvas class="z-block" style="background: lightblue;"></canvas>
      </div>
    </div>
  `,
  styles: [`
    pre {
      background: #f4f4f4;
      padding: 10px;
      border-radius: 5px;
      overflow-x: auto;
    }
    canvas.bez-block {
            /* display: inline (domyślnie) */
            width: 200px;
            height: 200px;
          }

    canvas.z-block {
      display: block;
      width: 200px;
      height: 200px;
    }
  `],
  imports: [CommonModule, DashboardModule]
})
export class DebugComponent {
  response: any = null;
  error: string = '';
  storageInfo: any = {};

  constructor(private authService: AuthService) {}

  testRefreshToken() {
    console.log('🔄 Starting refresh token...');
    this.response = null;
    this.error = '';

    this.authService.refreshToken().subscribe({
      next: (res) => {
        console.log('✅ Success:', res);
        this.response = res;
        this.showStorageInfo();
      },
      error: (err) => {
        console.error('❌ Error:', err);
        this.error = err.message || JSON.stringify(err);
        this.showStorageInfo();
      }
    });
  }

  testGetToken() {
    const token = this.authService.getToken();
    const refreshToken = this.authService.getRefreshToken();

    console.log('Token:', token);
    console.log('Refresh Token:', refreshToken);

    this.response = { token, refreshToken };
  }

  showStorageInfo() {
    this.storageInfo = {
      accessToken: localStorage.getItem('accessToken'),
      refreshToken: localStorage.getItem('refreshToken'),
      // dostosuj klucze do swoich
    };
  }
}
