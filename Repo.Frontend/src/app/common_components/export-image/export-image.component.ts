import { Component,Output,EventEmitter } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
@Component({
  selector: 'app-export-image',
  imports: [MatIconModule],
  templateUrl: './export-image.component.html',
  styleUrl: './export-image.component.css'
})
export class ExportImageComponent {
  fileName?: string;
  file?: File;
  @Output() fileSent: EventEmitter<File> = new EventEmitter<File>()
  onFileChange(event: any) {
    const files = event.target.files;
    if (files && files.length > 0) {
      this.file = files[0];
      this.fileName = this.file?.name;
      this.fileSent.emit(this.file);
    }
  }
}
