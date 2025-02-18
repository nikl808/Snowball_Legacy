import { Component } from "@angular/core";
import { ImportsModule } from "../../../imports";
import { PrimeNG } from "primeng/config";


@Component({
  selector: "app-fileupload",
  templateUrl: "./app.fileupload.html",
  standalone: true,
  imports: [ImportsModule]
})
export class AppFileupload {
  uploadedFiles: any[] = [];
  totalSize: number = 0;
  totalSizePercent: number = 0;

  constructor(private config: PrimeNG) { }

  choose(event: any, callback: () => void) {
    callback();
  }

  onRemoveTemplatingFile(event: any, file: { size: number; }, removeFileCallback: (arg0: any, arg1: any) => void, index: any) {
    removeFileCallback(event, index);
    this.totalSize -= parseInt(this.formatSize(file.size));
    this.totalSizePercent = this.totalSize / 10;
  }

  formatSize(bytes: number) {
    const k = 1024;
    const dm = 3;
    const sizes = this.config.translation.fileSizeTypes;
    if (sizes != undefined) {
      if (bytes === 0) {
        return `0 ${sizes[0]}`;
      }

      const i = Math.floor(Math.log(bytes) / Math.log(k));
      const formattedSize = parseFloat((bytes / Math.pow(k, i)).toFixed(dm));

      return `${formattedSize} ${sizes[i]}`;
    }
    return '';
  }

  onSelectedFiles(event: { currentFiles: any; }) {
    this.uploadedFiles = event.currentFiles;
    this.uploadedFiles.forEach((file: { size: number; }) => {
      this.totalSize += parseInt(this.formatSize(file.size));
    });
    this.totalSizePercent = this.totalSize / 10;
  }
}
