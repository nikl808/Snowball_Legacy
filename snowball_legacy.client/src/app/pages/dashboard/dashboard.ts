import { Component, OnInit } from '@angular/core'
import { ImportsModule } from "../../imports";
import { DataStoreService } from '../../services/data-store.service';
import { ApiDataService } from '../../services/api-data.service';
import { GameInfo } from '../../models/gameInfo';
import JSZip from 'jszip';
import fs from 'file-saver';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.html',
  standalone: true,
  imports: [ImportsModule]
})
export class Dashboard implements OnInit {
  gameId: string = '';
  gameInfo: GameInfo | undefined;
  titlePicture: any;
  oneScreen: any;
  screenshots: any[] = [];
  fileDownload: boolean = false;
  
  constructor(private dataStore: DataStoreService, private apiData: ApiDataService) {}

  ngOnInit(): void {
    this.dataStore.activeGameSubjectChanges$.subscribe(gameId => {
      //Clear fields
      this.clearFields();

      this.gameId = gameId;
      this.apiData.getGameInfo(gameId).subscribe({
        next: info => {
          this.gameInfo = info;
          if (this.gameInfo?.id != undefined) {
            this.apiData.getGameTitlePicture(this.gameInfo?.id).subscribe({
              next: blob => {
                const reader = new FileReader();
                const setTitlePicture = (result: any) => {
                  this.titlePicture = result;
                }
                reader.readAsDataURL(blob);
                reader.onloadend = () => {
                  // result includes identifier 'data:image/jpeg;base64,' plus the base64 data
                  setTitlePicture(reader.result);
                }
              }
            });

            this.apiData.getGameScreenshots(this.gameInfo?.id).subscribe({
              next: async data => {                
                await this.setScreenshots(data);
              }
            });
          }
        }
      });
    });
  }

  async setScreenshots(archive: Blob) {
    const zip = new JSZip();
    this.screenshots = [];
    zip.loadAsync(archive).then(extracted => {
      Object.keys(extracted.files).forEach((filename) => {
        zip.file(filename)?.async('base64').then((content) => {
          this.screenshots.push(content);
        });
      });
    });
  }

  getAdditionalFiles() {
    this.fileDownload = true;
    this.apiData.getAdditionalGameFiles(this.gameId).subscribe({
      next: zip => {
        fs.saveAs(zip, "files.zip");
      },
      complete: () => { this.fileDownload = !this.fileDownload; }
    });
  }

  private clearFields() {
    this.gameId = '';
    this.gameInfo = undefined;
    this.titlePicture = undefined;
    this.screenshots = [];
    this.fileDownload = false;
  }
}
