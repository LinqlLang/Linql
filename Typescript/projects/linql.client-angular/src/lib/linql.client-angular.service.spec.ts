import { TestBed } from '@angular/core/testing';

import { LinqlClientAngularService } from './linql.client-angular.service';

describe('LinqlClientAngularService', () => {
  let service: LinqlClientAngularService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LinqlClientAngularService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
