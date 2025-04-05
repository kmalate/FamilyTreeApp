import { TestBed } from '@angular/core/testing';

import { FamilyService } from './family.service';

describe('FamilyserviceService', () => {
  let service: FamilyService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FamilyService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
