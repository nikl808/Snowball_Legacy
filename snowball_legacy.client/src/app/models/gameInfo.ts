export interface GameInfo{
  id: number;
  name?: string;
  origin?: string;
  genre: string;
  releaseDate: Date;
  discNumber ?: number;
  description?: string;
  fromSeries?: string;
  developer: string;
  isAdditionalFiles: boolean;
}
