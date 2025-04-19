import { Component, OnInit } from '@angular/core';
import { MenuItem, ConfirmationService, MessageService } from 'primeng/api';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { StyleClassModule } from 'primeng/styleclass';
import { LayoutService } from '../../service/layout.service';
import { AddGame } from '../../../pages/addGame/addgame';
import { EditGame } from '../../../pages/editGame/editgame'
import { DividerModule } from 'primeng/divider';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ApiDataService } from '../../../services/api-data.service';
import { DataStoreService } from '../../../services/data-store.service';
import { HttpEventType } from '@angular/common/http';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [RouterModule, CommonModule, StyleClassModule, DividerModule, AddGame, EditGame,
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
    private apiData: ApiDataService,
    private messageService: MessageService) { }

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
        this.apiData.deleteGame(this.currentGameId).subscribe({
          next: event => {
            if (event.type === HttpEventType.Response) {
              if (event.ok)
                this.messageService.add({
                  severity: 'success',
                  summary: 'Выполнено',
                  detail: 'Игра удалена',
                  sticky: true
                });
              this.dataStore._updateGameListSubject.next(true);
            }
          },
          error: (err) => {
            this.messageService.add({
              severity: 'error',
              summary: 'Ошибка',
              detail: err.message,
              sticky: true
            });
            console.log(err);
          }

        });
      },
      reject: () => { },
    })
  }

  toggleDarkMode() {
    this.layoutService.layoutConfig.update((state) => ({ ...state, darkTheme: !state.darkTheme }));
  }
}
