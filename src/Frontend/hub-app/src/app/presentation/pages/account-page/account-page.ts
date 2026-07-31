import {
  Component,
  OnInit,
  computed,
  inject,
  signal,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { firstValueFrom } from 'rxjs';

import { AuthenticationService } from '../../../application/services/authentication.service';
import { EventService } from '../../../application/services/event.service';
import { GroupService } from '../../../application/services/group.service';
import {
  EventDetails,
  EventSummary,
} from '../../../application/models/event.model';
import { GroupSummary } from '../../../application/models/group.model';
import { RichText, TextFormat } from '../../../application/models/rich-text.model';
import {
  DateWindow,
  createDefaultEventWindow,
  formatDateRangeLabel,
  shiftDateWindow,
} from '../../shared/utils/date-window.utils';
import {
  eventStateLabel,
  formatEventDateTime,
} from '../../shared/utils/event-display.utils';
import { HtmlSanitizerService } from '../../shared/components/rich-text-editor/services/html-sanitizer.service';

const RANGE_SHIFT_DAYS = 30;

@Component({
  selector: 'app-account-page',
  imports: [FormsModule],
  templateUrl: './account-page.html',
  styleUrl: './account-page.scss',
})
export class AccountPage implements OnInit {
  private readonly auth = inject(AuthenticationService);
  private readonly groupService = inject(GroupService);
  private readonly eventService = inject(EventService);
  private readonly htmlSanitizer = inject(HtmlSanitizerService);
  private readonly domSanitizer = inject(DomSanitizer);

  protected readonly groups = signal<GroupSummary[]>([]);
  protected readonly selectedGroupId = signal<string | null>(null);
  protected readonly dateWindow = signal<DateWindow>(createDefaultEventWindow());
  protected readonly events = signal<EventSummary[]>([]);
  protected readonly selectedEventId = signal<string | null>(null);
  protected readonly selectedEvent = signal<EventDetails | null>(null);

  protected readonly isLoading = signal(true);
  protected readonly isEventsLoading = signal(false);
  protected readonly isDetailsLoading = signal(false);
  protected readonly loadError = signal<string | null>(null);
  protected readonly eventsError = signal<string | null>(null);
  protected readonly detailsError = signal<string | null>(null);

  protected readonly eventStateLabel = eventStateLabel;
  protected readonly formatEventDateTime = formatEventDateTime;

  protected descriptionHtml(description: RichText): SafeHtml {
    if (description.format === TextFormat.Html) {
      const clean = this.htmlSanitizer.sanitize(description.text);
      return this.domSanitizer.bypassSecurityTrustHtml(clean || '');
    }

    const escaped = description.text
      .replaceAll('&', '&amp;')
      .replaceAll('<', '&lt;')
      .replaceAll('>', '&gt;')
      .replaceAll('"', '&quot;')
      .replaceAll("'", '&#39;');

    return this.domSanitizer.bypassSecurityTrustHtml(
      escaped ? `<p>${escaped}</p>` : '',
    );
  }

  protected readonly dateRangeLabel = computed(() =>
    formatDateRangeLabel(this.dateWindow()),
  );

  protected readonly selectedGroup = computed(() => {
    const id = this.selectedGroupId();
    return this.groups().find((group) => group.id === id) ?? null;
  });

  async ngOnInit(): Promise<void> {
    await this.loadGroups();
  }

  protected async onGroupChange(groupId: string): Promise<void> {
    this.selectedGroupId.set(groupId);
    this.selectedEventId.set(null);
    this.selectedEvent.set(null);
    await this.loadEvents();
  }

  protected async shiftRangeBack(): Promise<void> {
    this.dateWindow.update((window) =>
      shiftDateWindow(window, -RANGE_SHIFT_DAYS),
    );
    await this.loadEvents();
  }

  protected async shiftRangeForward(): Promise<void> {
    this.dateWindow.update((window) =>
      shiftDateWindow(window, RANGE_SHIFT_DAYS),
    );
    await this.loadEvents();
  }

  protected async selectEvent(event: EventSummary): Promise<void> {
    if (this.selectedEventId() === event.id) {
      return;
    }

    this.selectedEventId.set(event.id);
    await this.loadEventDetails(event.id);
  }

  protected reload(): void {
    void this.loadGroups();
  }

  private async loadGroups(): Promise<void> {
    const user = this.auth.user();
    if (!user) {
      this.loadError.set('Не удалось определить текущего пользователя.');
      this.isLoading.set(false);
      return;
    }

    this.isLoading.set(true);
    this.loadError.set(null);

    try {
      const response = await firstValueFrom(
        this.groupService.getList({
          memberUserId: user.id,
          take: 30,
          skip: 0,
        }),
      );

      this.groups.set(response.values);

      if (response.values.length === 0) {
        this.selectedGroupId.set(null);
        this.events.set([]);
        this.selectedEventId.set(null);
        this.selectedEvent.set(null);
        return;
      }

      const firstGroupId = response.values[0].id;
      this.selectedGroupId.set(firstGroupId);
      await this.loadEvents();
    } catch {
      this.loadError.set('Не удалось загрузить группы. Попробуйте ещё раз.');
    } finally {
      this.isLoading.set(false);
    }
  }

  private async loadEvents(): Promise<void> {
    const groupId = this.selectedGroupId();
    if (!groupId) {
      return;
    }

    const window = this.dateWindow();
    this.isEventsLoading.set(true);
    this.eventsError.set(null);

    try {
      const response = await firstValueFrom(
        this.groupService.getEvents({
          groupId,
          fromDate: window.from.toISOString(),
          toDate: window.to.toISOString(),
        }),
      );

      this.events.set(response.values);

      const selectedId = this.selectedEventId();
      const stillVisible = response.values.some(
        (event) => event.id === selectedId,
      );

      if (!stillVisible) {
        this.selectedEventId.set(null);
        this.selectedEvent.set(null);
      }
    } catch {
      this.eventsError.set('Не удалось загрузить события.');
      this.events.set([]);
      this.selectedEventId.set(null);
      this.selectedEvent.set(null);
    } finally {
      this.isEventsLoading.set(false);
    }
  }

  private async loadEventDetails(eventId: string): Promise<void> {
    this.isDetailsLoading.set(true);
    this.detailsError.set(null);

    try {
      const details = await firstValueFrom(this.eventService.getById(eventId));
      if (this.selectedEventId() === eventId) {
        this.selectedEvent.set(details);
      }
    } catch {
      this.detailsError.set('Не удалось загрузить информацию о событии.');
      this.selectedEvent.set(null);
    } finally {
      this.isDetailsLoading.set(false);
    }
  }
}
