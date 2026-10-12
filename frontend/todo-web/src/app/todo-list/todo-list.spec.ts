import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';

import { environment } from '../../environments/environment';
import { TodoListComponent } from './todo-list';

describe('TodoListComponent', () => {
  let fixture: ComponentFixture<TodoListComponent>;
  let http: HttpTestingController;
  const url = `${environment.apiUrl}/todos`;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TodoListComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    fixture = TestBed.createComponent(TodoListComponent);
    http = TestBed.inject(HttpTestingController);
    fixture.detectChanges(); // runs ngOnInit, which requests the list
  });

  it('renders the todos returned by the API', () => {
    http.expectOne(url).flush([
      { id: 1, title: 'One', isCompleted: false, createdAt: '', completedAt: null },
      { id: 2, title: 'Two', isCompleted: true, createdAt: '', completedAt: '' },
    ]);
    fixture.detectChanges();

    const items = fixture.nativeElement.querySelectorAll('li');
    expect(items.length).toBe(2);
    expect(items[0].textContent).toContain('One');
  });

  it('shows an empty message when there are no todos', () => {
    http.expectOne(url).flush([]);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Nothing to do');
  });

  it('disables the Add button while the input is empty', () => {
    http.expectOne(url).flush([]);
    fixture.detectChanges();

    const button: HTMLButtonElement =
      fixture.nativeElement.querySelector('button[type="submit"]');
    expect(button.disabled).toBeTrue();
  });

  it('renames a todo with the Edit button', () => {
    http.expectOne(url).flush([
      { id: 1, title: 'One', isCompleted: false, createdAt: '', completedAt: null },
    ]);
    fixture.detectChanges();
    const root: HTMLElement = fixture.nativeElement;

    clickButton(root, 'Edit');
    fixture.detectChanges();

    const input: HTMLInputElement = root.querySelector('form.edit input')!;
    input.value = 'One, renamed';
    input.dispatchEvent(new Event('input'));
    root.querySelector('form.edit')!.dispatchEvent(new Event('submit'));

    const req = http.expectOne(`${url}/1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual({ title: 'One, renamed', isCompleted: false });
    req.flush({ id: 1, title: 'One, renamed', isCompleted: false, createdAt: '', completedAt: null });
    fixture.detectChanges();

    expect(root.querySelector('li')!.textContent).toContain('One, renamed');
  });

  it('shows the details of one todo loaded by id', () => {
    http.expectOne(url).flush([
      { id: 1, title: 'One', isCompleted: false, createdAt: '', completedAt: null },
    ]);
    fixture.detectChanges();
    const root: HTMLElement = fixture.nativeElement;

    clickButton(root, 'Details');

    const req = http.expectOne(`${url}/1`);
    expect(req.request.method).toBe('GET');
    req.flush({ id: 1, title: 'One', isCompleted: false, createdAt: '2026-09-19T10:00:00Z', completedAt: null });
    fixture.detectChanges();

    expect(root.querySelector('.details')!.textContent).toContain('One');
  });
});

// Finds a button by its text and clicks it, like a user would.
function clickButton(root: HTMLElement, text: string): void {
  const button = Array.from(root.querySelectorAll('button')).find(
    (b) => b.textContent?.trim() === text,
  );
  button!.click();
}