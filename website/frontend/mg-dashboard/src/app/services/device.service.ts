import {
  HttpHeaders,
  HttpClient,
  HttpErrorResponse,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, map, throwError } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Device } from '../models/device';
import { Registration } from '../models/registration';
import { Router } from '@angular/router';
import { DeviceType } from '../models/device-type';

@Injectable({
  providedIn: 'root',
})
export class DeviceService {
  private endpoint: string = environment.apiUrl;
  private headers = new HttpHeaders().set('Content-Type', 'application/json');
  constructor(private http: HttpClient, private router: Router) {}

  public get(id: string): Observable<Device> {
    const url = `${this.endpoint}/device/${id}`;
    return this.http.get<Device>(url, { headers: this.headers }).pipe(
      map((res) => {
        if (res) {
          res.type = this.getDeviceType(res.type);
        }
        return res || {};
      }),
      catchError(this.handleError)
    );
  }
  private getDeviceType(type: DeviceType | number): DeviceType {
    switch (type) {
      case 0:
        return DeviceType.Car;
      case 1:
        return DeviceType.Drone;
      case 2:
        return DeviceType.Boat;
      default:
        return DeviceType.Unknown;
    }
  }

  public getMy(): Observable<Device[]> {
    const url = `${this.endpoint}/device/my`;
    return this.http.get<Device[]>(url, { headers: this.headers }).pipe(
      map((res) => {
        if (res) {
          res.forEach((d) => {
            d.type = this.getDeviceType(d.type);
          });
        }

        return res || [];
      }),
      catchError(this.handleError)
    );
  }

  public register(registration: Registration) {
    const url = `${this.endpoint}/device/register`;
    return this.http
      .post<Device>(url, registration)
      .subscribe((res: Device) => {
        this.router.navigate([`device/${res.id}`]);
      });
  }

  private handleError(error: HttpErrorResponse) {
    const msg =
      error.error instanceof ErrorEvent
        ? error.error.message
        : `Error Code: ${error.status}\nMessage: ${error.message}`;

    return throwError(() => new Error(msg));
  }
}
