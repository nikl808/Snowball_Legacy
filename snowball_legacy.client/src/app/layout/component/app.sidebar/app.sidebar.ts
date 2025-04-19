import { Component, OnInit } from '@angular/core'
import { ImportsModule } from "../../../imports";
import { ApiDataService } from '../../../services/api-data.service';
import { DataStoreService } from '../../../services/data-store.service';
import { Game } from '../../../models/game';
import { AppGamesList } from '../app.gameslist/app.gameslist';

@Component({
  selector: "app-sidebar",
  templateUrl: "./app.sidebar.html",
  standalone: true,
  imports: [ImportsModule, AppGamesList]
})
export class AppSidebar implements OnInit {
  games: Game[] = [];

  constructor(private dataService: ApiDataService, private dataStore: DataStoreService) { }

  ngOnInit(): void {
    this.setGameList();
    this.dataStore.updateGameListSubjectChanges$.subscribe(val => {
      this.setGameList();
    });
  }

  setGameList() {
    this.dataService.getGames().subscribe({
      next: result => {
        this.games = result;
        if (this.games.length > 0)
          this.dataStore._activeGameSubject.next(this.games[0].id.toString() ?? '');
      }
    });
  }

  onGameChanged(event: Game) {
      this.dataStore._activeGameSubject.next(event.id.toString() ?? '');
  }
}
