import { Injectable, computed, inject, signal } from '@angular/core';

import { Todo, TodoUpdate } from './todo.model';
import { TodoApiService } from './todo-api';

@Injectable({ providedIn: 'root' })
export class TodoStore {
  private readonly api = inject(TodoApiService);

  readonly todos = signal<Todo[]>([]);
  readonly selected = signal<Todo | null>(null); // the one to-do shown in the details panel
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  // Derived state: recalculated automatically when todos changes.
  readonly remaining = computed(
    () => this.todos().filter((t) => !t.isCompleted).length,
  );

  load(): void {
    this.loading.set(true);
    this.api.getAll().subscribe({
      next: (todos) => {
        this.todos.set(todos);
        this.error.set(null);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load your to-dos. Is the API running?');
        this.loading.set(false);
      },
    });
  }

  add(title: string): void {
    this.api.create(title).subscribe({
      next: (created) => this.todos.update((list) => [...list, created]),
      error: () => this.error.set('Could not add that to-do.'),
    });
  }

  select(todo: Todo): void {
    this.api.getOne(todo.id).subscribe({
      next: (found) => this.selected.set(found),
      error: () => this.error.set('Could not load that to-do.'),
    });
  }

  clearSelection(): void {
    this.selected.set(null);
  }

  toggle(todo: Todo): void {
    this.save(todo, { title: todo.title, isCompleted: !todo.isCompleted }, 'Could not update that to-do.');
  }

  rename(todo: Todo, title: string): void {
    this.save(todo, { title, isCompleted: todo.isCompleted }, 'Could not rename that to-do.');
  }

  remove(todo: Todo): void {
    this.api.delete(todo.id).subscribe({
      next: () => {
        this.todos.update((list) => list.filter((t) => t.id !== todo.id));
        if (this.selected()?.id === todo.id) {
          this.selected.set(null);
        }
      },
      error: () => this.error.set('Could not delete that to-do.'),
    });
  }

  // Ticking and renaming are the same request (PUT) with different changes, so they share this method.
  private save(todo: Todo, changes: TodoUpdate, message: string): void {
    this.api.update(todo.id, changes).subscribe({
      next: (updated) => {
        this.todos.update((list) => list.map((t) => (t.id === updated.id ? updated : t)));
        if (this.selected()?.id === updated.id) {
          this.selected.set(updated);
        }
      },
      error: () => this.error.set(message),
    });
  }
}