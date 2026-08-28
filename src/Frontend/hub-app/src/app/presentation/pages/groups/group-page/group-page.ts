import { Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute } from '@angular/router';
import { map } from 'rxjs';

@Component({
  selector: 'app-group-page',
  imports: [],
  templateUrl: './group-page.html',
  styleUrl: './group-page.scss',
})
export class GroupPage {
  private readonly route = inject(ActivatedRoute);

  protected readonly groupId = toSignal(
    this.route.paramMap.pipe(map((params) => params.get('groupId'))),
    { initialValue: null },
  );
}
