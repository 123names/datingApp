export interface Message {
  id: number;
  senderId: number;
  senderUsername: string;
  senderMainPhotoUrl: string;
  recipientId: number;
  recipientUsername: string;
  recipientMainPhotoUrl: string;
  content: string;
  dateRead?: Date;
  messageSend: Date;
}
