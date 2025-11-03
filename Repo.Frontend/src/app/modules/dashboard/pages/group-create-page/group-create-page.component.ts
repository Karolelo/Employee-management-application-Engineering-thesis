import {Component, ViewChild} from '@angular/core';
import {ConfirmDialogComponent} from '../../../../common_components/confirm-dialog/confirm-dialog.component';
import {CanDeactivate} from '@angular/router';
import {MatSnackBar} from '@angular/material/snack-bar';
import {MatDialog} from '@angular/material/dialog';
import {MatStepper} from '@angular/material/stepper';
import {Router} from '@angular/router';
@Component({
  selector: 'app-group-create-page',
  standalone: false,
  templateUrl: './group-create-page.component.html',
  styleUrl: './group-create-page.component.css'
})
export class GroupCreatePageComponent implements CanDeactivate<boolean> {
  isLinear = true;
  createdGroupId: number  = 0;
  isFirstStepCompleted = false;
  isSecondStepCompleted = false;
  @ViewChild('stepper') stepper!: MatStepper;
  constructor(
    private dialog: MatDialog,
    private router: Router
  ) {}

  onGroupCreated(groupId: number) {
    console.log("Group id :"+groupId);
    this.createdGroupId = groupId;
    this.isFirstStepCompleted = true;
    this.stepper.next();
  }

  onUsersAdded(value: boolean) {
    this.isSecondStepCompleted = value;
    this.router.navigate(['/dashboard']);
  }


  canDeactivate(): boolean | Promise<boolean> {
    if (this.isSecondStepCompleted) {
      return true;
    }
    return this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: "Unsaved changes",
        message: "You have unsaved changes. Do you want to leave the page?",
        closeBtn: 'Cancel',
        confirmBtn: 'Exit'
      }
    }).afterClosed().toPromise().then(result => {
      return !!result;
    });
  }
}
