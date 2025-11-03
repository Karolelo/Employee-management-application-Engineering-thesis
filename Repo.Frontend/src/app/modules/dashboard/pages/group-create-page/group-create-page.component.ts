import { Component,AfterViewInit } from '@angular/core';
import {ConfirmDialogComponent} from '../../../../common_components/confirm-dialog/confirm-dialog.component';
import {CanDeactivate} from '@angular/router';
import {MatSnackBar} from '@angular/material/snack-bar';
import {MatDialog} from '@angular/material/dialog';
@Component({
  selector: 'app-group-create-page',
  standalone: false,
  templateUrl: './group-create-page.component.html',
  styleUrl: './group-create-page.component.css'
})
export class GroupCreatePageComponent implements CanDeactivate<boolean>,AfterViewInit {
  isLinear = true;
  createdGroupId: number  = 0;
  isSecondStepCompleted = false;

  constructor(
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  onGroupCreated(groupId: number) {
    this.createdGroupId = groupId;
  }

  onUsersAdded() {
    this.isSecondStepCompleted = true;
  }

  onAddUser(){

  }

  canDeactivate(): boolean | Promise<boolean> {
    if (this.isSecondStepCompleted) {
      return true;
    }
    return false;
  }
}
