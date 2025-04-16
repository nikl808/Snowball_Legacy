import { Game } from "./Game";

export interface GameFile {
   id: number;
   gameId: number;
   game?: Game;
   file? : any[];
}
