import { Component,Input,OnChanges,SimpleChanges } from '@angular/core';
import { FormGroup,FormBuilder } from '@angular/forms';
import {GroupService} from '../../services/group/group.service';
import {Group} from '../../interfaces/group';
import { NgOptimizedImage } from '@angular/common';

@Component({
  selector: 'app-group-image-edit-form',
  standalone: false,
  templateUrl: './group-image-edit-form.component.html',
  styleUrl: './group-image-edit-form.component.css'
})
export class GroupImageEditFormComponent implements OnChanges {
  private static readonly IMAGE_REFRESH_DELAY_MS = 1000;

  @Input() group?: Group;
  imageForm: FormGroup;
  imageUrl?: string;

  constructor(
    private fb: FormBuilder,
    private groupService: GroupService
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

    this.groupService.getGroupImagePath(this.group.id).subscribe({
      next: (response: any) => {
        this.imageUrl = response.path;
        console.log(this.imageUrl);
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

  getFullImageUrl(): string {
    return this.imageUrl
      ? 'http://localhost:5239' + this.imageUrl
      : '/images/default.png';
  }
}
