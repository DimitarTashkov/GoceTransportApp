# План за Интеграция на Google Maps Спирки в Маршрутите

## 1. Промени в Базата Данни (Data Layer)
* **Entity:** Обнови `Route.cs` в `Data.Models`.
* **Полета:** - `public string? StopName { get; set; }` (напр. "Автогара Юг - сектор 5")
    - `public double? Latitude { get; set; }`
    - `public double? Longitude { get; set; }`
* **Миграция:** Генерирай и приложи миграция `AddRouteCoordinates`.

## 2. Обновяване на ViewModels (Web Layer)
* Добави тези полета в `RouteInputModel` и `RouteDetailsViewModel`.
* Увери се, че координатите са `Nullable`, за да не гърми системата за стари маршрути.

## 3. UI за Администратори (Create/Edit Route)
* В изгледа за създаване на маршрут, добави секция "Местоположение на спирката".
* Интегрирай малък Google Maps JavaScript фрагмент. При клик върху картата, скрити полета (`input type="hidden"`) за Lat и Lng трябва да се попълват автоматично.

## 4. Визуализация за Пътници (Route/Schedule Details)
* В страницата за търсене или детайли на разписание, добави икона "Виж на картата".
* При клик, отвори малък Modal или Iframe с Google Maps, центриран върху координатите на спирката с маркер (Pin).
* Използвай Google Maps URL схемата за бутон "Навигация до тук", който отваря Google Maps приложението на телефона на пътника.

## 5. Сигурност и Docker
* **API Key:** Не хардкодвай Google Maps API ключа. Сложи го в `appsettings.json` и го подавай през Secret Manager или Environment Variables в Docker.