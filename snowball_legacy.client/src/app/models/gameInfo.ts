export interface GameInfo{
  id: number;
  name ?: string;
  genre: string;
  releaseDate: Date;
  discNumber ?: number;
  description?: string;
  developer: string;
  isAdditionalFiles: boolean;
}
