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
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { TranslateService } from "@ngx-translate/core";

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [RouterModule, CommonModule, StyleClassModule, DividerModule, AddGame, EditGame,
    ConfirmDialogModule],
  providers: [ConfirmationService],
  templateUrl: './app.topbar.html'
})
export class AppTopbar implements OnInit {
  private destroy$ = new Subject<void>();
  private msgConfirm: string = '';
  private msgDelete: string = '';
  private msgDelQuestion: string = '';
  private msgGameDeleted: string = '';
  private msgCancel: string = '';
  private msgSuccess: string = '';
  private msgError: string = '';

  items!: MenuItem[];
  currentGameId: string = '';
  constructor(public layoutService: LayoutService,
    public dataStore: DataStoreService,
    private confirmationService: ConfirmationService,
    private apiData: ApiDataService,
    private messageService: MessageService,
    private translate: TranslateService) { }

  ngOnInit(): void {
    this.translate.get('common.confirm').subscribe((res: any) => { this.msgConfirm = res });
    this.translate.get('common.delete').subscribe((res: any) => { this.msgDelete = res });
    this.translate.get('common.delQuestion').subscribe((res: any) => { this.msgDelQuestion = res });
    this.translate.get('common.gameDeleted').subscribe((res: any) => { this.msgGameDeleted = res });
    this.translate.get('common.success').subscribe((res: any) => { this.msgSuccess = res });
    this.translate.get('common.cancel').subscribe((res: any) => { this.msgCancel = res });
    this.translate.get('common.error').subscribe((res: any) => { this.msgError = res });
    this.dataStore.activeGameSubjectChanges$
      .pipe(takeUntil(this.destroy$))
      .subscribe(id => this.currentGameId = id);
  }

  deleteGame(event: Event) {
    this.confirmationService.confirm({
      target: event.target as EventTarget,
      message: this.msgConfirm,
      header: this.msgDelQuestion,
      closable: true,
      closeOnEscape: true,
      icon: 'pi pi-info-circle',
      rejectLabel: this.msgCancel,
      rejectButtonProps: {
        label: this.msgCancel,
        severity: 'secondary'
      },
      acceptButtonProps: {
        label: this.msgDelete,
        severity: 'Danger'
      },
      accept: () => {
        this.apiData.deleteGame(this.currentGameId).subscribe({
          next: event => {
            if (event.type === HttpEventType.Response) {
              if (event.ok)
                this.messageService.add({
                  severity: 'success',
                  summary: this.msgSuccess,
                  detail: this.msgGameDeleted,
                  sticky: true
                });
              this.dataStore._updateGameListSubject.next(true);
            }
          },
          error: (err) => {
            this.messageService.add({
              severity: 'error',
              summary: this.msgError,
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

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
