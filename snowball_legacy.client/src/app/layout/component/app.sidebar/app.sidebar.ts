import { Component, OnInit } from '@angular/core'
import { ImportsModule } from "../../../imports";
import { MenuItem } from 'primeng/api';
import { ApiDataService } from '../../../services/api-data.service';
import { AppMenuitem } from '../app.menuitem/app.menuitem';
import { DataStoreService } from '../../../services/data-store.service';

@Component({
  selector: "app-sidebar",
  templateUrl: "./app.sidebar.html",
  standalone: true,
  imports: [ImportsModule, AppMenuitem]
})
export class AppSidebar implements OnInit {
  model: MenuItem[] = [];

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
        let menuItems: MenuItem[] = [];
        result.forEach(game => {
          menuItems.push({ id: game.id.toString(), label: game.name, routerLink: ['/'] });
        });
        this.model.push({ items: menuItems })
      }
    });
  }
}
