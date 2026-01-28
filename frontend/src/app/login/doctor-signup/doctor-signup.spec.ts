import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DoctorSignup } from './doctor-signup';

describe('DoctorSignup', () => {
  let component: DoctorSignup;
  let fixture: ComponentFixture<DoctorSignup>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DoctorSignup]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DoctorSignup);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
