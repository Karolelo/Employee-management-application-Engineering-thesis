import {Task} from './task';

export interface RelatedTasks {
  task: Task;
  relatedTasks: Task[];
}
