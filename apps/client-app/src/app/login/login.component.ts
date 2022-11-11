/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable @angular-eslint/component-selector */
import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
} from '@angular/common/http';
import { NgForm } from '@angular/forms';
import { LoginResponse } from '../_interfaces/login-response';
import { LoginInput } from '../_interfaces/login-input';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit {
  invalidLogin = true;
  credentials: LoginInput = { username: '', password: '' };

  constructor(private router: Router, private http: HttpClient) {}

  ngOnInit(): void {
    return;
  }

  login = (form: NgForm) => {
    if (form.valid) {
      this.http
        .post<LoginResponse>(
          'https://localhost:5001/api/auth/login',
          this.credentials,
          {
            headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
          }
        )
        .subscribe({
          next: (response: LoginResponse) => {
            const token = response.token;
            const refreshToken = response.refreshToken;
            localStorage.setItem('jwt', token);
            localStorage.setItem('refreshToken', refreshToken);
            this.invalidLogin = false;
            this.router.navigate(['/']);
          },
          error: (err: HttpErrorResponse) => (this.invalidLogin = true),
        });
    }
  };
}
