import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CarTwinComponent } from './car-twin.component';

describe('CarTwinComponent', () => {
  let component: CarTwinComponent;
  let fixture: ComponentFixture<CarTwinComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CarTwinComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CarTwinComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
