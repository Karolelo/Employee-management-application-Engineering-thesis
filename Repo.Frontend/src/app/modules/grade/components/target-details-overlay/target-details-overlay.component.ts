import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TargetService } from '../../services/target.service';
import { Target } from '../../interfaces/target';

@Component({
  selector: 'app-target-details-overlay',
  standalone: false,
  templateUrl: './target-details-overlay.component.html',
  styleUrl: './target-details-overlay.component.css'
})
export class TargetDetailsOverlayComponent implements OnInit {
  @Input({required:true}) targetId!: number;
  @Output() close = new EventEmitter<void>();

  loading = true;
  error = '';
  target?: Target;

  constructor(private targetService: TargetService) { }

  ngOnInit(): void {
    this.loading = true;
    this.targetService.getTargetById(this.targetId).subscribe({
      next: target => {
        this.target = target;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load target details';
        this.loading = false;
      }
    });
  }

  dismiss(): void {
    this.close.emit();
  }

  formatDateTime(iso: string | null | undefined): string {
    if (!iso) return '-';
    const d = new Date(iso);
    if (Number.isNaN(d.getTime())) return String(iso);
    const dd = String(d.getDate()).padStart(2, '0');
    const mm = String(d.getMonth() + 1).padStart(2, '0');
    const yyyy = d.getFullYear();
    const hh = String(d.getHours()).padStart(2, '0');
    const mi = String(d.getMinutes()).padStart(2, '0');
    return `${dd}.${mm}.${yyyy} ${hh}:${mi}`;
  }
}
