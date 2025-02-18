import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core'
import { NewGame } from '../models/Game';

@Injectable({
  providedIn: 'root'
})
export class ApiDataService {
  constructor(private http: HttpClient) { }

  addGame(game: NewGame) {
    return this.http.post('/AddGame', game);
  }
}
