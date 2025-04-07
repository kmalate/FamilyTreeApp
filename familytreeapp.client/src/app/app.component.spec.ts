import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';

describe('AppComponent', () => {
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AppComponent],
      imports: [HttpClientTestingModule]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create the app', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize FamilyTree on tree div element and request data on init', () => {    
    fixture.detectChanges(); // triggers ngOnInit
    const familyTree = component.familyTree;
   
    expect(familyTree).not.toBeNull();
    if (familyTree) {
      expect(familyTree.constructor.name).toBe('FamilyTree');
      expect(familyTree.element.id).toBe('tree');
    }

    const req = httpMock.expectOne('/family/getfamilytreenodes');
    expect(req.request.method).toBe('GET');
  });
});
