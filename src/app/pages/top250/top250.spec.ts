import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Top250 } from './top250';

describe('Top250', () => {
  let component: Top250;
  let fixture: ComponentFixture<Top250>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Top250]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Top250);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
