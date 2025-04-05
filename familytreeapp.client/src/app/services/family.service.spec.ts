import { TestBed } from '@angular/core/testing';

import { FamilyService } from './family.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('FamilyserviceService', () => {
  let service: FamilyService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });
    service = TestBed.inject(FamilyService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
