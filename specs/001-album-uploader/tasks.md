# Implementation Tasks: Album Uploader (001-album-uploader)

This file lists the implementation tasks for the `001-album-uploader` feature. Tasks are grouped into phases and ordered by priority. Tasks that can run in parallel are marked `[P]`.

Phase: Setup

1. Verify local dev environment
	- Confirm `frontend/.env.local`, `vite.config.ts` (envDir), and dev ports (frontend 5173, backend 7071).
	- Ensure Azurite is running for blob emulation if required.

2. Confirm backend local settings and CORS
	- Verify `backend/local.settings.json` contains `AzureFunctionsJobHost__Host__Cors__AllowedOrigins` including `http://localhost:5173`.
	- Restart Functions host after changes.

3. Project housekeeping
	- Ensure `.gitignore` / `.dockerignore` contain common patterns for Node/.NET (create or verify as needed).

Phase: Tests (write tests before or alongside implementation) [P]

4. Add unit tests for `CreateAlbumHandler`
	- Validate command handling of new fields: `AllowUploads`, `MaxFileSizeMb`, `AlbumSizeTshirt`, retention/expiry.
	- Add negative tests for invalid max file size and disallowed uploads.

5. Add unit tests for `QuotaService` and `FileValidationService` (if missing)
	- Verify album caps for T-shirt sizes and file size enforcement.

Phase: Core Implementation

6. Expand `CreateAlbumRequest` model (backend)
	- Add properties: `bool AllowUploads`, `int? MaxFileSizeMb`, `string? AlbumSizeTshirt`, `int? RetentionDays` (or `DateTime? ExpiresAt` if preferred).
	- File: `backend/PicHub.AlbumUploader/Models/CreateAlbumRequest.cs`

7. Update CQRS command & handler
	- Update `CreateAlbumCommand` signature to include the new fields and update `CreateAlbumHandler` to map and persist them.
	- Files: `Services/Cqrs/Commands/CreateAlbumCommand.cs`, `Services/Cqrs/Commands/CreateAlbumHandler.cs`

8. Update repository persistence
	- Ensure `IAlbumRepository` and `AlbumRepository` persist the new fields to the database (columns already exist in migrations). Update mapping code if needed.
	- Files: `Services/AlbumRepository.cs` (or equivalent repository implementations).

9. Update HTTP function to accept new payload
	- Update `Functions/AlbumsFunction.cs` create endpoint to bind the new `CreateAlbumRequest` fields into the command.

Phase: Integration & Verification

10. Restart Functions host and verify CORS and endpoints
	 - Use `curl` or Postman to send an `OPTIONS` preflight and a `POST` to `/api/admin/albums` from origin `http://localhost:5173`.

11. Add integration test for admin create album
	 - Test end-to-end: POST create album with new fields → repository stores values → GET album returns same values.
	 - Location: `tests/integration/` or `backend/AlbumUploader.Tests/Integration`

12. Verify frontend integration
	 - Confirm `frontend/src/pages/admin.ts` sends the new fields in the create POST and that the UI behavior is correct.
	 - (Frontend changes already present; validate against backend).

Phase: Polish & CI

13. Update `quickstart.md` and `README` with updated local dev steps
	 - Include instructions to start Azurite, start Functions, and start the frontend; document ports and env overrides.

14. Add CI items and gating
	 - Add unit and integration test steps to CI; run linters and formatters.

15. Final review and merge
	 - Ensure tests pass, update PR description with implementation notes, request reviewers, and merge.

Notes
- Migrations: DB migrations already include `AllowUploads`, `MaxFileSizeMB`, `AlbumSizeTshirt`. Ensure the repository mapping uses matching property names.
- Choice between `RetentionDays` vs `ExpiresAt`: pick whichever fits existing code patterns (existing models mention retention policy days).

``` 
<snip>
