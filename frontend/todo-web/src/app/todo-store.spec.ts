import { TestBed } from '@angular/core/testing';
import { TodoStore } from './todo-store';

describe('TodoStore', () => {
  let service: TodoStore;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TodoStore);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
