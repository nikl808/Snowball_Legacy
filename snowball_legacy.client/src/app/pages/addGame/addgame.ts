import { Component, OnInit } from "@angular/core";
import { ApiDataService } from "../../services/api-data.service";
import { ImportsModule } from "../../imports";
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { GameVM } from "../../models/viewModels/game.vm";

@Component({
  selector: 'app-add-game',
  templateUrl: './addgame.html',
  standalone: true,
  imports: [ImportsModule],
})
export class AddGame implements OnInit {
  addGameForm!: FormGroup;
  showAddDlg: boolean = false;
  submitted: boolean = false;

  genres: string[] = ["Стратегия"];
  selectedGenre: string = this.genres[0];

  titlePic: File | undefined;
  screenshots: File[] = [];
  additionalFiles: File[] = [];
 
  get controls() {
    return this.addGameForm.controls;
  }

  constructor(private apiData: ApiDataService) { }

  ngOnInit(): void {
    this.addGameForm = new FormGroup({
      gameName: new FormControl('', [Validators.required]),
      developer: new FormControl('', [Validators.required]),
      gamedescr: new FormControl('', [Validators.required]),
      releasedate: new FormControl('', [Validators.required]),
      discnum: new FormControl('', [Validators.required])
    })
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
    let newGame: GameVM = {
      id: '',
      name: this.addGameForm.value.gameName ?? '',
      developer: this.addGameForm.value.developer ?? '',
      genre: this.selectedGenre,
      releaseDate: this.addGameForm.value.releasedate,
      description: this.addGameForm.value.gamedescr ?? '',
      discNumber: this.addGameForm.value.discnum ?? '',
      isAdditionalFiles: this.additionalFiles.length > 0,
      titlePicture: this.titlePic,
      screenshots: this.screenshots ?? [],
      additionalFiles: this.additionalFiles ?? [],
    };

    this.apiData.addGame(newGame);
  }

  cancel() {
    this.submitted = false;
    this.addGameForm.reset();
    this.showAddDlg = false;
  }
}
