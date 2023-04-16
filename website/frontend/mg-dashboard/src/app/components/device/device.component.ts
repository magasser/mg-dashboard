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
  private subscriptions: Subscription[];

  public twin$?: Observable<any>;
  public device?: Device;
  public isConnected: boolean;
  public state: DeviceState;
  public mode: DeviceMode;
  public deviceType = DeviceType;

  constructor(
    private deviceService: DeviceService,
    private deviceClient: DeviceClientService,
    private route: ActivatedRoute
  ) {
    this.isConnected = false;
    this.subscriptions = [];

    this.state = DeviceState.Unknown;
    this.mode = DeviceMode.Unknown;
  }

  async ngOnInit(): Promise<void> {
    const deviceId = this.route.snapshot.paramMap.get('id');

    if (!deviceId) {
      throw new Error('Invalid device id in param map.');
    }

    this.device = await firstValueFrom(this.deviceService.get(deviceId));

    this.deviceClient.start(deviceId);

    this.subscriptions.push(
      this.deviceClient.isConnected.subscribe((isConnected) => {
        this.isConnected = isConnected;
      }),
      this.deviceClient.state$.subscribe((state) => {
        this.state = state;
      }),
      this.deviceClient.mode$.subscribe((mode) => {
        this.mode = mode;
      })
    );

    this.twin$ = this.deviceClient.twin$;
  }
  ngOnDestroy(): void {
    this.subscriptions.forEach((s) => s.unsubscribe());
    this.subscriptions = [];

    this.deviceClient.stop();
  }
}
