import { NgModule } from "@angular/core";
import { TranslateModule } from '@ngx-translate/core';
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
import { PanelModule } from 'primeng/panel';
import { ToastModule } from 'primeng/toast';
import { MessageService } from "primeng/api";
import { ListboxModule } from 'primeng/listbox';
import { SelectModule } from 'primeng/select';
import { SkeletonModule } from 'primeng/skeleton';

@NgModule({
  imports: [
    TranslateModule, FormsModule, InputTextModule,
    MessageModule, ButtonModule, DialogModule,
    ReactiveFormsModule, FloatLabelModule, CommonModule,
    FileUploadModule, TextareaModule, ProgressBarModule,
    BadgeModule, OverlayBadgeModule, InputNumberModule,
    DropdownModule, InputMaskModule, CardModule,
    ImageModule, PanelModule, ToastModule, ListboxModule,
    SelectModule, SkeletonModule
  ],
  exports: [
    TranslateModule, FormsModule, InputTextModule,
    MessageModule, ButtonModule, DialogModule,
    ReactiveFormsModule, FloatLabelModule, CommonModule,
    FileUploadModule, TextareaModule, ProgressBarModule,
    BadgeModule, OverlayBadgeModule, InputNumberModule,
    DropdownModule, InputMaskModule, CardModule,
    ImageModule, PanelModule, ToastModule, ListboxModule,
    SelectModule, SkeletonModule
  ],
  providers: [MessageService]
})
export class ImportsModule { }
