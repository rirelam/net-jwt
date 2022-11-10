/* eslint-disable @angular-eslint/component-selector */
import { Component } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent {
  constructor(private jwtHelper: JwtHelperService) {}

  isUserAuthenticated = (): boolean => {
    const token = localStorage.getItem('jwt');

    if (token && !this.jwtHelper.isTokenExpired(token)) {
      return true;
    }

    return false;
  };

  logOut = () => {
    localStorage.removeItem('jwt');
  };
}
