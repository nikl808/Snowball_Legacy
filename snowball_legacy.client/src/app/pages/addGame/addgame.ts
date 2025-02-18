import { Component, OnInit } from "@angular/core";
import { ApiDataService } from "../../services/apidata.service";
import { ImportsModule } from "../../imports";
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AppFileupload } from "../../layout/component/app.fileupload/app.fileupload"

@Component({
  selector: 'app-add-game',
  templateUrl: './addgame.html',
  standalone: true,
  imports: [ImportsModule, AppFileupload],
})
export class AddGame implements OnInit {

  addGameForm!: FormGroup;
  showAddDlg: boolean = false;
  submitted: boolean = false;
  floatValue: any = null;
 

  get controls() {
    return this.addGameForm.controls;
  }

  constructor(private dataService: ApiDataService) { }

  ngOnInit(): void {
    this.addGameForm = new FormGroup({
      gameName: new FormControl('', [Validators.required])
    })
  }

  showAddDialog() {
    this.showAddDlg = true;
  }

  addGame() {
    this.submitted = true;
    if (this.controls.invalid) {
      return;
    }
  }

  cancel() {
    this.submitted = false;
    this.addGameForm.reset();
    this.showAddDlg = false;
  }
}
