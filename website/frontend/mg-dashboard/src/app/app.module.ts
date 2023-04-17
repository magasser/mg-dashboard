import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SigninComponent } from './components/signin/signin.component';
import { SignupComponent } from './components/signup/signup.component';
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { HomeComponent } from './components/home/home.component';
import { DeviceComponent } from './components/device/device.component';
import { MQTT_SERVICE_OPTIONS } from './services/device-client.service';
import { MqttModule } from 'ngx-mqtt';
import { CarComponent } from './components/device/car/car.component';
import { DeviceInfoComponent } from './components/device/device-info/device-info.component';

@NgModule({
  declarations: [
    AppComponent,
    SigninComponent,
    SignupComponent,
    HomeComponent,
    DeviceComponent,
    CarComponent,
    DeviceInfoComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    MqttModule.forRoot(MQTT_SERVICE_OPTIONS),
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true,
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
