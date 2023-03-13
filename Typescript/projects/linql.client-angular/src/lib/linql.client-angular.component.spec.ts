import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LinqlClientAngularComponent } from './linql.client-angular.component';

describe('LinqlClientAngularComponent', () => {
  let component: LinqlClientAngularComponent;
  let fixture: ComponentFixture<LinqlClientAngularComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LinqlClientAngularComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LinqlClientAngularComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
