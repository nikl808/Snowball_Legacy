import { Injectable } from '@angular/core'
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DataStoreService {
  public readonly _activeGameSubject = new Subject<string>();

  public activeGameSubjectChanges$ = this._activeGameSubject.asObservable();
}
