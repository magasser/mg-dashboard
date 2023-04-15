import { Injectable } from '@angular/core';
import { MqttService } from 'ngx-mqtt';

@Injectable({
  providedIn: 'root',
})
export class DeviceClientService {
  connection = {
    protocol: 'ws',
    hostname: 'localhost',
    port: 8083,
    path: '/mqtt',
  };

  constructor(private mqttService: MqttService) {}
}
