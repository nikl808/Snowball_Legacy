import { Component, OnInit } from '@angular/core'
import { ImportsModule } from "../../imports";
import { DataStoreService } from '../../services/datastore.service';
import { ApiDataService } from '../../services/apidata.service';
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
  constructor(private dataStore: DataStoreService, private apiData: ApiDataService) { }

  ngOnInit(): void {
    this.dataStore.activeGameSubjectChanges$.subscribe(gameId => {
      this.gameId = gameId;
      this.apiData.getGameInfo(gameId).subscribe({
        next: info => {
          this.gameInfo = info
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
              next: async screens => {
                const zip = new JSZip();
                this.screenshots = [];
                const extractedFiles = await zip.loadAsync(screens);
                extractedFiles.forEach(async (relativePath, file) => {
                  file.async('base64').then((content) => {
                    this.screenshots.push(content);
                  });
                });
              }
            });
          }
        }
      });
    })
  }

  getAdditionalFiles() {
    this.apiData.getAdditionalGameFiles(this.gameId).subscribe({
      next: zip => {
        fs.saveAs(zip, "files.zip");
      }
    });
  }
}
