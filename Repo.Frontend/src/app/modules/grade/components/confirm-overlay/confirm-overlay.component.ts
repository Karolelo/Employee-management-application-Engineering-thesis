import {Component, EventEmitter, Input, Output} from '@angular/core';

@Component({
  selector: 'app-confirm-overlay',
  standalone: false,
  templateUrl: './confirm-overlay.component.html',
  styleUrl: './confirm-overlay.component.css'
})
export class ConfirmOverlayComponent {
  @Input({ required: true }) title!: string;
  @Input() message = 'Are you sure?';
  @Output() cancel = new EventEmitter<void>();
  @Output() confirm = new EventEmitter<void>();
}
