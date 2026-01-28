import {
  Component,
  Input,
  ElementRef,
  ViewChild,
  AfterViewInit,
  OnChanges,
  SimpleChanges,
} from '@angular/core';
import { GanttTask } from '../../interfaces/gantt-task';
import { Router } from '@angular/router';
import html2canvas from 'html2canvas';
import jsPDF from 'jspdf';

interface Arrow {
  d: string; // SVG line
}

@Component({
  selector: 'app-task-gantt',
  templateUrl: './task-gantt.component.html',
  styleUrl: './task-gantt.component.css',
  standalone: false,
})
export class TaskGanttComponent implements AfterViewInit, OnChanges {
  @Input() tasks: GanttTask[] | null = [];
  @ViewChild('ganttWrapper') ganttWrapper!: ElementRef<HTMLDivElement>;
  @ViewChild('arrowsSvg') arrowsSvg!: ElementRef<SVGSVGElement>;

  constructor(private router: Router) {}

  arrows: Arrow[] = [];

  ngAfterViewInit(): void {
    this.recalculateArrows();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['tasks']) {
      setTimeout(() => this.recalculateArrows());
    }
  }

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


  get axisStart(): Date | null {
    if (!this.minDate) return null;
    return new Date(this.minDate.getFullYear(), this.minDate.getMonth(), this.minDate.getDate());
  }

  get axisEnd(): Date | null {
    if (!this.maxDate) return null;
    const d = new Date(this.maxDate.getFullYear(), this.maxDate.getMonth(), this.maxDate.getDate());
    d.setDate(d.getDate() + 1);
    return d;
  }

  getTotalDays(): number {
    if (!this.axisStart || !this.axisEnd) return 1;
    const diffMs = this.axisEnd.getTime() - this.axisStart.getTime();
    return Math.max(1, diffMs / (1000 * 60 * 60 * 24));
  }

  getLeftPercent(task: GanttTask): number {
    if (!this.axisStart || !this.axisEnd) return 0;

    const totalMs = this.axisEnd.getTime() - this.axisStart.getTime();
    const startMs = task.start_Time.getTime() - this.axisStart.getTime();

    const startRatio = startMs / totalMs;
    return startRatio * 100;
  }

  getWidthPercent(task: GanttTask): number {
    if (!this.axisStart || !this.axisEnd) return 0;

    const totalMs = this.axisEnd.getTime() - this.axisStart.getTime();
    const durationMs = task.end_Time.getTime() - task.start_Time.getTime();

    const widthRatio = durationMs / totalMs;
    return widthRatio * 100;
  }


  getDurationHours(task: GanttTask): number {
    const diffMs = task.end_Time.getTime() - task.start_Time.getTime();
    return Math.round(diffMs / (1000 * 60 * 60));
  }

  getDaysArray(): Date[] {
    const days: Date[] = [];
    if (!this.minDate || !this.maxDate || !this.tasks || this.tasks.length === 0) {
      return days;
    }

    const start = new Date(this.minDate.getFullYear(), this.minDate.getMonth(), this.minDate.getDate());
    let end = new Date(this.maxDate.getFullYear(), this.maxDate.getMonth(), this.maxDate.getDate());

    const realMaxEnd = this.tasks.reduce(
      (max, t) => (t.end_Time > max ? t.end_Time : max),
      this.tasks[0].end_Time
    );

    if (realMaxEnd > end) {
      end = new Date(realMaxEnd.getFullYear(), realMaxEnd.getMonth(), realMaxEnd.getDate());
    }

    for (let d = new Date(start); d <= end; d.setDate(d.getDate() + 1)) {
      days.push(new Date(d));
    }

    return days;
  }


  barClass(task: GanttTask): string {
    switch (task.status.toLowerCase()) {
      case 'finished':
        return 'bar done';
      case 'in-progress':
        return 'bar progress';
      default:
        return 'bar todo';
    }
  }

  private recalculateArrows(): void {
    this.arrows = [];
    if (!this.tasks || !this.tasks.length || !this.ganttWrapper || !this.arrowsSvg) return;

    const wrapperEl = this.ganttWrapper.nativeElement;
    const containerEl = wrapperEl.querySelector('.gantt-container') as HTMLDivElement;
    if (!containerEl) return;

    const svgEl = this.arrowsSvg.nativeElement;
    const containerRect = containerEl.getBoundingClientRect();

    const barNodes = containerEl.querySelectorAll<HTMLElement>('.gantt-bar');

    const byId = new Map<number, HTMLElement>();
    barNodes.forEach(el => {
      const idAttr = el.getAttribute('data-task-id');
      if (!idAttr) return;
      const id = Number(idAttr);
      if (!isNaN(id)) byId.set(id, el);
    });

    const arrows: Arrow[] = [];

    for (const task of this.tasks) {
      if (!task.dependencies || !task.dependencies.length) continue;

      const childBar = byId.get(task.id);
      if (!childBar) continue;

      const childRect = childBar.getBoundingClientRect();
      const childCenterY = childRect.top - containerRect.top + childRect.height / 2;
      const childXLeft = childRect.left - containerRect.left;

      for (const depId of task.dependencies) {
        const parentBar = byId.get(depId);
        if (!parentBar) continue;

        const parentRect = parentBar.getBoundingClientRect();
        const parentCenterY = parentRect.top - containerRect.top + parentRect.height / 2;
        const parentXRight = parentRect.right - containerRect.left;

        const channelYOffset = 18;
        const channelY = Math.min(parentCenterY, childCenterY) - channelYOffset;
        const hOffset = 8;

        const startX = parentXRight;
        const startY = parentCenterY;
        const endX = childXLeft;
        const endY = childCenterY;

        const d = [
          `M ${startX} ${startY}`,
          `V ${channelY}`,
          `H ${endX - hOffset}`,
          `V ${endY}`,
          `H ${endX}`,
        ].join(' ');

        arrows.push({ d });
      }
    }

    svgEl.setAttribute('width', String(containerRect.width));
    svgEl.setAttribute('height', String(containerRect.height));
    this.arrows = arrows;
  }

  openDetails(taskId: number): void {
    if (!taskId) return;
    this.router.navigate(['/tasks/task-details', taskId]);
  }

  async exportToPdf(): Promise<void> {
    if (!this.ganttWrapper) return;

    const element = this.ganttWrapper.nativeElement;
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
