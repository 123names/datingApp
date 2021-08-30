import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-top-nav',
  templateUrl: './top-nav.component.html',
  styleUrls: ['./top-nav.component.css']
})
export class TopNavComponent implements OnInit {

  user:any = {}
  constructor(public accountService:AccountService, private router:Router, private toast:ToastrService) { }

  ngOnInit(): void {
  }

  login(){
    this.accountService.login(this.user).subscribe(responseData=>{
      this.router.navigateByUrl("/members");
    });
  }

  logout(){
    this.accountService.logout();
    this.router.navigate([""]);
  }
}
