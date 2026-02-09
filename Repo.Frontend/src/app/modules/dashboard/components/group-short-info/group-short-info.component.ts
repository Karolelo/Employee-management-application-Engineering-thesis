import { Component,Input,OnChanges,SimpleChanges,inject} from '@angular/core';
import {Group} from '../../interfaces/group';
import {GroupService} from '../../services/group/group.service';
import { environment} from '../../../../../environments/environment';

@Component({
  selector: 'app-group-short-info',
  standalone: false,
  templateUrl: './group-short-info.component.html',
  styleUrl: './group-short-info.component.css'
})
export class GroupShortInfoComponent implements OnChanges {
  private static readonly DEFAULT_IMAGE_URL = '/images/default.png';

  @Input() group?: Group;
  imageUrl?: string;
  private groupService: GroupService = inject(GroupService);

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['group'] && this.group) {
      this.initializeValues();
    }
  }

  private initializeValues(): void {
    if (!this.group) return;

    this.groupService.getGroupImagePath(this.group.id).subscribe({
      next: (response: any) => {
        this.imageUrl = response.path;
      },
      error: (error) => {
        this.handleImageLoadError(error);
      }
    });
  }

  private handleImageLoadError(error: any): void {
    if (error.status === 404) {
      console.log("Image not found");
    } else {
      console.error('Error during taking image:', error);
    }
  }
  // Temporary changes
  getFullImageUrl(): string {
    return this.imageUrl
      ? environment.API_BASE_URL + this.imageUrl
      : GroupShortInfoComponent.DEFAULT_IMAGE_URL;
  }
}
