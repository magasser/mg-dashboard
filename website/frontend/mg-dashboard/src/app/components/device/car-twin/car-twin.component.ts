import { Component, Input } from '@angular/core';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-car-twin',
  templateUrl: './car-twin.component.html',
  styleUrls: ['./car-twin.component.scss'],
})
export class CarTwinComponent {
  @Input() twin$?: Observable<any>;
}
