# ContentFlow
CMS многоуровневая, модульная, с поддержкой ролей, аудита, кэширования, очередей, безопасного хранения данных и API.

Перед запуском: 

1. Перейти по пути - .\ContentFlow\ContentFlow.Web\
2. Найти файл - appsettings.json
3. В нем указать актуальные данные для подключения к БД:
	- Server - Имя сервера
	- Database - имя БД (можно оставить уже имеющееся значение)
	- User Id - login для входа 
	- Password - пароль
	- Остальные пункты оставляем по умолчанию.
4. В папке (в терминале) .\ContentFlow\ContentFlow.Web\ выполнить - "dotnet run"

Swagger - http://localhost:8080/swagger/index.html
Hangfire dashboards - http://localhost:8080/hangfire