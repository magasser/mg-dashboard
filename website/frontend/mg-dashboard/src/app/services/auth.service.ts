import {
  HttpClient,
  HttpHeaders,
  HttpErrorResponse,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, Subscription, of, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../models/user';
import { Credentials } from '../models/credentials';
import { Identification } from '../models/identification';
import { UserRegistration } from '../models/user-registration';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private _endpoint: string = environment.apiUrl;
  private _headers = new HttpHeaders().set('Content-Type', 'application/json');
  private _currentUser?: User;

  constructor(private http: HttpClient, public router: Router) {}

  public signUp(registration: UserRegistration): Subscription {
    let url = `${this._endpoint}/user/register`;
    return this.http
      .post<Identification>(url, registration)
      .subscribe((res) => {
        localStorage.setItem('id', res.id);
        localStorage.setItem('access_token', res.token);

        this.getUser().subscribe((res) => {
          this._currentUser = res;
          this.router.navigate(['']);
        });
      });
  }

  public signIn(credentials: Credentials): Subscription {
    let url = `${this._endpoint}/user/login`;
    return this.http.post<Identification>(url, credentials).subscribe((res) => {
      localStorage.setItem('id', res.id);
      localStorage.setItem('access_token', res.token);

      this.getUser().subscribe((res) => {
        this._currentUser = res;
        this.router.navigate(['']);
      });
    });
  }

  public get token(): string | null {
    return localStorage.getItem('access_token');
  }

  public get id(): string | null {
    return localStorage.getItem('id');
  }

  public get isLoggedIn(): boolean {
    return this.token !== null && this.id !== null;
  }

  public logout(): void {
    const removeId = localStorage.removeItem('id');
    const removeToken = localStorage.removeItem('access_token');
    if (removeToken == null && removeId == null) {
      this.router.navigate(['signin']);
    }
  }

  public getUser(): Observable<User> {
    if (this._currentUser) {
      return of<User>(this._currentUser);
    }

    const url = `${this._endpoint}/user/${this.id}`;
    return this.http.get<User>(url, { headers: this._headers }).pipe(
      map((res) => {
        return res || {};
      }),
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    const msg =
      error.error instanceof ErrorEvent
        ? error.error.message
        : `Error Code: ${error.status}\nMessage: ${error.message}`;

    return throwError(() => new Error(msg));
  }
}
