import {
  Component,
  Input,
  ElementRef,
  ViewChild,
} from '@angular/core';
import { GanttTask } from '../../interfaces/gantt-task';
import html2canvas from 'html2canvas';
import jsPDF from 'jspdf';

@Component({
  selector: 'app-task-gantt',
  templateUrl: './task-gantt.component.html',
  styleUrl: './task-gantt.component.css',
  standalone: false,
})
export class TaskGanttComponent {
  @Input() tasks: GanttTask[] | null = [];
  @ViewChild('ganttContainer') ganttContainer!: ElementRef<HTMLDivElement>;

  get minDate(): Date | null {
    if (!this.tasks || this.tasks.length === 0) return null;
    return this.tasks.reduce(
      (min, t) => (t.start_Time < min ? t.start_Time : min),
      this.tasks[0].start_Time
    );
  }

  get maxDate(): Date | null {
    if (!this.tasks || this.tasks.length === 0) return null;
    return this.tasks.reduce(
      (max, t) => (t.end_Time > max ? t.end_Time : max),
      this.tasks[0].end_Time
    );
  }

  getTotalDays(): number {
    if (!this.minDate || !this.maxDate) return 1;
    const diffMs = this.maxDate.getTime() - this.minDate.getTime();
    return Math.max(1, Math.ceil(diffMs / (1000 * 60 * 60 * 24)));
  }

  getLeftPercent(task: GanttTask): number {
    if (!this.minDate) return 0;
    const diffMs = task.start_Time.getTime() - this.minDate.getTime();
    const days = diffMs / (1000 * 60 * 60 * 24);
    return (days / this.getTotalDays()) * 100;
  }

  getWidthPercent(task: GanttTask): number {
    const diffMs = task.end_Time.getTime() - task.start_Time.getTime();
    const days = diffMs / (1000 * 60 * 60 * 24);
    return (days / this.getTotalDays()) * 100;
  }

  barClass(task: GanttTask): string {
    const s = task.status.toLowerCase();
    if (s.includes('finished') || s.includes('done')) return 'bar done';
    if (s.includes('in-progress')) return 'bar progress';
    return 'bar todo';
  }

  async exportToPdf(): Promise<void> {
    if (!this.ganttContainer) return;

    const element = this.ganttContainer.nativeElement;
    const canvas = await html2canvas(element, { scale: 2 });
    const imgData = canvas.toDataURL('image/png');

    const pdf = new jsPDF('landscape', 'mm', 'a4');
    const pageWidth = pdf.internal.pageSize.getWidth();
    const pageHeight = pdf.internal.pageSize.getHeight();

    const imgWidth = pageWidth - 10;
    const imgHeight = (canvas.height * imgWidth) / canvas.width;

    const x = 5;
    const y = (pageHeight - imgHeight) / 2;

    pdf.addImage(imgData, 'PNG', x, y, imgWidth, imgHeight);
    pdf.save('tasks-gantt.pdf');
  }
}
