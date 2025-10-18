import { Component, OnInit } from '@angular/core';
import {Target} from '../../interfaces/target';
import {UserStoreService} from '../../../login/services/user_data/user-store.service';
import {TargetService} from '../../services/target.service';

@Component({
  selector: 'app-target-list',
  standalone: false,
  templateUrl: './target-list.component.html',
  styleUrl: './target-list.component.css'
})
export class TargetListComponent implements OnInit {
  targets: Target[] = [];
  loading = true;

  constructor(
    private targetService: TargetService,
    private userStore: UserStoreService
  ) {
  }

  ngOnInit(): void {
    const userId = this.userStore.getUserId();
    if (!userId){
      this.loading = false;
      return;
    }

    this.targetService.getUserTargets(userId).subscribe({
      next: list => {
        this.targets = list;
        this.loading = false;
        },
      error: () => {
        this.targets = [];
        this.loading = false;
      }
    });
  }

  formatDate(iso: string | null | undefined): string {
    if (!iso) return '-';
    const d = new Date(iso);
    if (Number.isNaN(d.getTime())) return String(iso);
    const dd = String(d.getDate()).padStart(2, '0');
    const mm = String(d.getMonth() + 1).padStart(2, '0');
    const yyyy = d.getFullYear();
    return `${dd}.${mm}.${yyyy}`;
  }

  progressPercent(target: Target): number {
    const parse = (v?: string | null) => v ? new Date(v).getTime() : NaN;
    const start = parse(target.start_Time);
    const end   = isNaN(parse(target.finish_Time ?? null))
      ? (start + 30 * 86400000)     // fallback when no Finish_Time is set
      : parse(target.finish_Time!);

    if (!isFinite(start) || !isFinite(end) || end <= start) return 0;

    const now = Date.now();
    const ratio = (now - start) / (end - start);
    const clamped = Math.max(0, Math.min(1, ratio));
    return Math.round(clamped * 100);
  }
}
