# PersonalNotesApi

REST API для управления личными заметками.  
Бэкенд на .NET + PostgreSQL, предназначен для хранения, создания, редактирования и удаления заметок пользователями.

---

## Архитектура и структура

- Controllers/ — веб-контроллеры, принимают HTTP-запросы  
- Services/ — бизнес-логика, взаимодействие с репозиториями / данными  
- Data/ — контекст базы данных, репозитории  
- DTOs/ — объекты передачи данных (для API)  
- Models/ — внутренние модели данных (сущности)  
- Migrations/ — миграции базы данных  (далее БД)
- appsettings.json/ appsettings.Development.json` — конфигурация  
- PersonalNotesApi.http — файл с примерами HTTP-запросов  

---

## Установка и запуск

1. Клонировать репозиторий
   git clone https://github.com/maze37/PersonalNotesApi.git
   cd PersonalNotesApi

2. Настроить базу данных PostgreSQL 
   - Создать базу данных (имя, пользователь, пароль)  
   - Настроить подключение к БД в `appsettings.Development.json`

3. Применить миграции  
   dotnet ef database update

4. Запустить проект  
   dotnet run

---

## Требования

- .NET 7.0 или выше
- PostgreSQL 13+  

---

## Безопасность и авторизация

API использует JWT‑аутентификацию (токен Bearer)
