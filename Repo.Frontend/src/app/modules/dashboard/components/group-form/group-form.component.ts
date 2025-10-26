import { Component,Input } from '@angular/core';
import { Form,FormBuilder,FormGroup,Validators } from '@angular/forms';
@Component({
  selector: 'app-group-form',
  standalone: false,
  templateUrl: './group-form.component.html',
  styleUrl: './group-form.component.css'
})
export class GroupFormComponent {
  groupForm: FormGroup;
  constructor(private fb: FormBuilder) {
    this.groupForm = this.fb.group(
      {
        name: ['', Validators.required],
        description: ['', Validators.required],
        admin_ID: ['', Validators.pattern('\d{1,}')]
      }
    )
  }
}
