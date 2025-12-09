import { Component,Input,OnChanges,SimpleChanges } from '@angular/core';
import { FormGroup,FormBuilder } from '@angular/forms';
import {GroupService} from '../../services/group/group.service';
import {DomSanitizer,SafeUrl} from '@angular/platform-browser';
import {Group} from '../../interfaces/group';

@Component({
  selector: 'app-group-image-edit-form',
  standalone: false,
  templateUrl: './group-image-edit-form.component.html',
  styleUrl: './group-image-edit-form.component.css'
})
export class GroupImageEditFormComponent implements OnChanges {
  private static readonly IMAGE_REFRESH_DELAY_MS = 3000;
  @Input() group?: Group;
  imageForm: FormGroup;
  imageUrl?: SafeUrl;
  oldBlobUrl?: string;
  constructor(
    private fb: FormBuilder,
    private groupService: GroupService,
    private sanitizer: DomSanitizer
  ) {
    this.imageForm = this.fb.group({
      image: [null],
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['group'] && this.group) {
      console.log("Values of group " + this.group.description);
      this.initializeValues();
    }
  }

  initializeValues() {
    if (!this.group) return;

    if(this.oldBlobUrl) URL.revokeObjectURL(this.oldBlobUrl)

    this.groupService.getGroupImagePath(this.group.id).subscribe({
      next: (blob: Blob) => {
        const objectUrl = URL.createObjectURL(blob);
        this.oldBlobUrl = objectUrl;
        this.imageUrl = this.sanitizer.bypassSecurityTrustUrl(objectUrl);
      },
      error: (error) => {
        if (error.status === 404) {
          console.log("Image not found");
          return;
        }
      }
    });
  }

  onFileSent(file: File) {
    const isUpdate = !!this.imageUrl;
    const groupId = this.group?.id ?? 0;

    console.log(isUpdate ? "Image updating" : "Image creating");
    this.saveGroupImageWithRefresh(groupId, file, isUpdate);
  }

  private saveGroupImageWithRefresh(groupId: number, file: File, isUpdate: boolean): void {
    this.groupService.saveGroupImage(groupId, file, isUpdate).subscribe({
      next: (response) => {
        console.log("Image save correctly " + response);
        setTimeout(() => {
          this.initializeValues();
        }, GroupImageEditFormComponent.IMAGE_REFRESH_DELAY_MS);
      },
      error: (error) => {
        console.log(error);
      }
    });
  }
}

