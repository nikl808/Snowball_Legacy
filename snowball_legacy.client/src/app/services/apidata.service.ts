import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core'
import { GameVM } from '../models/viewModels/game.vm';
import { Game } from '../models/game';
import { catchError, Observable, of } from 'rxjs';
import { GameInfo } from '../models/gameInfo';

@Injectable({
  providedIn: 'root'
})
export class ApiDataService {
  private readonly headers: HttpHeaders;
  constructor(private http: HttpClient) {
    this.headers = new HttpHeaders({ 'Access-Control-Allow-Origin': '*' });
  }

  getGames() {
    return this.http.get<Game[]>('/api/game/list')
     .pipe(
       catchError(this.handleError<Game[]>('getGames', []))
     );
  }

  getGameInfo(gameId: string) {
    return this.http.get<GameInfo>('/api/game/info/' + gameId)
      .pipe(
        catchError(this.handleError<GameInfo>(`getGameInfo id=${gameId}`))
      );
  }

  public getGameTitlePicture(gameInfoId: number) {
    return this.http.get('/api/game/titlePicture/' + gameInfoId, { responseType: 'blob' }).pipe(
      catchError(this.handleError<Blob>('getGameTitlePicture unknown file'))
    );
  }

  addGame(game: GameVM) {
    const formData = new FormData();
    formData.append('Name', game.name);
    formData.append('Genre', game.genre);
    formData.append('ReleaseDate', game.releaseDate);
    formData.append('Description', game.description)
    formData.append('DiskNumber', game.discNumber?.toString());
    if(game.titlePicture != undefined) {
      formData.append('TitlePicture', game.titlePicture, game.titlePicture.name);
    }
    if (game.screenshots != null) {
      for (let i = 0; i < game.screenshots.length; i++) {
        formData.append('Screenshots', game.screenshots[i], game.screenshots[i].name);
      }
    }
    if (game.additionalFiles != null) {
      for (let i = 0; i < game.additionalFiles.length; i++) {
        formData.append('AdditionalFiles', game.additionalFiles[i], game.additionalFiles[i].name);
      }
    }

    this.http.post('/api/game', formData, { headers: this.headers }).subscribe({
      next: result => {
        console.log(result);
      },
      error: (err) => {
        console.log(err);
      }
    })
  }

  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {

      // TODO: send the error to remote logging infrastructure
      console.error(error); // log to console instead

      // TODO: better job of transforming error for user consumption
      console.log(`${operation} failed: ${error.message}`);

      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }
}
