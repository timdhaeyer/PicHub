# PicHub Development Guidelines

Auto-generated from all feature plans. Last updated: 2025-11-27

## Active Technologies

- Frontend: React (TypeScript)
- Backend: .NET 10 (Azure Functions)
- Datastores: Azure Table Storage, Azure SQL, Azure Cosmos DB
- File Storage: Azure Storage Account (Blobs)
- Infrastructure as Code: Bicep
- CI/CD: GitHub Actions

## Project Structure

```text
backend/
frontend/
tests/
```

## Commands (common)

- Frontend: `pnpm install` / `pnpm dev` / `pnpm build` / `pnpm test`
- Backend: `dotnet build` / `dotnet test` / `func start` (Azure Functions Core Tools)
- IaC: `az bicep build --file main.bicep` / GitHub Actions workflows in `.github/workflows/`

## Code Style

- Frontend: TypeScript + ESLint + Prettier configuration; prefer `eslint --fix` and `pnpm format` before commits.
- Backend: Use `dotnet format` and follow Microsoft's .editorconfig conventions; enable nullable reference types and analyzers.

## Security & Privacy

- High focus on secure defaults: encryption at rest and in transit, least-privilege identities for Azure resources, and explicit PII handling guidance.

## Recent Changes

- 001-album-uploader: Added [e.g., Python 3.11, Swift 5.9, Rust 1.75 or NEEDS CLARIFICATION] + [e.g., FastAPI, UIKit, LLVM or NEEDS CLARIFICATION]

<!-- MANUAL ADDITIONS START -->
<!-- MANUAL ADDITIONS END -->
