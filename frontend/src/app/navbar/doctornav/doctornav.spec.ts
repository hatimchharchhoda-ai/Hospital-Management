import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Doctornav } from './doctornav';

describe('Doctornav', () => {
  let component: Doctornav;
  let fixture: ComponentFixture<Doctornav>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Doctornav]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Doctornav);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
