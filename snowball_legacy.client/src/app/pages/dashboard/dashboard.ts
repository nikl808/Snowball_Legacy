import { Component, OnInit } from '@angular/core'
import { ImportsModule } from "../../imports";
import { DataStoreService } from '../../services/datastore.service';
import { ApiDataService } from '../../services/apidata.service';
import { GameInfo } from '../../models/gameInfo';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.html',
  standalone: true,
  imports: [ImportsModule]
})
export class Dashboard implements OnInit {
  gameInfo: GameInfo | undefined;
  titlePicture: any;
  constructor(private dataStore: DataStoreService, private apiData: ApiDataService) { }

  ngOnInit(): void {
    this.dataStore.activeGameSubjectChanges$.subscribe(gameId => {
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
          }
        }
      });
    })
  }
}
