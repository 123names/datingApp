import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root',
})
export class MemberService {
  BACKEND_URL = environment.apiURL + 'users/';
  members: Member[] = [];

  constructor(private http: HttpClient) {}

  getMembers() {
    if (this.members.length > 0) return of(this.members);
    return this.http.get<Member[]>(this.BACKEND_URL).pipe(
      map((members) => {
        this.members = members;
        return members;
      })
    );
  }

  getMember(username: string) {
    const member = this.members.find((user) => user.username === username);
    if (member !== undefined) return of(member);
    return this.http.get<Member>(this.BACKEND_URL + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.BACKEND_URL, member).pipe(
      map(() => {
        const idx = this.members.indexOf(member);
        this.members[idx] = member;
      })
    );
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.BACKEND_URL + 'set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.BACKEND_URL + 'delete-photo/' + photoId);
  }
}
