import {Component, ViewChild} from '@angular/core';
import {ConfirmDialogComponent} from '../../../../common_components/confirm-dialog/confirm-dialog.component';
import {CanDeactivate} from '@angular/router';
import {MatSnackBar} from '@angular/material/snack-bar';
import {MatDialog} from '@angular/material/dialog';
import {MatStepper} from '@angular/material/stepper';
import {Router} from '@angular/router';
import {Group} from '../../interfaces/group';
@Component({
  selector: 'app-group-create-page',
  standalone: false,
  templateUrl: './group-create-page.component.html',
  styleUrl: './group-create-page.component.css'
})
export class GroupCreatePageComponent implements CanDeactivate<boolean> {
  isLinear = true;
  createdGroupId: number  = 0;
  createdAdminId: number = 0;
  isFirstStepCompleted = false;
  isSecondStepCompleted = false;
  @ViewChild('stepper') stepper!: MatStepper;
  constructor(
    private dialog: MatDialog,
    private router: Router
  ) {}

  onGroupCreated(group: Group) {
    this.createdGroupId = group.id;
    this.createdAdminId = group.admin_ID;
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
        title: 'Waitt',
        message: 'You sure you want to leave progress will be gone?',
        closeBtn: 'Stay',
        confirmBtn: 'Leave'
      },
      disableClose: true
    }).afterClosed().toPromise().then(result => {
      return !!result;
    });
  }
}
