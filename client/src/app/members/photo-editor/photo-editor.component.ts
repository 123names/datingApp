import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { UserPhoto } from 'src/app/_models/userPhoto';
import { AccountService } from 'src/app/_services/account.service';
import { MemberService } from 'src/app/_services/member.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css'],
})
export class PhotoEditorComponent implements OnInit {
  @Input() member: Member;
  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiURL;
  user: User;

  constructor(
    private accountService: AccountService,
    private memberService: MemberService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe((user) => {
      this.user = user;
    });
  }

  ngOnInit(): void {
    this.initializeUploader();
  }

  fileOverDropZone(e: any) {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',
      authToken: 'Bearer ' + this.user.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024,
    });

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    this.uploader.onSuccessItem = (item, response, headers) => {
      if (response) {
        const newPhoto: UserPhoto = JSON.parse(response);
        this.member.userPhotos.push(newPhoto);
        if (newPhoto.isMain) {
          this.user.userMainDisplayImageUrl = newPhoto.url;
          this.member.mainPhotoUrl = newPhoto.url;
          this.accountService.setCurrentUser(this.user);
        }
      }
    };
  }

  setMainPhoto(photo: UserPhoto) {
    this.memberService.setMainPhoto(photo.id).subscribe(() => {
      this.user.userMainDisplayImageUrl = photo.url;
      this.accountService.setCurrentUser(this.user);
      this.member.mainPhotoUrl = photo.url;
      this.member.userPhotos.forEach((p) => {
        if (p.isMain) p.isMain = false;
        if (p.id === photo.id) p.isMain = true;
      });
    });
  }

  deletePhoto(photoId: number) {
    this.memberService.deletePhoto(photoId).subscribe(() => {
      this.member.userPhotos = this.member.userPhotos.filter(
        (x) => x.id !== photoId
      );
    });
  }
}
