const api = Cypress.expose('apiUrl') as string;
const root = api.replace(/\/api$/, '');

describe('Security checks (OWASP)', () => {
  beforeEach(() => {
    cy.resetTodos();
  });

  
  it('A05: a SQL injection string in a title is stored as plain text', () => {
    const attack = `'); DROP TABLE "Todos"; --`;

    cy.request('POST', `${api}/todos`, { title: attack }).then((res) => {
      expect(res.status).to.eq(201);
      expect(res.body.title).to.eq(attack);
    });

    cy.request(`${api}/todos`).its('body').should('have.length', 1);
  });

  it('A05: script in a title is shown as text and never runs', () => {
    const payload = '<img src=x onerror=alert(1)>';
    cy.addTodoViaApi(payload);

    cy.visit('/', {
      onBeforeLoad(win) {
        cy.stub(win, 'alert').as('alert');
      },
    });

    cy.contains('li', payload).should('be.visible');
    cy.get('ul.list li img').should('not.exist');
    cy.get('@alert').should('not.have.been.called');
  });


  it('API3: the client cannot choose the id, the dates or the completed flag', () => {
    cy.request('POST', `${api}/todos`, {
      title: 'Buy milk',
      id: 9999,
      isCompleted: true,
      createdAt: '2000-01-01T00:00:00Z',
      completedAt: '2000-01-01T00:00:00Z',
    }).then(({ body }) => {
      expect(body.id).to.not.eq(9999);
      expect(body.isCompleted).to.eq(false);
      expect(body.completedAt).to.eq(null);
      expect(new Date(body.createdAt).getTime()).to.be.greaterThan(Date.parse('2024-01-01'));
    });
  });

  const badTitles: [string, string][] = [
    ['empty', ''],
    ['only spaces', '   '],
    ['201 characters long', 'x'.repeat(201)],
  ];

  badTitles.forEach(([name, title]) => {
    it(`A06: refuses a title that is ${name}`, () => {
      cy.request({ method: 'POST', url: `${api}/todos`, body: { title }, failOnStatusCode: false })
        .its('status')
        .should('eq', 400);

      cy.request(`${api}/todos`).its('body').should('have.length', 0);
    });
  });

  ['{', '[]', 'null', '{"title":123}', '{"title":{"a":1}}'].forEach((body) => {
    it(`A10: the malformed body ${body} gets a 400 and leaks nothing`, () => {
      cy.request({
        method: 'POST',
        url: `${api}/todos`,
        headers: { 'Content-Type': 'application/json' },
        body,
        failOnStatusCode: false,
      }).then((res) => {
        expect(res.status).to.eq(400);
        const text = JSON.stringify(res.body);
        expect(text).not.to.contain('StackTrace');
        expect(text).not.to.contain('   at ');
        expect(text).not.to.contain('TodoApi.');
      });
    });
  });

  it('A10: unknown ids and unknown routes give a plain 404', () => {
    ['/todos/999999', '/todos/abc', '/nothing-here'].forEach((path) => {
      cy.request({ url: `${api}${path}`, failOnStatusCode: false }).its('status').should('eq', 404);
    });
  });

  it('A02: there are no forgotten files or debug pages', () => {
    ['/swagger', '/.env', '/appsettings.json', '/api/v2/todos'].forEach((path) => {
      cy.request({ url: `${root}${path}`, failOnStatusCode: false }).its('status').should('eq', 404);
    });
  });

  it('A02: a foreign website origin gets no CORS permission', () => {
    cy.request({
      method: 'OPTIONS',
      url: `${api}/todos`,
      headers: { Origin: 'http://evil.example', 'Access-Control-Request-Method': 'POST' },
      failOnStatusCode: false,
    }).then((res) => {
      expect(res.headers['access-control-allow-origin']).to.be.undefined;
    });
  });

  it('A02: the Angular origin is allowed', () => {
    cy.request({
      method: 'OPTIONS',
      url: `${api}/todos`,
      headers: { Origin: 'http://localhost:4200', 'Access-Control-Request-Method': 'POST' },
      failOnStatusCode: false,
    }).then((res) => {
      expect(res.headers['access-control-allow-origin']).to.eq('http://localhost:4200');
    });
  });

  // A known gap: remove .skip after you add the middleware of section 3.
  it.skip('A02: responses carry the security headers', () => {
    cy.request(`${api}/todos`).then((res) => {
      expect(res.headers['x-content-type-options']).to.eq('nosniff');
      expect(res.headers['x-frame-options']).to.eq('DENY');
      expect(res.headers).to.have.property('referrer-policy');
    });
  });

  it('A01: DOCUMENTED GAP, the API has no authentication, so CORS protects nothing', () => {
    cy.addTodoViaApi('A private note');

    cy.request({ url: `${api}/todos`, headers: { Origin: 'http://evil.example' } }).then((res) => {
      expect(res.status).to.eq(200);
      expect(res.body).to.have.length(1);
    });
  });
});