import { Component, EventEmitter, Input, Output } from '@angular/core'
import { Game } from '../../../models/game';
import { ImportsModule } from "../../../imports";

@Component({
  selector: 'app-games-list',
  standalone: true,
  templateUrl: './app.gameslist.html',
  imports: [ImportsModule],
  styles: [`
      .listbox-items {
        padding: 0;
        margin: 0;
        list-style: none;
      }

      .listbox-item {
        display: flex;
        align-items:
        center; color:
        var(--text-color);
        position: relative;
        padding: 0.75rem 1rem;
        border-radius: var(--content-border-radius);
        transition:background-color var(--element-transition-duration),
          box-shadow var(--element-transition-duration);
          outline: 0 none;
          padding: 10px;
          cursor: pointer;
        }
        .listbox-item:hover { background-color: var(--surface-hover);}
        .activeitem { font-weight: 700; color: var(--primary-color);}
  `]
})
export class AppGamesList {
  @Input() items: Game[] = [];
  @Output() itemSelected: EventEmitter<Game> = new EventEmitter();
  activeGame: number = 0;


  onItemSelect(event: Game, index: number) {
    if (this.activeGame != index) {
      this.activeGame = index;
      this.itemSelected.emit(event);
    }
  }

  trackById(index: number, item: Game): number {
    return item.id;
  }
}
