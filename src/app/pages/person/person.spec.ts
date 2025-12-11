import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Person } from './person';

describe('Person', () => {
  let component: Person;
  let fixture: ComponentFixture<Person>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Person]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Person);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
