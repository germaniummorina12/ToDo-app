import { DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';

import { Todo } from '../todo.model';
import { TodoStore } from '../todo-store';

@Component({
  selector: 'app-todo-list',
  imports: [FormsModule, ReactiveFormsModule, DatePipe],
  template: `
    <form class="add" (submit)="add($event)">
      <label for="new-title" class="sr-only">New to-do</label>
      <input
        id="new-title"
        type="text"
        [formControl]="title"
        placeholder="What needs doing?"
        maxlength="200"
      />
      <button type="submit" [disabled]="title.invalid">Add</button>
    </form>

    @if (store.error(); as message) {
      <p class="error" role="alert">{{ message }}</p>
    }

    @if (store.loading()) {
      <p>Loading…</p>
    }

    <ul class="list">
      @for (todo of store.todos(); track todo.id) {
        <li>
          @if (editingId() === todo.id) {
            <form class="edit" (submit)="saveEdit(todo, $event)">
              <input
                [formControl]="editTitle"
                maxlength="200"
                aria-label="Edit the title"
                (keydown.escape)="cancelEdit()"
              />
              <button type="submit" [disabled]="editTitle.invalid">Save</button>
              <button type="button" (click)="cancelEdit()">Cancel</button>
            </form>
          } @else {
            <label>
              <input
                type="checkbox"
                [checked]="todo.isCompleted"
                (change)="store.toggle(todo)"
              />
              <span [class.done]="todo.isCompleted">{{ todo.title }}</span>
            </label>
            <div class="actions">
              <button type="button" (click)="store.select(todo)">Details</button>
              <button type="button" (click)="startEdit(todo)">Edit</button>
              <button type="button" (click)="store.remove(todo)">Delete</button>
            </div>
          }
        </li>
      } @empty {
        <li class="empty">Nothing to do. Add your first task above.</li>
      }
    </ul>

    <p class="summary">{{ store.remaining() }} remaining</p>

    @if (store.selected(); as todo) {
      <section class="details" aria-label="To-do details">
        <h2>{{ todo.title }}</h2>
        <dl>
          <dt>Id</dt>
          <dd>{{ todo.id }}</dd>
          <dt>Status</dt>
          <dd>{{ todo.isCompleted ? 'Done' : 'Open' }}</dd>
          <dt>Created</dt>
          <dd>{{ todo.createdAt | date: 'medium' }}</dd>
          @if (todo.completedAt) {
            <dt>Completed</dt>
            <dd>{{ todo.completedAt | date: 'medium' }}</dd>
          }
        </dl>
        <button type="button" (click)="store.clearSelection()">Close</button>
      </section>
    }
  `,
  styles: `
    .add { display: flex; gap: 8px; }
    .add input { flex: 1; padding: 8px; }
    .list { list-style: none; padding: 0; }
    .list li { display: flex; justify-content: space-between; align-items: center; gap: 8px; padding: 8px 0; border-bottom: 1px solid #ddd; }
    .actions { display: flex; gap: 6px; }
    .edit { display: flex; flex: 1; gap: 6px; }
    .edit input { flex: 1; padding: 6px; }
    .done { text-decoration: line-through; color: #888; }
    .details { margin-top: 16px; padding: 12px 16px; border: 1px solid #ddd; border-radius: 6px; }
    .details h2 { margin: 0 0 8px; font-size: 1.1rem; }
    .details dl { display: grid; grid-template-columns: auto 1fr; gap: 4px 12px; margin: 0 0 8px; }
    .details dt { font-weight: 600; }
    .details dd { margin: 0; }
    .error { color: #b00020; }
    .sr-only { position: absolute; left: -9999px; }
  `,
})
export class TodoListComponent implements OnInit {
  protected readonly store = inject(TodoStore);

  protected readonly title = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required, Validators.maxLength(200)],
  });

  // Only the screen cares about these two, so they live here and not in the store.
  protected readonly editingId = signal<number | null>(null);
  protected readonly editTitle = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required, Validators.maxLength(200)],
  });

  ngOnInit(): void {
    this.store.load();
  }

  protected add(event: Event): void {
    event.preventDefault(); // without this the browser would reload the page
    const value = this.title.value.trim();
    if (!value) {
      return;
    }
    this.store.add(value);
    this.title.reset();
  }

  protected startEdit(todo: Todo): void {
    this.editTitle.setValue(todo.title);
    this.editingId.set(todo.id);
  }

  protected saveEdit(todo: Todo, event: Event): void {
    event.preventDefault();
    const value = this.editTitle.value.trim();
    if (!value) {
      return;
    }
    if (value !== todo.title) {
      this.store.rename(todo, value);
    }
    this.editingId.set(null);
  }

  protected cancelEdit(): void {
    this.editingId.set(null);
  }
}