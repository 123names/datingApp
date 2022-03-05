import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();

  registerForm: FormGroup;
  ageRestriction: Date;
  validationErrors: string[] = [];

  constructor(
    private accountService: AccountService,
    private toast: ToastrService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    this.ageRestriction = new Date();
    this.ageRestriction.setFullYear(this.ageRestriction.getFullYear() - 18);
  }

  initializeForm() {
    this.registerForm = new FormGroup({
      gender: new FormControl('male', Validators.required),
      username: new FormControl('', Validators.required),
      knownAs: new FormControl('', Validators.required),
      email: new FormControl('', [Validators.required, Validators.email]),
      dateOfBirth: new FormControl('', Validators.required),
      city: new FormControl('', Validators.required),
      country: new FormControl('', Validators.required),
      password: new FormControl('', [
        Validators.required,
        Validators.minLength(8),
        Validators.maxLength(32),
      ]),
      confirmPassword: new FormControl('', [
        Validators.required,
        this.matchValues('password'),
      ]),
    });
    this.registerForm.controls.password.valueChanges.subscribe(() => {
      this.registerForm.controls.confirmPassword.updateValueAndValidity();
    });
  }

  matchValues(matchToField: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control?.value === control?.parent?.controls[matchToField].value
        ? null
        : { isMatch: true };
    };
  }

  register() {
    this.accountService.register(this.registerForm.value).subscribe(
      (response) => {
        this.router.navigateByUrl('/members');
      },
      (error) => {
        this.validationErrors = error;
      }
    );
  }
  cancel() {
    this.cancelRegister.emit(false);
  }
}
