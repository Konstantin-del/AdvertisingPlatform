# Advertising Platform

Веб-сервис для работы с рекламной платформой, построенный на ASP.NET Core 9.0.

## Требования

- .NET 9.0 SDK
- Docker (для запуска через Docker)
- Git

## Запуск проекта

### Вариант 1: Запуск без Docker

1. **Клонирование репозитория**

   ```bash
   git clone <repository-url>
   cd AdvertisingPlatform
   ```

2. **Восстановление зависимостей**

   ```bash
   dotnet restore
   ```

3. **Сборка проекта**

   ```bash
   dotnet build
   ```

4. **Запуск приложения**

   ```bash
   cd AdvertisingPlatform.Presentation
   dotnet run
   ```

   Или запуск из корневой папки:

   ```bash
   dotnet run --project AdvertisingPlatform.Presentation
   ```

5. **Доступ к приложению**
   - HTTP: http://localhost:5127
   - HTTPS: https://localhost:7215
   - Swagger UI: https://localhost:7215/swagger (в режиме Development)

### Вариант 2: Запуск с помощью Docker

#### Использование Docker Compose

1. **Клонирование репозитория**

   ```bash
   git clone <repository-url>
   cd AdvertisingPlatform
   ```

2. **Запуск с помощью Docker Compose**

   ```bash
   docker-compose up --build
   ```

3. **Доступ к приложению**
   - HTTP: http://localhost:8080
   - Swagger UI: http://localhost:8080/swagger (если включен в Production)

#### Использование Docker напрямую

1. **Сборка Docker образа**

   ```bash
   docker build -t advertising-platform .
   ```

2. **Запуск контейнера**

   ```bash
   docker run -p 8080:8080 --name advertising-platform-app advertising-platform
   ```

3. **Доступ к приложению**
   - HTTP: http://localhost:8080

## Структура проекта

- `AdvertisingPlatform.Presentation/` - Веб-API слой (контроллеры, конфигурация)
- `AdvertisingPlatform.Application/` - Бизнес-логика приложения
- `Tests/` - Модульные тесты
- `Dockerfile` - Конфигурация для Docker
- `docker-compose.yml` - Конфигурация для Docker Compose

## API Документация

После запуска приложения в режиме Development, документация API доступна по адресу:

- Swagger UI: https://localhost:7215/swagger
