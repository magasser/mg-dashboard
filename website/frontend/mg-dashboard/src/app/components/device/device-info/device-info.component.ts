import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { Device } from 'src/app/models/device';
import { DeviceMode } from 'src/app/models/device-mode';
import { DeviceState } from 'src/app/models/device-state';
import { DeviceClientService } from 'src/app/services/device-client.service';

@Component({
  selector: 'app-device-info',
  templateUrl: './device-info.component.html',
  styleUrls: ['./device-info.component.scss'],
})
export class DeviceInfoComponent implements OnInit, OnDestroy {
  private _subscriptions: Subscription[] = [];

  @Input() public device?: Device;
  @Input() public deviceClient?: DeviceClientService;

  public isConnected: boolean = false;
  public state: DeviceState = DeviceState.Unknown;
  public mode: DeviceMode = DeviceMode.Unknown;

  public ngOnInit(): void {
    if (!this.deviceClient) {
      throw new Error('Device client is not set.');
    }

    this._subscriptions.push(
      this.deviceClient.isConnected$.subscribe((isConnected) => {
        this.isConnected = isConnected;
      }),
      this.deviceClient.state$.subscribe((state) => {
        this.state = state;
      }),
      this.deviceClient.mode$.subscribe((mode) => {
        this.mode = mode;
      })
    );
  }

  public ngOnDestroy(): void {
    this._subscriptions.forEach((s) => s.unsubscribe());
    this._subscriptions = [];
  }
}
