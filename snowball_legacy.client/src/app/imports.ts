import { NgModule } from "@angular/core";
import { CommonModule } from '@angular/common';
import { ButtonModule } from "primeng/button";
import { DialogModule } from "primeng/dialog";
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FloatLabelModule } from 'primeng/floatlabel';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { FileUploadModule } from 'primeng/fileupload';
import { TextareaModule } from 'primeng/textarea';
import { ProgressBarModule } from 'primeng/progressbar';
import { BadgeModule } from 'primeng/badge';
import { OverlayBadgeModule } from 'primeng/overlaybadge';
import { InputNumberModule } from 'primeng/inputnumber';
import { DropdownModule } from 'primeng/dropdown'
import { InputMaskModule } from 'primeng/inputmask';
import { CardModule } from 'primeng/card';
import { ImageModule } from 'primeng/image';

@NgModule({
  imports: [
    FormsModule, InputTextModule, MessageModule,
    ButtonModule, DialogModule, ReactiveFormsModule,
    FloatLabelModule, CommonModule, FileUploadModule,
    TextareaModule, ProgressBarModule, BadgeModule, OverlayBadgeModule,
    InputNumberModule, DropdownModule, InputMaskModule, CardModule,
    ImageModule
  ],
  exports: [
    FormsModule, InputTextModule, MessageModule,
    ButtonModule, DialogModule, ReactiveFormsModule,
    FloatLabelModule, CommonModule, FileUploadModule,
    TextareaModule, ProgressBarModule, BadgeModule, OverlayBadgeModule,
    InputNumberModule, DropdownModule, InputMaskModule, CardModule,
    ImageModule
  ],
  providers: []
})
export class ImportsModule { }
