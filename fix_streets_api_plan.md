# План за Локализиране на Търсенето на Улици (Премахване на външното API)

## 1. Цел на задачата
Заявката за взимане на улици в `loadStreetsInCity.js` се опитва да извика външен WebApi проект на `https://localhost:7119`, което причинява проблеми. Целта е да преместим логиката изцяло в основния MVC проект.

## 2. Обновяване на Контролера
**Файл:** `Web/GoceTransportApp.Web/Controllers/StreetController.cs`

**Стъпки:**
1. Инжектирай `ICityService cityService` в конструктора на контролера и го запази в `private readonly` поле.
2. Добави следния нов екшън метод в контролера:

```csharp
[HttpGet]
[AllowAnonymous]
public async Task<IActionResult> GetStreetsByCity(string id)
{
    if (string.IsNullOrEmpty(id))
    {
        return BadRequest("City ID cannot be null or empty.");
    }

    if (!Guid.TryParse(id, out Guid cityGuid))
    {
        return BadRequest("Invalid City ID.");
    }

    var streets = await this.cityService.GetAllStreetsInCityAsync(cityGuid);
    return Json(streets);
}
Файл: Web/GoceTransportApp.Web/wwwroot/js/loadStreetsInCity.js

Стъпки:
Намери fetch заявката и замени хардкоднатия URL адрес (който сочи към localhost:7119) с относителния път към новия метод в MVC проекта.