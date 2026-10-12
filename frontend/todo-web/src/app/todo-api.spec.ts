import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { environment } from '../environments/environment';
import { TodoApiService } from './todo-api';
import { Todo } from './todo.model';

describe('TodoApiService', () => {
  let service: TodoApiService;
  let http: HttpTestingController;
  const url = `${environment.apiUrl}/todos`;

  const sample: Todo = {
    id: 1,
    title: 'Learn Angular',
    isCompleted: false,
    createdAt: '2026-09-19T10:00:00Z',
    completedAt: null,
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(TodoApiService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify()); // fails if a request was unexpected

  it('GETs all todos', () => {
    let result: Todo[] = [];
    service.getAll().subscribe((todos) => (result = todos));

    const req = http.expectOne(url);
    expect(req.request.method).toBe('GET');
    req.flush([sample]);

    expect(result).toEqual([sample]);
  });

  it('GETs one todo by id', () => {
    let result: Todo | undefined;
    service.getOne(1).subscribe((todo) => (result = todo));

    const req = http.expectOne(`${url}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(sample);

    expect(result).toEqual(sample);
  });

  it('POSTs the title when creating', () => {
    service.create('Learn Angular').subscribe();

    const req = http.expectOne(url);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ title: 'Learn Angular' });
    req.flush(sample);
  });

  it('PUTs changes to the item URL', () => {
    service.update(1, { title: 'Learn Angular', isCompleted: true }).subscribe();

    const req = http.expectOne(`${url}/1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body.isCompleted).toBeTrue();
    req.flush({ ...sample, isCompleted: true });
  });

  it('DELETEs the item URL', () => {
    service.delete(1).subscribe();

    const req = http.expectOne(`${url}/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
});