# KmlApiApp

![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-API-512BD4)
![Swagger](https://img.shields.io/badge/Swagger-OpenAPI-85EA2D?logo=swagger)
![KML](https://img.shields.io/badge/KML-Geographic_Data-4285F4)
![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)

## Description

REST API для работы с географическими данными полей из KML-файлов.
Сервис парсит KML с полигонами полей и их центроидами, предоставляя эндпоинты для:

- Получения списка всех полей с координатами полигонов и центров
- Получения площади конкретного поля по ID
- Расчёта расстояния от произвольной точки до центра поля (формула Haversine)
- Определения, в каком поле находится заданная точка (ray casting алгоритм)

---

## API Endpoints

Базовый путь: `/api/fields`

| Метод | Endpoint | Описание | Параметры |
|-------|----------|----------|-----------|
| `GET` | `/api/fields` | Все поля с координатами | — |
| `GET` | `/api/fields/{id}` | Площадь поля по ID | `id` (path) |
| `GET` | `/api/fields/distance` | Расстояние до центра поля | `fieldId`, `lat`, `lng` (query) |
| `GET` | `/api/fields/point-inside` | Проверка принадлежности точки | `lat`, `lng` (query) |

### GET /api/fields

Возвращает список всех полей с полигонами и центроидами.

```json
// Response 200
[
  {
    "id": 1,
    "name": "Поле Северное",
    "size": 125.5,
    "locations": {
      "center": [52.1234, 41.5678],
      "polygon": [
        [52.1200, 41.5600],
        [52.1250, 41.5700],
        [52.1230, 41.5750]
      ]
    }
  }
]
```

### GET /api/fields/{id}

Возвращает площадь поля. `404` если поле не найдено.

```
GET /api/fields/1
// Response 200
125.5
```

### GET /api/fields/distance

Рассчитывает расстояние (в метрах) от точки `(lat, lng)` до центроида поля по формуле Haversine.

```
GET /api/fields/distance?fieldId=1&lat=52.13&lng=41.57
// Response 200
1523.47
```

### GET /api/fields/point-inside

Определяет, принадлежит ли точка какому-либо полю (ray casting).
Возвращает `{ id, name }` или `false`.

```
GET /api/fields/point-inside?lat=52.123&lng=41.567
// Response 200 (точка внутри поля)
{ "id": 1, "name": "Поле Северное" }

// Response 200 (точка вне полей)
false
```

---

## Tech Stack

| Технология | Назначение |
|------------|------------|
| **.NET 9** | Платформа |
| **ASP.NET Core** | Web API фреймворк |
| **Swashbuckle** | Swagger / OpenAPI документация |
| **System.Xml.Linq** | Парсинг KML (XML) |

---

## Quick Start

```bash
# Клонирование
git clone https://github.com/PavelHopson/KmlApiApp.git
cd KmlApiApp

# Восстановление зависимостей
dotnet restore

# Запуск (dev режим с Swagger UI)
dotnet run
```

Swagger UI доступен по адресу: `https://localhost:{port}/swagger`

> Для работы API необходимы KML-файлы `kml/fields.kml` и `kml/centroids.kml` в корне проекта.

---

## Project Structure

```
KmlApiApp/
├── Controllers/
│   └── FieldsController.cs    # API контроллер — эндпоинты полей
├── Models/
│   ├── Field.cs               # Доменная модель поля
│   ├── FieldDto.cs            # DTO для API ответов
│   └── LocationInfo.cs        # Координаты центра и полигона
├── Services/
│   └── KmlService.cs          # Парсинг KML, расчёты расстояний и геопозиции
├── kml/
│   ├── fields.kml             # KML с полигонами полей
│   └── centroids.kml          # KML с центроидами полей
├── Program.cs                 # Точка входа, конфигурация DI и middleware
├── KmlApiApp.csproj           # Файл проекта .NET 9
└── appsettings.json           # Конфигурация приложения
```

---

## License

[MIT](LICENSE) -- 2025 PavelHopson
