import {Component, OnInit, Output, EventEmitter, Input, OnChanges, SimpleChanges} from '@angular/core';
import {Target} from '../../interfaces/target';
import {UserStoreService} from '../../../login/services/user_data/user-store.service';
import {TargetService} from '../../services/target.service';

@Component({
  selector: 'app-target-list',
  standalone: false,
  templateUrl: './target-list.component.html',
  styleUrl: './target-list.component.css'
})
export class TargetListComponent implements OnInit, OnChanges {
  @Input() userId?: number;
  @Output() select = new EventEmitter<number>();
  targets: Target[] = [];
  loading = true;

  constructor(
    private targetService: TargetService,
    private userStore: UserStoreService
  ) {
  }

  ngOnInit(): void {
    this.load();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['userId'] && !changes['userId'].firstChange) this.load();
  }

  private load(): void {
    const uId = this.userId ?? this.userStore.getUserId();
    this.loading = true;
    if (!uId){
      this.targets = [];
      this.loading = false;
      return;
    }

    this.targetService.getUserTargets(uId).subscribe({
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

  onSelect(target: Target){
    this.select.emit(target.id);
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
