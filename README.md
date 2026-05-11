# PDF Processing System

This project is a small system for PDF files.

It can:

- upload PDF files;
- save files on disk;
- save document data in PostgreSQL;
- send a message to RabbitMQ;
- read text from PDF files;
- save text by pages;
- return documents and their text through HTTP API.

## Projects

```text
src/PdfProcessing
|-- PdfProcessing.Api
|   |-- HTTP API
|   |-- background consumer
|   |-- application start and DI
|   |-- Swagger and Serilog
|
|-- PdfProcessing.Application
|   |-- application services
|   |-- DTO models
|   |-- DTO mapping
|   |-- PDF text extractor
|
|-- PdfProcessing.Core
|   |-- domain models
|   |-- interfaces
|   |-- message contracts
|
|-- PdfProcessing.Data
|   |-- EF Core context
|   |-- PostgreSQL repositories
|   |-- file storage
|   |-- data mapping
|   |-- migrations
|
|-- PdfProcessing.Messaging
|   |-- RabbitMQ code
|
|-- PdfProcessing.Application.UnitTests
|-- PdfProcessing.Data.UnitTests
```

## Main Technologies

- .NET 10
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- RabbitMQ
- PdfPig
- AutoMapper
- Serilog
- xUnit
- NSubstitute
- Docker Compose
- GitHub Actions

## How It Works

```text
Client
-> POST /api/documents
-> save PDF file
-> save document with Received state
-> send DocumentUploadedMessage to RabbitMQ
-> background consumer gets the message
-> load document
-> set Processing state
-> read PDF text by pages
-> save page text
-> set Processed state
```

If processing fails, the document state becomes `Error`.

## API

Default local addresses:

```text
http://localhost:5010
https://localhost:7266
```

### Upload PDF

```http
POST /api/documents
Content-Type: multipart/form-data
```

Form field:

```text
file
```

Response: `DocumentDto`.

### Get Documents

```http
GET /api/documents
```

Response: list of `DocumentDto`.

### Get Document Text

```http
GET /api/documents/{id}
```

Response: `DocumentContentDto`.

If the document is processed, the response has text for each page.

## Run Locally

### Requirements

- Docker
- .NET 10 SDK

### Start PostgreSQL and RabbitMQ

```bash
docker compose up -d
```

This starts:

- PostgreSQL: `localhost:5432`
- RabbitMQ: `localhost:5672`
- RabbitMQ UI: `http://localhost:15672`

RabbitMQ login:

```text
guest
```

RabbitMQ password:

```text
guest
```

### Apply Database Migrations

The app does not run database migrations by itself.

Run migrations before you start the API:

```bash
dotnet ef database update --project src/PdfProcessing/PdfProcessing.Data --startup-project src/PdfProcessing/PdfProcessing.Api
```

### Run API

```bash
dotnet run --project src/PdfProcessing/PdfProcessing.Api
```

Swagger:

```text
https://localhost:7266/swagger
http://localhost:5010/swagger
```

## Configuration

Main configuration file:

```text
src/PdfProcessing/PdfProcessing.Api/appsettings.json
```

PostgreSQL connection string:

```text
Host=localhost;Port=5432;Database=pdf_db;Username=postgres;Password=postgres
```

RabbitMQ settings:

```json
{
  "MessageBus": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  }
}
```

Uploaded files are saved in the `Files` folder.

## Tests

Run all tests:

```bash
dotnet test src/PdfProcessing/PdfProcessing.slnx
```

The solution has:

- application unit tests;
- data unit tests;
- AutoMapper tests;
- service tests with NSubstitute;
- repository tests with EF InMemory.

## CI

GitHub Actions workflow:

```text
.github/workflows/ci.yml
```

The pipeline runs on push to `main`.

It does:

- restore packages;
- build the solution;
- run tests.

## Known Limits

- The background worker is inside the API app.
- There is no separate worker service.
- The app does not apply database migrations automatically.
- File save, database save, and message publish are not one transaction.
- RabbitMQ has no retry policy.
- RabbitMQ has no dead-letter queue.
- RabbitMQ starts with blocking async calls.
- Soft delete is only partly done.
- Some reads can still return deleted rows.
- File storage uses local disk.
- File storage needs more work for production.
- There is no login or user security.
- There is no full validation pipeline.

## AI Tools

AI tools used during development:

- OpenAI Codex for unit tests generation;
- OpenAI ChatGPT for exploring new technologies.
