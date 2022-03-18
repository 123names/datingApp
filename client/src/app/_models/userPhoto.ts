export interface UserPhoto {
  id: number;
  url: string;
  isMain: boolean;
  isApproved: boolean;
  username?: string;
}
