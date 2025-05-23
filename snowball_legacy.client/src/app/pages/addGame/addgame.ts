import { Component, OnInit, ViewChild } from "@angular/core";
import { ApiDataService } from "../../services/api-data.service";
import { ImportsModule } from "../../imports";
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { GameVM } from "../../models/viewModels/game.vm";
import { HttpEventType } from "@angular/common/http";
import { MessageService } from "primeng/api";
import { DataStoreService } from "../../services/data-store.service";
import { FileUpload } from "primeng/fileupload";
import { TranslateService } from "@ngx-translate/core";

@Component({
  selector: 'app-add-game',
  templateUrl: './addgame.html',
  standalone: true,
  imports: [ImportsModule],
  styles: []
})
export class AddGame implements OnInit {
  @ViewChild('title') titleChild: FileUpload | undefined;
  @ViewChild('screens') screenChild: FileUpload | undefined;
  @ViewChild('additional') additionalChild: FileUpload | undefined;

  private msgGameAdded: string = '';
  private msgSuccess: string = '';
  private msgError: string = '';

  addGameForm!: FormGroup;
  showAddDlg: boolean = false;
  submitted: boolean = false;
  genres: string[] = [];
  titlePic: File | undefined;
  screenshots: File[] = [];
  additionalFiles: File[] = [];
  loading: boolean = false;

 
  get controls() {
    return this.addGameForm.controls;
  }

  constructor(private apiData: ApiDataService, private dataStore: DataStoreService,
    private messageService: MessageService, private translate: TranslateService) { }

  ngOnInit(): void {
    this.translate.get('common.gameAdded').subscribe((res: any) => { this.msgGameAdded = res });
    this.translate.get('common.success').subscribe((res: any) => { this.msgSuccess = res });
    this.translate.get('common.error').subscribe((res: any) => { this.msgError = res });
    this.apiData
      .getGamesGenresFromJson().subscribe({
        next: values => {
          this.genres = values.genres;
        }
      });
    this.addGameForm = new FormGroup({
      gameName: new FormControl('', [Validators.required]),
      originName: new FormControl('', [Validators.required]),
      developer: new FormControl('', [Validators.required]),
      gamedescr: new FormControl('', [Validators.required]),
      releasedate: new FormControl('', [Validators.required]),
      discnum: new FormControl('', [Validators.required]),
      fromSeries: new FormControl(''),
      selectGenre: new FormControl(this.genres[0], [Validators.required])
    });
  }

  showAddDialog() {
    this.showAddDlg = true;
  }

  onSelectedFile(type: string, event: any) {
    const action: { [key: string]: (Function) } = {
      "title": () => { this.titlePic = event.currentFiles[0] },
      "screen": () => { this.screenshots = event.currentFiles },
      "additional": () => { this.additionalFiles = event.currentFiles },
    };
    const actionMethod = action[type];
    if (actionMethod) actionMethod();
  }

 
  addGame() {
    this.submitted = true;
    if (this.controls.invalid) {
      return;
    }
    this.loading = true;
    let newGame: GameVM = {
      id: '',
      name: this.addGameForm.value.gameName ?? '',
      origin: this.addGameForm.value.originName ?? '',
      developer: this.addGameForm.value.developer ?? '',
      genre: this.addGameForm.value.selectGenre,
      releaseDate: this.addGameForm.value.releasedate,
      description: this.addGameForm.value.gamedescr ?? '',
      fromSeries: this.addGameForm.value.fromSeries ?? '',
      discNumber: this.addGameForm.value.discnum ?? '',
      isAdditionalFiles: this.additionalFiles.length > 0,
      titlePicture: this.titlePic,
      screenshots: this.screenshots ?? [],
      additionalFiles: this.additionalFiles ?? [],
    };

    this.apiData.addGame(newGame).subscribe({
      next: event => {
        if (event.type === HttpEventType.Response) {
          this.loading = !event.ok;
          if (event.ok)
            this.messageService.add({
              severity: 'success',
              summary: this.msgSuccess,
              detail: this.msgGameAdded,
              sticky: true
            });
          this.dataStore._updateGameListSubject.next(true);
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
    this.addGameForm.reset();
    this.titleChild?.clear();
    this.screenChild?.clear();
    this.additionalChild?.clear();
    this.showAddDlg = false;
  }

  dialogHide(event: any) {
    this.titleChild?.clear();
    this.screenChild?.clear();
    this.additionalChild?.clear();
    this.addGameForm.reset();
  }
}
