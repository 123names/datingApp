import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { map } from "rxjs/operators"
import { User } from '../_models/user';
import { ReplaySubject } from 'rxjs';

const BACKEND_URL = environment.apiURL+"account/";
@Injectable({
  providedIn: 'root'
})
export class AccountService {

  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private httpClient: HttpClient) { }

  login(model:any){
    return this.httpClient.post(BACKEND_URL+"login", model).pipe(
      map((response:User)=>{
        const user = response;
        if (user){
          localStorage.setItem("user", JSON.stringify(user));
          this.currentUserSource.next(user);
        }
      })
    );
  }

  regsiter(model:any){
    return this.httpClient.post(BACKEND_URL+"register", model).pipe(
      map((user:User)=>{
        if(user){
          localStorage.setItem("user", JSON.stringify(user));
          this.currentUserSource.next(user);
        }
      })
    )
  }

  setCurrentUser(user: User){
    this.currentUserSource.next(user);
  }

  logout(){
    localStorage.removeItem("user");
    this.currentUserSource.next(null);
  }

  signUp(){

  }
}
