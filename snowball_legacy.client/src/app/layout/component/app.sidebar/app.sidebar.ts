import { Component, OnInit } from '@angular/core'
import { ImportsModule } from "../../../imports";
import { MenuItem } from 'primeng/api';
import { ApiDataService } from '../../../services/apidata.service';
import { AppMenuitem } from './../app.menuitem';

@Component({
  selector: "app-sidebar",
  templateUrl: "./app.sidebar.html",
  standalone: true,
  imports: [ImportsModule, AppMenuitem]
})
export class AppSidebar implements OnInit {
  model: MenuItem[] = [];
   
  constructor(private dataService: ApiDataService) { }

  ngOnInit(): void {
    this.dataService.getGames().subscribe({
      next: result => {
        console.log(result);
        let menuItems: MenuItem[] = [];
        result.forEach(game => {
          menuItems.push({ id: game.id.toString(), label: game.name, routerLink: ['/'] });
        });
        this.model.push({ items: menuItems })
      }
    });
  }
}
