import { Component,Input,OnChanges,SimpleChanges } from '@angular/core';
import {Group} from '../../interfaces/group';
import {GroupService} from '../../services/group/group.service';
import {DomSanitizer,SafeUrl} from '@angular/platform-browser';
@Component({
  selector: 'app-group-short-info',
  standalone: false,
  templateUrl: './group-short-info.component.html',
  styleUrl: './group-short-info.component.css'
})
export class GroupShortInfoComponent implements OnChanges {
  @Input() group?: Group;
  imageUrl?: SafeUrl;

  constructor(private group_service: GroupService,
              private sanitizer: DomSanitizer) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['group'] && this.group) {
      console.log("Values of group ");
      this.initializeValues();
    }
  }

  initializeValues() {
    if (!this.group) return;

    this.group_service.getGroupImagePath(this.group.id).subscribe({
      next: (blob: Blob) => {
        const objectUrl = URL.createObjectURL(blob);
        this.imageUrl = this.sanitizer.bypassSecurityTrustUrl(objectUrl);
      },
      error: (error) => {
          if(error.status == 404){
            console.log("Image not found")
            return
          }
          if(error.status != 404)
            console.error('Error during taking image:', error);
        }
    });
  }
}
