import { Component, Input } from '@angular/core';
import { Device } from 'src/app/models/device';
import { DeviceClientService } from 'src/app/services/device-client.service';

@Component({
  selector: 'app-car',
  templateUrl: './car.component.html',
  styleUrls: ['./car.component.scss'],
})
export class CarComponent {
  private _isSteering: boolean = false;
  private _isMoving: boolean = false;

  @Input() public device?: Device;
  @Input() public deviceClient!: DeviceClientService;

  public steerKey?: string;
  public moveKey?: string;

  public controlKeyDown(event: KeyboardEvent): void {
    if (event.repeat) return;

    switch (event.key) {
      case 'a':
        if (this._isSteering) return;
        this._isSteering = true;
        this.deviceClient.sendMessage('cmd.tl,100;');
        this.steerKey = 'a';
        break;
      case 'd':
        if (this._isSteering) return;
        this._isSteering = true;
        this.deviceClient.sendMessage('cmd.tr,100;');
        this.steerKey = 'd';
        break;
      case 'w':
        if (this._isMoving) return;
        this._isMoving = true;
        this.deviceClient.sendMessage('cmd.fw,100;');
        this.moveKey = 'w';
        break;
      case 's':
        if (this._isMoving) return;
        this._isMoving = true;
        this.deviceClient.sendMessage('cmd.bw,100;');
        this.moveKey = 's';
        break;
    }
  }

  public controlKeyUp(event: KeyboardEvent): void {
    if (event.key === this.steerKey) {
      this.resetSteering();
    }

    if (event.key === this.moveKey) {
      this.resetMoving();
    }
  }

  public controlFocusOut(event: FocusEvent): void {
    if (this._isSteering) {
      this.resetSteering();
    }

    if (this._isMoving) {
      this.resetMoving();
    }
  }

  private resetSteering(): void {
    this.deviceClient.sendMessage('cmd.tl,0;');
    this._isSteering = false;
    this.steerKey = undefined;
  }

  private resetMoving(): void {
    this.deviceClient.sendMessage('cmd.fw,0;');
    this._isMoving = false;
    this.moveKey = undefined;
  }
}
