export interface GameVM {
  name: string;
  developer: string;
  genre: string;
  releaseDate: string;
  description: string;
  discNumber: number;
  isAdditionalFiles: boolean;
  titlePicture: File | undefined;
  screenshots ?: File[];
  additionalFiles ?: File[];
}
