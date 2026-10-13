describe('To-Do application', () => {
  beforeEach(() => {
    cy.resetTodos();
  });

  it('shows the heading and the empty state', () => {
    cy.visit('/');

    cy.contains('h1', 'My To-Dos').should('be.visible');
    cy.contains('Nothing to do. Add your first task above.').should('be.visible');
    cy.contains('0 remaining').should('be.visible');
  });

  it('keeps the Add button disabled while the input is empty', () => {
    cy.visit('/');

    cy.contains('button', 'Add').should('be.disabled');
    cy.get('#new-title').type('Buy milk');
    cy.contains('button', 'Add').should('be.enabled');
  });

  it('adds a to-do through the form and saves it in the API', () => {
    cy.intercept('POST', '**/api/todos').as('createTodo');
    cy.visit('/');

    cy.get('#new-title').type('Buy milk{enter}');

    cy.wait('@createTodo').its('response.statusCode').should('eq', 201);
    cy.get('ul.list li').should('have.length', 1).and('contain.text', 'Buy milk');
    cy.contains('1 remaining').should('be.visible');
    cy.get('#new-title').should('have.value', '');
  });

  it('does not send a title that is only spaces', () => {
    cy.intercept('POST', '**/api/todos').as('createTodo');
    cy.visit('/');

    cy.get('#new-title').type('   {enter}');

    cy.contains('Nothing to do. Add your first task above.').should('be.visible');
    cy.get('@createTodo.all').should('have.length', 0);
  });

  it('marks a to-do as completed and remembers it after a reload', () => {
    cy.addTodoViaApi('Buy milk');
    cy.visit('/');

    cy.contains('li', 'Buy milk').find('input[type=checkbox]').check();

    cy.contains('li', 'Buy milk').find('span').should('have.class', 'done');
    cy.contains('0 remaining').should('be.visible');

    cy.reload();
    cy.contains('li', 'Buy milk').find('input[type=checkbox]').should('be.checked');
  });

  it('deletes a to-do', () => {
    cy.addTodoViaApi('Buy milk');
    cy.addTodoViaApi('Walk the dog');
    cy.visit('/');

    cy.contains('li', 'Buy milk').contains('button', 'Delete').click();

    cy.get('ul.list li').should('have.length', 1).and('contain.text', 'Walk the dog');
    cy.contains('1 remaining').should('be.visible');
  });
});

describe('when the API fails', () => {
  it('shows a friendly message when the list cannot be loaded', () => {
    cy.intercept('GET', '**/api/todos', { statusCode: 500, body: { title: 'boom' } }).as('loadTodos');
    cy.visit('/');

    cy.wait('@loadTodos');
    cy.get('[role=alert]').should('contain.text', 'Could not load your to-dos');
  });

  it('shows a message when adding a to-do fails', () => {
    cy.intercept('GET', '**/api/todos', { body: [] }).as('loadTodos');
    cy.intercept('POST', '**/api/todos', { statusCode: 500 }).as('createTodo');
    cy.visit('/');
    cy.wait('@loadTodos');

    cy.get('#new-title').type('Buy milk{enter}');

    cy.wait('@createTodo');
    cy.get('[role=alert]').should('contain.text', 'Could not add that to-do.');
    cy.get('ul.list li').should('have.length', 1).and('contain.text', 'Nothing to do');
  });

  it('shows Loading while a slow response is on its way', () => {
    cy.intercept('GET', '**/api/todos', { body: [], delay: 1000 }).as('loadTodos');
    cy.visit('/');

    cy.contains('Loading…').should('be.visible');
    cy.wait('@loadTodos');
    cy.contains('Loading…').should('not.exist');
  });
});