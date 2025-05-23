import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core'
import { GameVM } from '../models/viewModels/game.vm';
import { Game } from '../models/game';
import { GamesGenres } from '../models/gamesGenres';
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

  public getGamesGenresFromJson() {
    return this.get<GamesGenres>("/genres.json", "getGamesGenresFromJson");
  }

  getGames() {
    return this.get<Game[]>('/api/game/list', 'getGames');
  }

  getGameInfo(gameId: string) {
    return this.get<GameInfo>(`/api/game/info/${gameId}`, `getGameInfo id=${gameId}`);
  }

  public getGameTitlePicture(gameInfoId: number) {
    return this.getBlob(`/api/gamepicture/title/${gameInfoId}`, 'getGameTitlePicture');
  }

  public getGameScreenshots(gameInfoId: number) {
    return this.getBlob(`/api/gamepicture/screenshots/${gameInfoId}`, 'getGameScreenshots');
  }

  public getAdditionalGameFiles(gameId: string) {
    return this.getBlob(`/api/gamefile/archive/${gameId}`, 'getAdditionalGameFiles');
  }

  addGame(game: GameVM) {
    return this.http.post('/api/game', this.setFormData(game),
      { headers: this.headers, reportProgress: true, observe: 'events', responseType: 'text' });
  }

  updateGame(game: GameVM) {
    return this.http.put('/api/game/update', this.setFormData(game),
      { headers: this.headers, reportProgress: true, observe: 'events', responseType: 'text' });
  }

  deleteGame(gameId: string) {
    let header: HttpHeaders = new HttpHeaders({ 'gameId': gameId });
    return this.http.delete('/api/game/', { headers: header, reportProgress: true, observe: 'events', responseType: 'text' });
  }

  private get<T>(url: string, operation: string): Observable<T> {
    return this.http.get<T>(url)
      .pipe(
        catchError(this.handleError<T>(operation))
      );
  }

  private getBlob(url: string, operation: string): Observable<Blob> {
    return this.http.get(url, { responseType: 'blob' }).pipe(
      catchError(this.handleError<Blob>(operation))
    );
  }

  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      // TODO: send the error to remote logging infrastructure
      console.error(`${operation} failed ${error}`); // log to console instead

      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }

  private appendFiles(formData: FormData, files: File[] | null, fieldName: string): void {
    if (files) {
      files.forEach(file => formData.append(fieldName, file, file.name));
    }
  }

  private setFormData(game: GameVM) {
    const formData = new FormData();
    formData.append('Id', game.id == '' ? '0' : game.id);
    formData.append('Developer', game.developer);
    formData.append('Name', game.name);
    formData.append('Origin', game.origin);
    formData.append('Genre', game.genre);
    formData.append('ReleaseDate', game.releaseDate);
    formData.append('FromSeries', game.fromSeries ?? '');
    formData.append('Description', game.description)
    formData.append('DiscNumber', game.discNumber?.toString());
    formData.append('IsAdditionalFiles', game.isAdditionalFiles ? '1' : '0')
    if (game.titlePicture) 
      formData.append('TitlePicture', game.titlePicture, game.titlePicture.name);
    
    if (game.screenshots)
      this.appendFiles(formData, game.screenshots, 'Screenshots');
    
    if (game.additionalFiles)
      this.appendFiles(formData, game.additionalFiles, 'AdditionalFiles');  
    
    return formData;
  }
}
