import { Component, Input, ViewChild } from "@angular/core";
import { ImportsModule } from "../../imports";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { ApiDataService } from "../../services/api-data.service";
import { FileUpload } from "primeng/fileupload";
import JSZip from 'jszip';
import { GameVM } from "../../models/viewModels/game.vm";
import { DataStoreService } from "../../services/data-store.service";
import { MessageService } from "primeng/api";
import { HttpEventType } from "@angular/common/http";
import { TranslateService } from "@ngx-translate/core";

@Component({
  selector: 'app-edit-game',
  templateUrl: './editgame.html',
  standalone: true,
  imports: [ImportsModule],
  providers: [ApiDataService],
})
export class EditGame {
  @Input() gameId: string = '';
  @ViewChild('title') titleFile: FileUpload | undefined;
  @ViewChild('screens') screenFiles: FileUpload | undefined;
  @ViewChild('additional') additionalFiles: FileUpload | undefined;

  private msgGameUpdated: string = '';
  private msgSuccess: string = '';
  private msgError: string = '';

  editGameForm!: FormGroup;
  showEdit: boolean = false;
  submitted: boolean = false;
  loading: boolean = false;
  genres: string[] = [];

  get controls() {
    return this.editGameForm.controls;
  }

  constructor(public dataStore: DataStoreService, private apiData: ApiDataService,
    private messageService: MessageService, private translate: TranslateService) {
    this.translate.get('common.gameUpdated').subscribe((res: any) => { this.msgGameUpdated = res });
    this.translate.get('common.success').subscribe((res: any) => { this.msgSuccess = res });
    this.translate.get('common.error').subscribe((res: any) => { this.msgError = res });
    this.apiData
      .getGamesGenresFromJson().subscribe({
        next: values => {
          this.genres = values.genres;
        }
      });
    this.editGameForm = new FormGroup({
      gameName: new FormControl('', [Validators.required]),
      developer: new FormControl('', [Validators.required]),
      gamedescr: new FormControl('', [Validators.required]),
      releasedate: new FormControl('', [Validators.required]),
      discnum: new FormControl('', [Validators.required]),
      selectGenre: new FormControl('')
    })
  }

  showEditDialog() {
    this.apiData.getGameInfo(this.gameId).subscribe(async info => {
      let index = this.genres.findIndex((item: string) => item === info.genre);
      this.editGameForm.setValue({
        gameName: info.name,
        developer: info.developer,
        gamedescr: info.description,
        releasedate: new Date(info.releaseDate).toLocaleDateString(),
        discnum: info.discNumber,
        selectGenre: this.genres[index]
      });
      this.setTitlePicture(info.id);
      this.setAdditionalFiles(this.gameId);
      await this.setScreenshots(info.id);
    });
    this.showEdit = true;
  }

  editGame() {
    this.submitted = true;
    if (this.controls.invalid) {
      return;
    }
    this.loading = true;
    let updateGame: GameVM = {
      id: this.gameId,
      name: this.editGameForm.value.gameName ?? '',
      developer: this.editGameForm.value.developer ?? '',
      genre: this.editGameForm.value.selectGenre,
      releaseDate: this.editGameForm.value.releasedate,
      description: this.editGameForm.value.gamedescr ?? '',
      discNumber: this.editGameForm.value.discnum ?? '',
      isAdditionalFiles: this.additionalFiles != undefined ? this.additionalFiles?.files.length > 0 : false,
      titlePicture: this.titleFile?.files[0],
      screenshots: this.screenFiles?.files ?? [],
      additionalFiles: this.additionalFiles?.files ?? [],
    };
    this.apiData.updateGame(updateGame).subscribe({
      next: event => {
        if (event.type === HttpEventType.Response) {
          this.loading = !event.ok;
          if (event.ok)
            this.messageService.add({
              severity: 'success',
              summary: this.msgSuccess,
              detail: this.msgGameUpdated,
              sticky: true
            });
          this.dataStore._activeGameSubject.next(this.gameId);
        }
      },
      error: (err) => {
        this.loading = false
        this.messageService.add({
          severity: 'error',
          summary: this.msgError,
          detail: err.message,
          sticky: true
        });
        console.log(err);
      }
    });
  }

  cancel() {
    this.submitted = false;
    this.editGameForm.reset();
    this.showEdit = false;
  }

  private setTitlePicture(gameInfoId: number) {
    this.apiData.getGameTitlePicture(gameInfoId).subscribe({
      next: blob => {
        var file = new File([blob], "title.jpg", { type: "image/jpeg", lastModified: Date.now() });
        this.titleFile?.clear();
        this.titleFile?.files.push(file);
      }
    });
  }

  private async setScreenshots(gameInfoId: number) {
    this.apiData.getGameScreenshots(gameInfoId).subscribe({
      next: async screens => {
        const zip = new JSZip();
        let files = []
        const extractedFiles = await zip.loadAsync(screens);
        for (const key of Object.keys(extractedFiles.files)) {
          const fileData = await extractedFiles.files[key].async('blob');
          var file = new File([fileData], key, { type: "image/jpeg", lastModified: Date.now() });
          files.push(file);
        }
        this.screenFiles?.clear();
        this.screenFiles?.files.push(...files);
      }
    });
  }

  private setAdditionalFiles(gameId: string) {
    this.apiData.getAdditionalGameFiles(gameId).subscribe({
      next: zip => {
        var file = new File([zip], "files.zip", { type: "application/zip", lastModified: Date.now() });
        this.additionalFiles?.clear();
        this.additionalFiles?.files.push(file);
      }
    });
  }
}
