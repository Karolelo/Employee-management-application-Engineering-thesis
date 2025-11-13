import { Component,Input } from '@angular/core';

@Component({
  selector: 'app-info-message',
  standalone: true,
  templateUrl: './info-message.component.html',
  styleUrl: './info-message.component.css'
})
export class InfoMessageComponent {
@Input() title?: string;
@Input() text?: string;

}
