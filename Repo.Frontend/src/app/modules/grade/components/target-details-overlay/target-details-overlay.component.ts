import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TargetService } from '../../services/target.service';
import { Target } from '../../interfaces/target';
import {UserStoreService} from '../../../login/services/user_data/user-store.service';

@Component({
  selector: 'app-target-details-overlay',
  standalone: false,
  templateUrl: './target-details-overlay.component.html',
  styleUrl: './target-details-overlay.component.css'
})
export class TargetDetailsOverlayComponent implements OnInit {
  @Input({required:true}) targetId!: number;
  @Output() close = new EventEmitter<boolean>();

  loading = true;
  error = '';
  target?: Target;

  isLeaderOrAdmin = false;
  editOpen = false;
  confirmOpen = false;
  submitting = false;

  constructor(private targetService: TargetService,
              private userStore: UserStoreService) { }

  ngOnInit(): void {
    this.isLeaderOrAdmin = this.userStore.hasRole('TeamLeader') || this.userStore.hasRole('Admin');
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
    this.close.emit(false);
  }

  openEdit(){ this.editOpen = true; }
  onEditClosed(changed: boolean){ this.editOpen=false; if(changed && this.target){ this.targetService.getTargetById(this.target.id).subscribe(t=>this.target=t); this.close.emit(true); } }
  askDelete(){ this.confirmOpen=true; }
  cancelDelete(){ this.confirmOpen=false; }
  confirmDelete(){
    if(!this.target) return;
    this.submitting=true;
    this.targetService.deleteTarget(this.target.id).subscribe({
      next: ()=> { this.submitting=false; this.confirmOpen=false; this.close.emit(true); },
      error:()=> { this.submitting=false; this.confirmOpen=false; }
    });
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
