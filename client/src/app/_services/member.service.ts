import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import {
  getFilteredPaginatedResult,
  getPaginationHeaders,
} from './paginationHelper';

@Injectable({
  providedIn: 'root',
})
export class MemberService {
  BACKEND_URL = environment.apiURL;
  members: Member[] = [];
  membersCache = new Map();
  user: User;
  userParams: UserParams;

  constructor(
    private http: HttpClient,
    private accountService: AccountService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe((user) => {
      this.user = user;
      this.userParams = new UserParams(user);
    });
  }

  getUserParams() {
    return this.userParams;
  }
  setUserParams(params: UserParams) {
    this.userParams = params;
  }
  resetUserParams() {
    this.userParams = new UserParams(this.user);
    return this.userParams;
  }

  getMembers(userParams: UserParams) {
    var responseCached = this.membersCache.get(
      Object.values(userParams).join('-')
    );
    if (responseCached) {
      return of(responseCached);
    }
    let params = getPaginationHeaders(
      userParams.pageNumber,
      userParams.pageSize
    );
    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    return getFilteredPaginatedResult<Member[]>(
      this.BACKEND_URL + 'users/',
      params,
      this.http
    ).pipe(
      map((response) => {
        this.membersCache.set(Object.values(userParams).join('-'), response);
        return response;
      })
    );
  }

  getMember(username: string) {
    const member = [...this.membersCache.values()]
      .reduce((arr, currElem) => arr.concat(currElem.result), [])
      .find((member: Member) => member.username === username);
    if (member) {
      return of(member);
    }
    return this.http.get<Member>(this.BACKEND_URL + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.BACKEND_URL + 'users/', member).pipe(
      map(() => {
        const idx = this.members.indexOf(member);
        this.members[idx] = member;
      })
    );
  }

  setMainPhoto(photoId: number) {
    return this.http.put(
      this.BACKEND_URL + 'users/set-main-photo/' + photoId,
      {}
    );
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.BACKEND_URL + 'users/delete-photo/' + photoId);
  }

  addLike(username: string) {
    return this.http.post(this.BACKEND_URL + 'likes/' + username, {});
  }

  getLikes(predicate: string, pageNumber: number, pageSize: number) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('predicate', predicate);

    return getFilteredPaginatedResult<Partial<Member[]>>(
      this.BACKEND_URL + 'likes',
      params,
      this.http
    );
  }
}
