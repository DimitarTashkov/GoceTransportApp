# План за Цялостен Тестов Seed (Master Test Scenario)

## 1. Цел на задачата
Създаване на `TestScenarioSeeder.cs`, който да напълни празна база данни с напълно свързани релационни данни: Потребители -> Организации -> Автопарк/Шофьори/Маршрути -> Разписания -> Билети. 

## 2. Имплементация на `TestScenarioSeeder`
**Файл:** `Data/GoceTransportApp.Data/Seeding/TestScenarioSeeder.cs`
* Класът трябва да имплементира `ISeeder`.
* В `SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)` провери: `if (dbContext.Organizations.Any()) return;` (за да не се дублират данни).

**Стъпка 2.1: Потребители (Users)**
* Създай 4 инстанции на `ApplicationUser` (ако не съществуват в базата):
  * 3 собственици: `org1@test.com`, `org2@test.com`, `org3@test.com`.
  * 1 пътник: `passenger@test.com`.
* Използвай `PasswordHasher<ApplicationUser>` (или ги създай през `UserManager`, ако е наличен през `serviceProvider`), за да им зададеш парола `Test1234!`. Запиши ги в базата.

**Стъпка 2.2: Градове (Cities)**
* Провери дали има градове. Ако няма, създай 3 базови: "София", "Пловдив", "Варна". Запиши ги.

**Стъпка 2.3: Организации (Organizations)**
* Създай 3 организации:
  * "Express Lines" -> `FounderId` = ID-то на `org1@test.com`.
  * "Global Trans" -> `FounderId` = ID-то на `org2@test.com`.
  * "Eco Travel" -> `FounderId` = ID-то на `org3@test.com`.
* Сложи им измислени адреси и телефони. Запиши ги.

**Стъпка 2.4: Ресурси и Разписания (The Core Engine)**
* За **всяка** от 3-те организации генерирай по:
  * 1 `Vehicle` (Превозно средство): напр. Капацитет 50, Рег. номер "CB1234AB".
  * 1 `Driver` (Шофьор): напр. Иван Иванов.
  * 1 `Route` (Маршрут): напр. От София до Варна (свържи с ID-тата на градовете), `Duration` = 360 (минути).
  * Запиши ги в базата, за да им се генерират ID-та.
  * 1 `Schedule` (Разписание): Използвай току-що създадените Route, Vehicle и Driver. Задай дата 7 дни напред в бъдещето (`DateTime.UtcNow.AddDays(7)`). Цена: 35.00 лв. Запиши в базата.

**Стъпка 2.5: Билети (Tickets)**
* Вземи ID-то на първото генерирано разписание и ID-то на пътника (`passenger@test.com`).
* Създай 1 `Ticket` (или `UserTicket`) за този пътник.

## 3. Регистрация на Seeder-а
**Файл:** `Data/GoceTransportApp.Data/Seeding/ApplicationDbContextSeeder.cs`
* Добави `new TestScenarioSeeder()` в списъка `seeders`. Увери се, че е добавен **след** базовите сийдъри (като тези за Роли и Градове, ако има такива).