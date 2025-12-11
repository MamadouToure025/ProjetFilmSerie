import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddSerie } from './add-serie';

describe('AddSerie', () => {
  let component: AddSerie;
  let fixture: ComponentFixture<AddSerie>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddSerie]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddSerie);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
