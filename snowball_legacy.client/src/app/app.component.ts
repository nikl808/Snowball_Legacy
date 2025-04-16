import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { PrimeNG } from 'primeng/config';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterModule, TranslateModule],
  template: `<router-outlet></router-outlet>`
})
export class AppComponent {
  constructor(private config: PrimeNG, private translateService: TranslateService) {
    this.translateService.addLangs(['ru', 'en']);
    this.translateService.setDefaultLang('ru');
    this.translateService.use('ru');
    this.translateService.get('primeng').subscribe(res => this.config.setTranslation(res));
  }
}
