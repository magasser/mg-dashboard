import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable, Subscription, firstValueFrom } from 'rxjs';
import { Device } from 'src/app/models/device';
import { DeviceMode } from 'src/app/models/device-mode';
import { DeviceState } from 'src/app/models/device-state';
import { DeviceType } from 'src/app/models/device-type';
import { DeviceClientService } from 'src/app/services/device-client.service';
import { DeviceService } from 'src/app/services/device.service';

@Component({
  selector: 'app-device',
  templateUrl: './device.component.html',
  styleUrls: ['./device.component.scss'],
})
export class DeviceComponent implements OnInit, OnDestroy {
  public device?: Device;
  public deviceType = DeviceType;

  constructor(
    public deviceClient: DeviceClientService,
    private deviceService: DeviceService,
    private route: ActivatedRoute
  ) {}

  async ngOnInit(): Promise<void> {
    const deviceId = this.route.snapshot.paramMap.get('id');

    if (!deviceId) {
      throw new Error('Invalid device id in param map.');
    }

    this.device = await firstValueFrom(this.deviceService.get(deviceId));

    this.deviceClient.start(deviceId);
  }

  ngOnDestroy(): void {
    this.deviceClient.stop();
  }
}
