import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LinqlClientNodeFetchComponent } from './linql.client-node-fetch.component';

describe('LinqlClientNodeFetchComponent', () => {
  let component: LinqlClientNodeFetchComponent;
  let fixture: ComponentFixture<LinqlClientNodeFetchComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LinqlClientNodeFetchComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LinqlClientNodeFetchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
