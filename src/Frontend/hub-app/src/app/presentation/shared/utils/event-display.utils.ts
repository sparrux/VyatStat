import { EventState } from '../../../application/models/event.model';

const EVENT_STATE_LABELS: Record<EventState, string> = {
  [EventState.Draft]: 'Черновик',
  [EventState.RegistrationOpen]: 'Регистрация открыта',
  [EventState.RegistrationClosed]: 'Регистрация закрыта',
  [EventState.InProgress]: 'Идёт',
  [EventState.Completed]: 'Завершено',
  [EventState.Cancelled]: 'Отменено',
};

export function eventStateLabel(state: EventState): string {
  return EVENT_STATE_LABELS[state] ?? 'Неизвестно';
}

export function formatEventDateTime(value: string): string {
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) {
    return value;
  }

  return date.toLocaleString('ru-RU', {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });
}
