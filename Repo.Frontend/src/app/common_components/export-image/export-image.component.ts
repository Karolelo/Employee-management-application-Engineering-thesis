import { Component,Output,EventEmitter } from '@angular/core';

@Component({
  selector: 'app-export-image',
  standalone: false,
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
      console.log('Wysyłam plik:', this.file); // debugging
      this.fileSent.emit(this.file);
    } else {
      console.log('Nie wybrano pliku'); // debugging
    }
  }
}
