import { Component,Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
@Component({
  selector: 'app-delete-user-dialog',
  standalone: false,
  templateUrl: './delete-user-dialog.component.html',
  styleUrl: './delete-user-dialog.component.css'
})
export class DeleteUserDialogComponent {
  constructor(public dialogRef: MatDialogRef<DeleteUserDialogComponent>) { }

  confirmDelete(): void {
    this.dialogRef.close({ confirmed: true });
  }
  cancel(): void {
    this.dialogRef.close({ confirmed: false });
  }
}
