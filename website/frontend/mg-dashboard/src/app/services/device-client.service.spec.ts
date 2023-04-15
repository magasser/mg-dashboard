import { TestBed } from '@angular/core/testing';

import { DeviceClientService } from './device-client.service';

describe('DeviceClientService', () => {
  let service: DeviceClientService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DeviceClientService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
