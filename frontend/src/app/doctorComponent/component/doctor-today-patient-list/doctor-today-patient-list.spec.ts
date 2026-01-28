import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DoctorTodayPatientList } from './doctor-today-patient-list';

describe('DoctorTodayPatientList', () => {
  let component: DoctorTodayPatientList;
  let fixture: ComponentFixture<DoctorTodayPatientList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DoctorTodayPatientList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DoctorTodayPatientList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
