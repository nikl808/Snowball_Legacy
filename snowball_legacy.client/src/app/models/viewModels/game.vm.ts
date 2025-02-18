export interface GameVM {
  name: string;
  genre: string;
  releaseDate: string;
  description: string;
  discNumber: number;
  titlePicture: File | undefined;
  screenshots ?: File[];
  additionalFiles ?: File[];
}
