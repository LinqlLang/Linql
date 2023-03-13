import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LinqlClientFetchComponent } from './linql.client-fetch.component';

describe('LinqlClientFetchComponent', () => {
  let component: LinqlClientFetchComponent;
  let fixture: ComponentFixture<LinqlClientFetchComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LinqlClientFetchComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LinqlClientFetchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
