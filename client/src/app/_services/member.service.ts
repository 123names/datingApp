import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

const BACKEND_URL = environment.apiURL+"users/";

@Injectable({
  providedIn: 'root'
})
export class MemberService {

  constructor(private http: HttpClient) { }

  getMembers(){
    return this.http.get<Member[]>(BACKEND_URL);
  }

  getMember(username:string){
    return this.http.get<Member>(BACKEND_URL+username);
  }
}
