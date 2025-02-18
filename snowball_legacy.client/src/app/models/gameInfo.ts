import { Game } from "./Game";
import { GamePicture } from "./gamePicture";

export interface GameInfo {
  id: number;
  gameId: number;
  diskNumber?: number;
  description?: string;
  game?: Game;
  titlePicture?: GamePicture;
  screenShoots?: GamePicture[];
}
