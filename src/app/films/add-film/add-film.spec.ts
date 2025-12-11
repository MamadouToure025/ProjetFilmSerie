import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddFilm } from './add-film';

describe('AddFilm', () => {
  let component: AddFilm;
  let fixture: ComponentFixture<AddFilm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddFilm]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddFilm);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
