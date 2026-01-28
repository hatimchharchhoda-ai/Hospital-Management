import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppointmentPopup } from './appointment-popup';

describe('AppointmentPopup', () => {
  let component: AppointmentPopup;
  let fixture: ComponentFixture<AppointmentPopup>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppointmentPopup]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AppointmentPopup);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
