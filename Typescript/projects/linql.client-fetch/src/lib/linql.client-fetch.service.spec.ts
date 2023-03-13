import { TestBed } from '@angular/core/testing';

import { LinqlClientFetchService } from './linql.client-fetch.service';

describe('LinqlClientFetchService', () => {
  let service: LinqlClientFetchService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LinqlClientFetchService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
