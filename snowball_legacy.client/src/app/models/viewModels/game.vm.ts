export interface GameVM {
  id: string;
  name: string;
  origin: string;
  developer: string;
  genre: string;
  releaseDate: string;
  description: string;
  fromSeries?: string;
  discNumber: number;
  isAdditionalFiles: boolean;
  titlePicture: File | undefined;
  screenshots ?: File[];
  additionalFiles ?: File[];
}
