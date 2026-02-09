import {
  Component,
  Input,
  ElementRef,
  ViewChild,
  AfterViewInit,
  OnChanges,
  SimpleChanges, HostListener,
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
  @Input() viewDate: Date | null = null;
  @Input() windowDays = 7;
  @ViewChild('ganttWrapper') ganttWrapper!: ElementRef<HTMLDivElement>;
  @ViewChild('arrowsSvg') arrowsSvg!: ElementRef<SVGSVGElement>;

  constructor(private router: Router) {}

  arrows: Arrow[] = [];

  ngAfterViewInit(): void {
    this.scheduleRecalculateArrows();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['tasks'] || changes['viewDate'] || changes['windowDays']) {
      this.scheduleRecalculateArrows();
    }
  }

  @HostListener('window:resize')
  onResize() {
    this.scheduleRecalculateArrows();
  }

  private startOfDay(date: Date): Date {
    return new Date(date.getFullYear(), date.getMonth(), date.getDate());
  }

  private addDays(date: Date, days: number): Date {
    const x = new Date(date);
    x.setDate(x.getDate() + days);
    return x;
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
    const center = this.viewDate ? this.startOfDay(this.viewDate) : this.startOfDay(new Date());
    const backDays = Math.floor(this.windowDays / 2);
    return this.addDays(center, -backDays);
  }

  get axisEnd(): Date | null {
    if (!this.axisStart) return null;
    return this.addDays(this.axisStart, this.windowDays);
  }

  get axisEndInclusive(): Date | null {
    if (!this.axisEnd) return null;
    return this.addDays(this.axisEnd, -1);
  }

  getTotalDays(): number {
    return Math.max(1, this.windowDays || 7);
  }

  isInView(task: GanttTask): boolean {
    if (!this.axisStart || !this.axisEnd) return false;
    return task.end_Time > this.axisStart && task.start_Time < this.axisEnd;
  }

  getLeftPercent(task: GanttTask): number {
    if (!this.axisStart || !this.axisEnd) return 0;

    const totalMs = this.axisEnd.getTime() - this.axisStart.getTime();
    const clippedStart = new Date(Math.max(task.start_Time.getTime(), this.axisStart.getTime()));

    const startMs = clippedStart.getTime() - this.axisStart.getTime();
    const startRatio = startMs / totalMs;
    return startRatio * 100;
  }

  getWidthPercent(task: GanttTask): number {
    if (!this.axisStart || !this.axisEnd) return 0;

    const totalMs = this.axisEnd.getTime() - this.axisStart.getTime();
    const clippedStart = new Date(Math.max(task.start_Time.getTime(), this.axisStart.getTime()));
    const clippedEnd = new Date(Math.min(task.end_Time.getTime(), this.axisEnd.getTime()));

    const durationMs = clippedEnd.getTime() - clippedStart.getTime();
    return Math.max(0, (durationMs / totalMs) * 100);
  }


  getDurationHours(task: GanttTask): number {
    const diffMs = task.end_Time.getTime() - task.start_Time.getTime();
    return Math.round(diffMs / (1000 * 60 * 60));
  }

  getDaysArray(): Date[] {
    const days: Date[] = [];
    if (!this.axisStart) return days;

    const n = this.getTotalDays();
    for (let i = 0; i < n; i++) {
      days.push(this.addDays(this.axisStart, i));
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

  private rafId: number | null = null;

  private scheduleRecalculateArrows(): void {
    if (this.rafId) cancelAnimationFrame(this.rafId);

    this.rafId = requestAnimationFrame(() => {
      this.rafId = requestAnimationFrame(() => {
        this.recalculateArrows();
        this.rafId = null;
      });
    });
  }

  private clamp(v: number, min: number, max: number): number {
    return Math.max(min, Math.min(max, v));
  }

  private timeToX(date: Date, trackRect: DOMRect, containerRect: DOMRect): number {
    if (!this.axisStart || !this.axisEnd) return trackRect.left - containerRect.left;

    const totalMs = this.axisEnd.getTime() - this.axisStart.getTime();
    const ratio = (date.getTime() - this.axisStart.getTime()) / totalMs;

    const x = (trackRect.left - containerRect.left) + ratio * trackRect.width;
    const left = trackRect.left - containerRect.left;
    const right = trackRect.right - containerRect.left;

    return this.clamp(x, left, right);
  }

  private recalculateArrows(): void {
    this.arrows = [];
    if (!this.tasks || !this.tasks.length || !this.ganttWrapper || !this.arrowsSvg) return;
    if (!this.axisStart || !this.axisEnd) return;

    const wrapperEl = this.ganttWrapper.nativeElement;
    const containerEl = wrapperEl.querySelector('.gantt-container') as HTMLDivElement;
    if (!containerEl) return;

    const svgEl = this.arrowsSvg.nativeElement;
    const containerRect = containerEl.getBoundingClientRect();

    const trackNodes = containerEl.querySelectorAll<HTMLElement>('.gantt-row-track');
    const trackById = new Map<number, HTMLElement>();

    trackNodes.forEach(el => {
      const idAttr = el.getAttribute('data-task-id');
      if (!idAttr) return;
      const id = Number(idAttr);
      if (!isNaN(id)) trackById.set(id, el);
    });

    const taskById = new Map<number, GanttTask>();
    this.tasks.forEach(t => taskById.set(t.id, t));

    const arrows: Arrow[] = [];

    for (const childTask of this.tasks) {
      if (!childTask.dependencies?.length) continue;

      const childTrack = trackById.get(childTask.id);
      if (!childTrack) continue;

      const childTrackRect = childTrack.getBoundingClientRect();
      const childCenterY = childTrackRect.top - containerRect.top + childTrackRect.height / 2;

      const childX = this.timeToX(childTask.start_Time, childTrackRect, containerRect);

      for (const parentId of childTask.dependencies) {
        const parentTask = taskById.get(parentId);
        const parentTrack = trackById.get(parentId);
        if (!parentTask || !parentTrack) continue;

        const childVisible = this.isInView(childTask);
        const parentVisible = this.isInView(parentTask);
        if (!childVisible && !parentVisible) continue;

        const parentTrackRect = parentTrack.getBoundingClientRect();
        const parentCenterY = parentTrackRect.top - containerRect.top + parentTrackRect.height / 2;

        const parentX = this.timeToX(parentTask.end_Time, parentTrackRect, containerRect);

        if (Math.abs(parentX - childX) < 2 && Math.abs(parentCenterY - childCenterY) < 2) {
          continue;
        }

        const channelYOffset = 18;
        const channelY = Math.min(parentCenterY, childCenterY) - channelYOffset;

        const dir = (childX - parentX) === 0 ? 1 : Math.sign(childX - parentX);
        const hOffset = 8 * dir;

        const d = [
          `M ${parentX} ${parentCenterY}`,
          `V ${channelY}`,
          `H ${childX - hOffset}`,
          `V ${childCenterY}`,
          `H ${childX}`,
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
