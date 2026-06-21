# Contributing to Cortikal

Thank you for your interest in contributing to Cortikal! 🧠

## Development Setup

### Prerequisites
- Node.js ≥ 20.x
- .NET ≥ 10.0
- Rust ≥ 1.75 (for desktop builds)

### Getting Started

```bash
git clone https://github.com/cortikal/cortikal.git
cd cortikal
npm install
```

### Running the Frontend
```bash
cd apps/web
npm run dev
```

### Running the Backend
```bash
cd server/src/Cortikal.Api
dotnet run
```

### Running Tests
```bash
# .NET tests
dotnet test server/Cortikal.sln

# Frontend tests
cd apps/web && npm test
```

## Commit Convention

We follow [Conventional Commits](https://www.conventionalcommits.org/):

- `feat:` — New feature
- `fix:` — Bug fix
- `docs:` — Documentation
- `style:` — Formatting
- `refactor:` — Code restructuring
- `test:` — Tests
- `chore:` — Maintenance

## Code Style

- **TypeScript**: ESLint + Prettier
- **C#**: .editorconfig (4-space indent)
- **Rust**: `cargo fmt`
