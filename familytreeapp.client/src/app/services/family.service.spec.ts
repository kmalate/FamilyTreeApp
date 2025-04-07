import { TestBed } from '@angular/core/testing';

import { FamilyService } from './family.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { FamilyNode } from '../models/family-node.model';
import { UpdateNodeArgs } from '../models/update-node-args.model';

describe('FamilyserviceService', () => {
  let service: FamilyService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [FamilyService]
    });
    service = TestBed.inject(FamilyService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch family nodes', () => {
    const dummyFamilyNodes: FamilyNode[] = [
      { id: 1, name: 'John Doe', gender: 'male' } as FamilyNode,
      { id: 2, name: 'Jane Doe', gender: 'female' } as FamilyNode
    ];

    service.getFamilyNodes().subscribe(nodes => {
      expect(nodes.length).toBe(2);
      expect(nodes).toEqual(dummyFamilyNodes);
    });

    const req = httpMock.expectOne('/family/getfamilytreenodes');
    expect(req.request.method).toBe('GET');
    req.flush(dummyFamilyNodes);
  });

  it('should update family tree nodes', () => {
    const updateArgs = {} as UpdateNodeArgs;
    const dummyResponse = { "oldId1": 1 };
    service.updateFamilyTreeNodes(updateArgs).subscribe(response => {
      expect(response).toBeTruthy();
      expect(response["oldId1"]).toBe(1);
    });
    const req = httpMock.expectOne('/family/updatefamilytreenodes');
    expect(req.request.method).toBe('POST');
    req.flush(dummyResponse);
  });
});
