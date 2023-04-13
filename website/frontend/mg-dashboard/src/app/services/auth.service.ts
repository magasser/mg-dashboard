import {
  HttpClient,
  HttpHeaders,
  HttpErrorResponse,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  endpoint: string = environment.apiUrl;
  headers = new HttpHeaders().set('Content-Type', 'application/json');
  currentUser = {};

  constructor(private http: HttpClient, public router: Router) {}

  signUp(user: User): Observable<any> {
    let url = `${this.endpoint}/user/register`;
    return this.http.post<any>(url, user).pipe(catchError(this.handleError));
  }

  signIn(user: User) {
    let url = `${this.endpoint}/user/login`;
    return this.http.post(url, user).subscribe((res: any) => {
      localStorage.setItem('id', res.id);
      localStorage.setItem('access_token', res.token);
      this.getUserProfile(res.id).subscribe((res) => {
        this.currentUser = res;
        this.router.navigate(['user-profile/' + res.id]);
      });
    });
  }

  getToken() {
    return localStorage.getItem('access_token');
  }

  getId() {
    return localStorage.getItem('id');
  }

  isLoggedIn(): boolean {
    let authToken = this.getToken();
    return authToken !== null;
  }

  logout() {
    let removeId = localStorage.removeItem('id');
    let removeToken = localStorage.removeItem('access_token');
    if (removeToken == null && removeId == null) {
      this.router.navigate(['signin']);
    }
  }

  getUserProfile(id: any): Observable<any> {
    let url = `${this.endpoint}/user/${id}`;
    return this.http.get(url, { headers: this.headers }).pipe(
      map((res) => {
        return res || {};
      }),
      catchError(this.handleError)
    );
  }

  handleError(error: HttpErrorResponse) {
    let msg =
      error.error instanceof ErrorEvent
        ? error.error.message
        : `Error Code: ${error.status}\nMessage: ${error.message}`;

    return throwError(() => new Error(msg));
  }
}
