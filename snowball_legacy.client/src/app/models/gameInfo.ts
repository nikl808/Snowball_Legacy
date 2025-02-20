import { Game } from "./game";
import { GamePicture } from "./gamePicture";

export interface GameInfo {
  id: number;
  name ?: string;
  genre: string;
  releaseDate: Date;
  diskNumber ?: number;
  description ?: string;
}
