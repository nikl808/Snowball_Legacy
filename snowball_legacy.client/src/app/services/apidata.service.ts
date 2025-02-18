import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core'
import { GameVM } from '../models/viewModels/game.vm';

@Injectable({
  providedIn: 'root'
})
export class ApiDataService {
  private readonly headers: HttpHeaders;
  constructor(private http: HttpClient) {
    this.headers = new HttpHeaders({ 'Access-Control-Allow-Origin': '*' });
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

    console.log(formData);

    this.http.post('/api/game', formData, { headers: this.headers }).subscribe({
      next: result => {
        console.log(result);
      },
      error: (err) => {
        console.log(err);
      }
    })
  }
}
