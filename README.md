# ContentFlow

ContentFlow - модульная CMS с ASP.NET Core backend и Vue/Vite frontend.

## Backend

Перед локальным запуском создайте файл:

```powershell
copy ContentFlow\ContentFlow.Web\appsettings.Development.json.example ContentFlow\ContentFlow.Web\appsettings.Development.json
```

Заполните в `ContentFlow/ContentFlow.Web/appsettings.Development.json` актуальные значения:

- `ConnectionStrings:DefaultConnection` - подключение к SQL Server.
- `EmailSettings` - SMTP-настройки.
- `JwtSettings:Secret` - секрет JWT длиной минимум 32 байта.

`ContentFlow/ContentFlow.Web/appsettings.json` не нужно заполнять реальными данными. Это общий шаблон, а локальные значения берутся из `appsettings.Development.json`.

Запуск backend:

```powershell
dotnet run --project ContentFlow\ContentFlow.Web
```

Полезные URL для локального запуска:

- Swagger: `http://127.0.0.1:8080/swagger/index.html`
- Hangfire Dashboard: `http://127.0.0.1:8080/hangfire`

Hangfire Dashboard доступен только для авторизованных пользователей с ролью `Admin` или `Moderator`.

## Frontend

Перед локальным запуском клиента создайте env-файл:

```powershell
copy Client\.env.example Client\.env
```

По умолчанию API URL:

```env
VITE_API_BASE_URL=http://127.0.0.1:8080/api
```

Запуск frontend:

```powershell
cd Client
npm install
npm run dev
```

## Проверки

Backend:

```powershell
dotnet build ContentFlow\ContentFlow.sln
dotnet test ContentFlow\Tests\ContentFlow.Domain.Tests\ContentFlow.Domain.Tests.csproj
```

Frontend:

```powershell
cd Client
npm run build
```
