const MS_PER_DAY = 24 * 60 * 60 * 1000;

export interface DateWindow {
  from: Date;
  to: Date;
}

export function addDays(date: Date, days: number): Date {
  return new Date(date.getTime() + days * MS_PER_DAY);
}

export function startOfDay(date: Date): Date {
  const next = new Date(date);
  next.setHours(0, 0, 0, 0);
  return next;
}

export function endOfDay(date: Date): Date {
  const next = new Date(date);
  next.setHours(23, 59, 59, 999);
  return next;
}

/** Default account events window: 15 days back … 15 days forward. */
export function createDefaultEventWindow(now = new Date()): DateWindow {
  return {
    from: startOfDay(addDays(now, -15)),
    to: endOfDay(addDays(now, 15)),
  };
}

export function shiftDateWindow(window: DateWindow, days: number): DateWindow {
  return {
    from: addDays(window.from, days),
    to: addDays(window.to, days),
  };
}

export function formatDateRangeLabel(window: DateWindow): string {
  const options: Intl.DateTimeFormatOptions = {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
  };

  const from = window.from.toLocaleDateString('ru-RU', options);
  const to = window.to.toLocaleDateString('ru-RU', options);
  return `${from} — ${to}`;
}

export function toDateTimeLocalValue(date: Date): string {
  const pad = (value: number) => String(value).padStart(2, '0');

  return [
    date.getFullYear(),
    '-',
    pad(date.getMonth() + 1),
    '-',
    pad(date.getDate()),
    'T',
    pad(date.getHours()),
    ':',
    pad(date.getMinutes()),
  ].join('');
}

export function fromDateTimeLocalValue(value: string): Date | null {
  if (!value) {
    return null;
  }

  const date = new Date(value);
  return Number.isNaN(date.getTime()) ? null : date;
}
