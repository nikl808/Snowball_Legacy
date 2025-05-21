import { Injectable } from '@angular/core';
import { DataStoreService } from './data-store.service';
import { ApiDataService } from './api-data.service';
import { firstValueFrom } from 'rxjs';
import JSZip from 'jszip';
import fs from 'file-saver';

@Injectable({
  providedIn: 'root'
})
export class GameDataService {

  constructor(private dataStore: DataStoreService, private apiData: ApiDataService) { }

  async loadGameInfo(gameId: string) {
    return await firstValueFrom(this.apiData.getGameInfo(gameId));
  }

  async loadGameTitlePicture(gameInfoId: number) {
    const blob = await firstValueFrom(this.apiData.getGameTitlePicture(gameInfoId));
    return await this.blobToDataUrl(blob);
  }

  async loadGameScreenshots(gameInfoId: number) {
    const scrRaw = await firstValueFrom(this.apiData.getGameScreenshots(gameInfoId));
    return await this.setScreenshots(scrRaw);
  }

  async downloadAdditionalFiles(gameId: string) {
    const zip = await firstValueFrom(this.apiData.getAdditionalGameFiles(gameId));
    fs.saveAs(zip, "files.zip");
  }

  private blobToDataUrl(blob: Blob): Promise<string | ArrayBuffer | null> {
    return new Promise((resolve) => {
      const reader = new FileReader();
      reader.readAsDataURL(blob);
      reader.onloadend = () => resolve(reader.result);
    });
  }

  private async setScreenshots(archive: Blob) {
    const zip = new JSZip();
    const screenshots: any = [];
    zip.loadAsync(archive).then(extracted => {
      Object.keys(extracted.files).forEach((filename) => {
        zip.file(filename)?.async('base64').then((content) => {
          screenshots.push(content);
        });
      });
    });
    return screenshots;
  }
}
