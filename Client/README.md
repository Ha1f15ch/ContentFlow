# ContentFlow Client

Vue 3 frontend для ContentFlow.

## Настройка окружения

Создайте локальный `.env` из примера:

```sh
cp .env.example .env
```

Для Windows PowerShell:

```powershell
copy .env.example .env
```

Основная настройка:

```env
VITE_API_BASE_URL=http://127.0.0.1:8080/api
```

Если backend запущен на другом порту или хосте, обновите `VITE_API_BASE_URL`.

## Установка

```sh
npm install
```

## Запуск в dev-режиме

```sh
npm run dev
```

## Production build

```sh
npm run build
```

## Preview production build

```sh
npm run preview
```
