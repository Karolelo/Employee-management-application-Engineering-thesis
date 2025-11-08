import { Component,Input,OnChanges,SimpleChanges } from '@angular/core';
import { FormGroup,FormBuilder } from '@angular/forms';
import {GroupService} from '../../services/group/group.service';
import {DomSanitizer,SafeUrl} from '@angular/platform-browser';
import {Group} from '../../interfaces/group';
import {HttpClient,HttpEventType,HttpErrorResponse,HttpResponse} from '@angular/common/http';
@Component({
  selector: 'app-group-edit-form',
  standalone: false,
  templateUrl: './group-edit-form.component.html',
  styleUrl: './group-edit-form.component.css'
})
export class GroupEditFormComponent implements OnChanges{
  @Input() group?: Group;
  imageForm: FormGroup;
  imageUrl?: SafeUrl;
  constructor(private fb: FormBuilder,private group_service: GroupService
              ,private sanitizer: DomSanitizer) {
    this.imageForm = this.fb.group({
      image: [null],
      isUpdate: [false]
    })
  }
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['group'] && this.group) {
      console.log("Values of group "+ this.group.description);
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

  onFileSent(file: File) {
    if (this.imageUrl) {
      console.log("Image updating")
      this.group_service.saveGroupImage(this.group?.id ?? 0, file, true).subscribe(
        {
          next: (response) => {
            console.log("Image save correctly " + response)
            setTimeout(
              () => {
                this.initializeValues();
              }, 3000)
          },
          error: (error) => {
            console.log(error)
          }
        }
      )
    } else {
      console.log("Image creating")
      this.group_service.saveGroupImage(this.group?.id ?? 0, file, false).subscribe(
        {
          next: (response) => {
            console.log("Image save correctly " + response)
            setTimeout(
              () => {
                this.initializeValues();
              }, 3000)
          },
          error: (error) => {
            console.log(error)
          }
        }
      )
    }
  }

  protected readonly FormGroup = FormGroup;
}

