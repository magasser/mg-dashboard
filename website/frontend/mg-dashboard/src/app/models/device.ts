import { DeviceType } from './device-type';

export class Device {
  constructor(
    public id: string,
    public name: string,
    public type: DeviceType
  ) {}
}
