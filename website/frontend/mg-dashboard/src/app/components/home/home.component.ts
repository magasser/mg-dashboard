import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { Device } from 'src/app/models/device';
import { DeviceService } from 'src/app/services/device.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  public devices: Device[] = [];

  constructor(private deviceService: DeviceService, private router: Router) {}

  public async ngOnInit(): Promise<void> {
    this.devices = await firstValueFrom(this.deviceService.getMy());
  }

  public viewDevice(device: Device) {
    this.router.navigate([`device/${device.id}`]);
  }
}
