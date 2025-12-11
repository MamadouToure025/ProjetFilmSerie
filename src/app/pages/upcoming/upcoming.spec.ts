import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Upcoming } from './upcoming';

describe('Upcoming', () => {
  let component: Upcoming;
  let fixture: ComponentFixture<Upcoming>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Upcoming]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Upcoming);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
