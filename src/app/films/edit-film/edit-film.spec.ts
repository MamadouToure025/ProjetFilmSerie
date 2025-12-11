import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditFilm } from './edit-film';

describe('EditFilm', () => {
  let component: EditFilm;
  let fixture: ComponentFixture<EditFilm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditFilm]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditFilm);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
