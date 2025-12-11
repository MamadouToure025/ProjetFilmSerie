import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeleteFilm } from './delete-film';

describe('DeleteFilm', () => {
  let component: DeleteFilm;
  let fixture: ComponentFixture<DeleteFilm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeleteFilm]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DeleteFilm);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
