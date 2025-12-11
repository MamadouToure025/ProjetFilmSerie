import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeleteSerie } from './delete-serie';

describe('DeleteSerie', () => {
  let component: DeleteSerie;
  let fixture: ComponentFixture<DeleteSerie>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeleteSerie]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DeleteSerie);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
