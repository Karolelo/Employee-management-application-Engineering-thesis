import {Component,AfterViewInit,ViewChild,OnInit} from '@angular/core';
import {Group} from '../../interfaces/group';
import {ActivatedRoute} from '@angular/router';
import {GroupService} from '../../services/group/group.service';
import {MatDialog} from '@angular/material/dialog';
import {ConfirmDialogComponent} from '../../../../common_components/confirm-dialog/confirm-dialog.component';
@Component({
  selector: 'app-groups-page',
  standalone: false,
  templateUrl: './groups-page.component.html',
  styleUrl: './groups-page.component.css'
})
export class GroupsPageComponent {
  groups: Group[] = [];
  selectedGroups: Group[] = [];
  private pageSize = 5;
  private currentPage = 1;
  constructor(private route: ActivatedRoute,private dialog: MatDialog,private groupService: GroupService) {}
  ngOnInit() {
    this.groups = this.route.snapshot.data['groups'];
    this.selectedGroups = this.groups.slice(0, this.pageSize);
  }

  onLeftArrowClick() {
    if(this.currentPage > 1) {
      this.currentPage--;
    }
    this.selectedGroups = this.groups.slice(
      (this.currentPage - 1) * this.pageSize,
      this.currentPage * this.pageSize
    )
  }

  onRightArrowClick() {
    if(this.currentPage < Math.ceil(this.groups.length / this.pageSize)) {
      this.currentPage++;
    }
    this.selectedGroups = this.groups.slice(
      (this.currentPage - 1) * this.pageSize,
      this.currentPage * this.pageSize
    )
  }

  deleteGroup(id: number) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Delete group',
        message: 'You sure you want to delete this group?',
        closeBtn: 'Cancel',
        confirmBtn: 'Delete'
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe((result: boolean) => {
      if (result) {
        this.groups = this.groups.filter(group => group.id !== id);
        this.selectedGroups = this.groups.filter(group => group.id !== id);
        this.groupService.deleteGroup(id).subscribe({
          error: (error) => {
            console.error('Error deleting group:', error);
          }
        });
      }
    });
  }
}
