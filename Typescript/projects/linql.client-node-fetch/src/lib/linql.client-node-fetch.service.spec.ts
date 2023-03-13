import { TestBed } from '@angular/core/testing';

import { LinqlClientNodeFetchService } from './linql.client-node-fetch.service';

describe('LinqlClientNodeFetchService', () => {
  let service: LinqlClientNodeFetchService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LinqlClientNodeFetchService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
