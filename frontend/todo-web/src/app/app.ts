import { Component } from '@angular/core';

import { TodoListComponent } from './todo-list/todo-list';

@Component({
  selector: 'app-root',
  imports: [TodoListComponent],
  template: `
    <main class="page">
      <h1>My To-Dos</h1>
      <app-todo-list />
    </main>
  `,
  styles: `
    .page { max-width: 560px; margin: 2rem auto; padding: 0 1rem; font-family: system-ui, sans-serif; }
  `,
})
export class App {}