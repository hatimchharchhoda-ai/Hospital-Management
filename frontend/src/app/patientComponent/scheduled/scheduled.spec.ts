import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Scheduled } from './scheduled';

describe('Scheduled', () => {
  let component: Scheduled;
  let fixture: ComponentFixture<Scheduled>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Scheduled]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Scheduled);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
