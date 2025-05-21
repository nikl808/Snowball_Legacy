import { Component, OnInit } from '@angular/core'
import { trigger, transition, style, animate } from '@angular/animations';
import { ImportsModule } from "../../imports";
import { DataStoreService } from '../../services/data-store.service';
import { GameInfo } from '../../models/gameInfo';
import { Game } from '../../models/game';
import { GameDataService } from '../../services/game-data.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.html',
  standalone: true,
  imports: [ImportsModule],
  animations: [
    trigger('fadeIn', [
      transition(':enter', [
        style({ opacity: 0 }),
        animate('0.5s ease-in', style({ opacity: 1 }))
      ]),
      transition(':leave', [
        animate('0.5s ease-out', style({ opacity: 0 }))
      ])
    ])
  ]
})
export class Dashboard implements OnInit {
  gameId: string = '';
  gameInfo: GameInfo | undefined;
  titlePicture: any;
  screenshots: any[] = [];
  fileDownload: boolean = false;
  firstGame: boolean = false;
  lastGame: boolean = false;
  games: Game[] = [];
  currGameIndex = 0;

  constructor(private dataStore: DataStoreService,
    private  gameData: GameDataService) { }

  ngOnInit() {
    this.dataStore.gamesSubjectChanges$.subscribe(games => {
      this.games = games;
      this.updateGamePosition();
    });
    this.dataStore.activeGameSubjectChanges$.subscribe(async gameId => {
      //Clear fields
      this.clearFields();
      this.gameId = gameId;
      this.updateCurrentGameIndex(parseInt(gameId));
      this.updateGamePosition();
      this.gameInfo = await this.gameData.loadGameInfo(gameId);
      if (this.gameInfo?.id != undefined) {
        const infoId = this.gameInfo?.id;
        this.titlePicture = await this.gameData.loadGameTitlePicture(infoId);
        this.screenshots = await this.gameData.loadGameScreenshots(infoId);
      }
    });
  }

  async getAdditionalFiles() {
    this.fileDownload = true;
    await this.gameData.downloadAdditionalFiles(this.gameId);
    this.fileDownload = false;
  }

  prevGame() {
    const prevId = this.getPrevGameId();
    console.log(prevId);
    if (prevId) {
      this.dataStore._activeGameSubject.next(prevId);
    }
  }
  nextGame() {
    const nextId = this.getNextGameId();
    if (nextId) {
      this.dataStore._activeGameSubject.next(nextId);
    }
  }

  private clearFields() {
    this.gameId = '';
    this.gameInfo = undefined;
    this.titlePicture = undefined;
    this.screenshots = [];
    this.fileDownload = false;
  }

  private getPrevGameId() {
    if (this.currGameIndex > 0) return this.games[this.currGameIndex - 1].id.toString();
    return null;
  }

  private getNextGameId() {
    if (this.currGameIndex >= 0 && this.currGameIndex < this.games.length - 1) return this.games[this.currGameIndex + 1].id.toString();
    return null;
  }

  private updateCurrentGameIndex(currentId: number) {
    if (!this.games.length) this.currGameIndex = 0;
    this.currGameIndex = this.games.findIndex(g => g.id === currentId);
  }

  private updateGamePosition() {
    this.firstGame = this.currGameIndex === 0;
    this.lastGame = this.currGameIndex === this.games.length - 1 && this.games.length > 0;
  }
}
