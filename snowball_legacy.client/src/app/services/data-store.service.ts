import { Injectable } from '@angular/core'
import { Subject } from 'rxjs';
import { Game } from '../models/game';

@Injectable({
  providedIn: 'root'
})
export class DataStoreService {
  public readonly _gamesSubject = new Subject<Game[]>();
  public readonly _activeGameSubject = new Subject<string>();
  public readonly _updateGameListSubject = new Subject<boolean>();

  public activeGameSubjectChanges$ = this._activeGameSubject.asObservable();
  public updateGameListSubjectChanges$ = this._updateGameListSubject.asObservable();
  public gamesSubjectChanges$ = this._gamesSubject.asObservable();
}
