import { GameInfo } from "./gameInfo";

export interface GamePicture {
   id: number;
   gameInfoId: number;
   gameInfo?: GameInfo;
   picture?: any[];
}
