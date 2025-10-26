import { Component,Inject } from '@angular/core';
import {CommonModule} from '@angular/common';
import { MatDialogModule,MatDialogRef,MAT_DIALOG_DATA } from '@angular/material/dialog';
@Component({
  selector: 'app-confirm-dialog',
  imports: [CommonModule,MatDialogModule],
  templateUrl: './confirm-dialog.component.html',
  styleUrl: './confirm-dialog.component.css'
})

export class ConfirmDialogComponent {
  /*@Input() template?: TemplateRef<any>;*/
  constructor(private dialogRef: MatDialogRef<ConfirmDialogComponent>
              ,@Inject(MAT_DIALOG_DATA) public data: any) {

  }
  confirm(){
    this.dialogRef.close(true);
  }

  cancel(){
    this.dialogRef.close(false);
  }
}
