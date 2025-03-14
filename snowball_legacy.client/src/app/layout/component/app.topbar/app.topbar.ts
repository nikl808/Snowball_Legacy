import { Component, OnInit } from '@angular/core';
import { MenuItem, ConfirmationService } from 'primeng/api';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { StyleClassModule } from 'primeng/styleclass';
import { AppConfigurator } from '../app.configurator/app.configurator';
import { LayoutService } from '../../service/layout.service';
import { AddGame } from '../../../pages/addGame/addgame';
import { EditGame } from '../../../pages/editGame/editgame'
import { DividerModule } from 'primeng/divider';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ApiDataService } from '../../../services/api-data.service';
import { DataStoreService } from '../../../services/data-store.service';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [RouterModule, CommonModule, StyleClassModule, DividerModule, AppConfigurator, AddGame, EditGame,
    ConfirmDialogModule],
  providers: [ConfirmationService],
  templateUrl: './app.topbar.html'
})
export class AppTopbar implements OnInit {
  items!: MenuItem[];
  currentGameId: string = '';
  constructor(public layoutService: LayoutService,
    public dataStore: DataStoreService,
    private confirmationService: ConfirmationService,
    private apiData: ApiDataService) { }

  ngOnInit(): void {
    this.dataStore.activeGameSubjectChanges$.subscribe(id => this.currentGameId = id);
  }

  deleteGame(event: Event) {
    this.confirmationService.confirm({
      target: event.target as EventTarget,
      message: 'Вы уверены что хотите продолжить?',
      header: 'Удалить?',
      closable: true,
      closeOnEscape: true,
      icon: 'pi pi-info-circle',
      rejectLabel: 'Отмена',
      rejectButtonProps: {
        label: 'Отмена',
        severity: 'secondary'
      },
      acceptButtonProps: {
        label: 'Удалить',
        severity: 'Danger'
      },
      accept: () => {
        this.apiData.deleteGame(this.currentGameId);
      },
      reject: () => { },
    })
  }

  toggleDarkMode() {
    this.layoutService.layoutConfig.update((state) => ({ ...state, darkTheme: !state.darkTheme }));
  }
}
