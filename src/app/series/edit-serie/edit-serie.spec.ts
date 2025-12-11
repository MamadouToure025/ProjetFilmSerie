import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditSerie } from './edit-serie';

describe('EditSerie', () => {
  let component: EditSerie;
  let fixture: ComponentFixture<EditSerie>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditSerie]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditSerie);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
