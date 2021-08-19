import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  test = "The Dating app"
  users: any;
  constructor(private httpClient:HttpClient){}

  ngOnInit(){
    this.getUsers();
  }
  getUsers(){
    this.httpClient.get("https://localhost:5001/api/users").subscribe(responseData=>{
      this.users = responseData;
    }, error=>{
      console.log(error);
    })
  }
}
