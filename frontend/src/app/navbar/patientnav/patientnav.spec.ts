import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Patientnav } from './patientnav';

describe('Patientnav', () => {
  let component: Patientnav;
  let fixture: ComponentFixture<Patientnav>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Patientnav]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Patientnav);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
