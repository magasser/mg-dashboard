import { Injectable } from '@angular/core';
import {
  IMqttMessage,
  IMqttServiceOptions,
  MqttConnectionState,
  MqttService,
} from 'ngx-mqtt';
import {
  BehaviorSubject,
  Observable,
  Subject,
  Subscription,
  firstValueFrom,
  map,
} from 'rxjs';
import { environment } from 'src/environments/environment';
import { DeviceState } from '../models/device-state';
import { DeviceMode } from '../models/device-mode';

export const MQTT_SERVICE_OPTIONS: IMqttServiceOptions = {
  url: environment.mqttConnectionUrl,
};

const Topics = {
  STATE: '/device/state',
  MODE: '/device/mode',
  MESSAGE: '/device/message',
  TWIN: '/device/twin',
};

@Injectable({
  providedIn: 'root',
})
export class DeviceClientService {
  private deviceId?: string;
  private subscriptions: Subscription[];
  private isConnectedSubject$: BehaviorSubject<boolean>;
  private stateSubject$: BehaviorSubject<DeviceState>;
  private modeSubject$: BehaviorSubject<DeviceMode>;

  public twinSubject: Subject<any>;

  constructor(private mqttService: MqttService) {
    this.subscriptions = [];
    this.isConnectedSubject$ = new BehaviorSubject<boolean>(false);
    this.stateSubject$ = new BehaviorSubject<DeviceState>(DeviceState.Unknown);
    this.modeSubject$ = new BehaviorSubject<DeviceMode>(DeviceMode.Unknown);
    this.twinSubject = new Subject<any>();
  }

  public get isConnected$(): Observable<boolean> {
    return this.isConnectedSubject$.asObservable();
  }

  public get state$(): Observable<DeviceState> {
    return this.stateSubject$.asObservable();
  }

  public get mode$(): Observable<DeviceMode> {
    return this.modeSubject$.asObservable();
  }

  public get twin$(): Observable<any> {
    return this.twinSubject.asObservable();
  }

  public start(deviceId: string): void {
    this.deviceId = deviceId;

    this.subscriptions.push(
      this.mqttService.state
        .asObservable()
        .subscribe((connectionState) =>
          this.onConnectionStateChanged(connectionState)
        ),
      this.mqttService
        .observe(`${this.deviceId}${Topics.STATE}`)
        .subscribe((state) => this.onStateChanged(state)),
      this.mqttService
        .observe(`${this.deviceId}${Topics.MODE}`)
        .subscribe((mode) => this.onModeChanged(mode)),
      this.mqttService
        .observe(`${this.deviceId}${Topics.TWIN}`)
        .subscribe((twin) => this.onTwinChanged(twin))
    );
  }

  public stop(): void {
    this.deviceId = undefined;

    this.subscriptions.forEach((s) => s.unsubscribe());
    this.subscriptions = [];
  }

  public sendMessage(message: string): void {
    if (!this.isConnectedSubject$.value) {
      throw new Error('Client has to be connected to send message.');
    }

    const topic = `${this.deviceId}${Topics.MESSAGE}`;
    firstValueFrom(this.mqttService.publish(topic, message)).catch((err) => {
      console.error(err);
    });
  }

  private onConnectionStateChanged(state: MqttConnectionState): void {
    this.isConnectedSubject$.next(state === MqttConnectionState.CONNECTED);
  }

  private onStateChanged(message: IMqttMessage): void {
    const state = parseInt(message.payload.toString());

    switch (state) {
      case 1:
        this.stateSubject$.next(DeviceState.Running);
        break;
      case 2:
        this.stateSubject$.next(DeviceState.Error);
        break;
      default:
        this.stateSubject$.next(DeviceState.Unknown);
        break;
    }
  }

  private onModeChanged(message: IMqttMessage): void {
    const mode = parseInt(message.payload.toString());

    switch (mode) {
      case 1:
        this.modeSubject$.next(DeviceMode.Automatic);
        break;
      case 2:
        this.modeSubject$.next(DeviceMode.Controlled);
        break;
      default:
        this.modeSubject$.next(DeviceMode.Unknown);
        break;
    }
  }

  private onTwinChanged(message: IMqttMessage): void {
    const twin = JSON.parse(message.payload.toString());

    if (twin) {
      this.twinSubject.next(twin);
    }
  }
}
