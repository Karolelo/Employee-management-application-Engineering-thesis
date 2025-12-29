export interface Target {
  id: number;
  name: string;
  description: string;
  start_Time: string;       // ISO DateTime from API, e.g. "2025-04-01T00:00:00"
  finish_Time?: string | null;
  tag?: string | null;
}

export type TargetMini = {
  name: string;
  description: string;
  start_Time: string;       // send as ISO (YYYY-MM-DD or full ISO)
  finish_Time?: string | null;
  tag?: string | null;
};
