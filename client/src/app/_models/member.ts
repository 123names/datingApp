import { UserPhoto } from "./userPhoto";

export interface Member {
  id: number;
  username: string;
  mainPhotoUrl: string;
  age: number;
  knownAs: string;
  userCreatedOn: Date;
  lastActive: Date;
  gender: string;
  introduction: string;
  lookingFor: string;
  interests: string;
  city: string;
  country: string;
  userPhotos: UserPhoto[];
}


