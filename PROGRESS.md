# Текущ прогрес на проекта

* **Завършено до момента:** Добавени са Dockerfile-ове за Web и API. Направен е редизайн на картите и формите чрез Bootstrap 5. Оправена е типографията и са добавени базови UX анимации в site.css/js. **Секция 2** (Капацитет) и **Секция 3** (Търсачка) от business_logic_plan.md са завършени.
* **Секция 3 детайли:** Добавена форма за търсене в `Home/Index.cshtml`, нов `Schedule/Search` екшън и view, нов метод `SearchSchedulesAsync` в ScheduleService (филтрира по FromCity, ToCity, DayOfWeek от датата).
* **Текущ фокус:** Бизнес логика — `business_logic_plan.md`.
* **Секция 4 детайли:** Добавен `MyTickets [GET]` и `Cancel [POST]` в TicketController. Нови ViewModels: `MyTicketViewModel`, `MyTicketsViewModel`. Нови методи в TicketService: `GetMyTicketsAsync`, `CancelTicketAsync` (24h check). View: `Views/Ticket/MyTickets.cshtml` с две секции (Upcoming/Past). Линк "My Trips" добавен в navbar.
* **Секция 5 детайли:** Добавено `IsBoarded` в `UserTicket` + EF миграция `AddIsBoardedToUserTicket`. ViewModels: `PassengerViewModel`, `PassengerListViewModel`. Методи: `GetPassengersForScheduleAsync`, `BoardPassengerAsync` (toggle). Екшъни: `Passengers [GET]` и `Board [POST]` в `ScheduleController`. View: `Views/Schedule/Passengers.cshtml` с таблица + брояч. Бутон "Passenger List" добавен в `_ScheduleDetails.cshtml`.
* **Секция 6 детайли:** Добавен `GetUserOrganizationIdsAsync` в BaseController. `List<Guid> OrganizationFilter` добавен в `AllVehiclesSearchFilterViewModel` и `AllSchedulesSearchFilterViewModel`. `GetAllDriversAsync` получи `List<Guid>?` параметър. Всички три сервиза прилагат `.Where(Contains)` при зададен филтър. Index екшъните в VehicleController, DriverController, ScheduleController автоматично ограничават данните до организациите на логнатия потребител (Admin вижда всичко).
* **ВСИЧКИ 6 СЕКЦИИ от business_logic_plan.md са завършени.**
* **ВСИЧКИ 4 СЕКЦИИ от master_ux_ui_redirection_plan.md са завършени.**
* **feature3_reviews_plan.md — ЗАВЪРШЕН (всички 4 стъпки).**
* **feature2_emails_plan.md — ЗАВЪРШЕН (всички 4 стъпки).**
* **ux_fixes_plan.md — ЗАВЪРШЕН (всички 3 стъпки).**
* **master_seeder_plan.md — ЗАВЪРШЕН.**
* **advanced_filtering_pagination_plan.md — ЗАВЪРШЕН (всички 4 точки).**
* **ultimate_e2e_self_healing_plan.md — ЗАВЪРШЕН (Фаза 1 + Фаза 2, 8/8 теста минават).**

## advanced_filtering_pagination_plan.md детайли:
* **Нов enum:** `ScheduleSorting` (Default/DepartureAscending/DepartureDescending/ArrivalAscending/ArrivalDescending) — `Web.ViewModels/Schedules/ScheduleSorting.cs`
* **AllSchedulesSearchFilterViewModel** — добавени: `FromCityId`, `ToCityId`, `OrganizationId` (single dropdown), `SortBy`, `AvailableCities`, `AvailableOrganizations`; `EntitiesPerPage` default намален от 20 → 10
* **ScheduleService** — рефакториран с private `BuildFilteredQuery()` helper: прилага всички филтри + сортиране върху IQueryable. `GetSchedulesCountByFilterAsync` вече ползва същия helper (оправен bug: `>=` → `==` за TimeFilter). Двете заявки в Index action се изпълняват паралелно с `Task.WhenAll`.
* **ScheduleController.Index** — зарежда `AvailableCities` и `AvailableOrganizations` за dropdown-ите (Admin вижда всички орг.; User вижда само своите)
* **Views/Schedule/Index.cshtml** — пълен редизайн: 2-колонен layout (col-md-3 ляво филтри + col-md-9 дясно резултати), sorting dropdown с auto-submit, Bootstrap pagination с windowing (±2 страни + ellipsis), всички филтри се запазват в URL при смяна на страница чрез `asp-all-route-data`

## TestScenarioSeeder детайли:
* Файл: `Data/GoceTransportApp.Data/Seeding/TestScenarioSeeder.cs`
* Guard: `if (dbContext.Organizations.Any()) return;` — само на празна база
* **4 потребителя** (парола `Test1234!`): `org1@test.com`, `org2@test.com`, `org3@test.com`, `passenger@test.com`
* **3 града**: Sofia / Plovdiv / Varna + по 1 улица за всеки ("Bus Terminal") с CityStreet link
* **3 организации**: Express Lines (org1), Global Trans (org2), Eco Travel (org3) с `/images/no-organization-image.png`
* **Per org**: 1 Vehicle (Mercedes Travego, capacity 50, уникален номер CB1111AB/2222PB/3333VB) + 1 Driver + 1 Route + 1 Schedule (08:00 след 7 дни)
  * Маршрути: Sofia→Plovdiv (150km/150min), Plovdiv→Varna (250km/240min), Varna→Sofia (440km/360min)
* **1 Ticket + 1 UserTicket** за `passenger@test.com` на Express Lines (цена 35.00 BGN, departure = 7 дни напред)
* Регистриран след `OrganizationSeeder` в `ApplicationDbContextSeeder.cs`
* ⚠️ За да тестваш Reviews: ExpiryDate е = departure date (7 дни напред) — след изтичане пътникът може да остави отзив

## ux_fixes_plan.md детайли:

### Стъпка 1 — Submit бутон freeze fix (site.js)
* Преди `btn.disabled = true` се проверява: ако jQuery Validate е закачен на формата и тя НЕ е валидна → `return` без замразяване
* Бутонът се деактивира САМО когато формата мине валидацията успешно
* Spinner текст сменен от "Обработване..." на "Processing..."

### Стъпка 2 — Route Duration в минути
* `RouteInputModel.Duration` + `EditRouteInputModel.Duration` → добавен `[Display(Name = "Duration (minutes)")]`
* `Route/Create.cshtml` + `Route/Edit.cshtml` → label "Duration (minutes) — e.g. 135", input type="number" min="1"
* `_RouteDetailsPartial.cshtml` → форматиране: `(int)Model.Duration / 60` h. `% 60` min. (напр. "2 h. 15 min.")
* `Organization/Details.cshtml` → Routes таб вече показва форматирана продължителност вместо сурово число

### Стъпка 3 — Tickets таб в Organization/Details
* `GetOrganizationDetailsAsync` → добавени `.ThenInclude(t => t.Route).ThenInclude(r => r.FromCity/ToCity)` за да се зареждат имената на градовете
* `Organization/Details.cshtml` → нов таб "Tickets" с таблица (Route, Issued, Expires, Price) + CRUD бутони за owner/admin + empty state с 🎫

## feature2_emails_plan.md детайли:
* `EmailTemplates.cs` — статичен клас в `Services.Messaging/` с два метода:
  * `GetTicketConfirmationEmail(...)` — красив HTML с inline CSS (route banner, details grid, booking ref)
  * `GetTicketCancellationEmail(...)` — HTML с червен хедър и cancelled route card
* `appsettings.json` — добавена секция `SendGrid: { ApiKey, SenderEmail, SenderName }`; ApiKey = "SG.DummyKey" за dev
* `Program.cs` — логика: ако ApiKey е реален (не "SG.Dummy...") → регистрира `SendGridEmailSender`; иначе → `NullMessageSender`
* `TicketController` — инжектирани `IEmailSender` + `IConfiguration`; два нови private helpers:
  * `SendTicketCreatedEmailAsync` — изпраща на org owner след Create [POST]
  * `SendCancellationEmailAsync` — зарежда ticket details преди CancelTicketAsync, после изпраща cancel email
* Email failure е wrapped в try/catch — никога не счупва user flow

## feature3_reviews_plan.md детайли:
* `Review.cs` entity в `Data.Models` (BaseModel<Guid>, OrganizationId, PassengerId, Rating 1-5, Comment opt.)
* `DbSet<Review> Reviews` в `ApplicationDbContext`
* EF migration `AddReviews` — приложена
* `IReviewService` + `ReviewService` в `Services.Data/Reviews/`
  * `AddReviewAsync`: валидира дали user е пътувал (UserTicket + Ticket.ExpiryDate < Now) + дали вече е дал отзив
  * `GetReviewsForOrganizationAsync`: връща `IEnumerable<ReviewViewModel>` (PassengerName, Rating, Comment, CreatedOn)
  * `GetAverageRatingAsync`: изчислява средния рейтинг с 1 знак след запетаята
* `OrganizationDetailsViewModel` — добавени `double AverageRating` + `IEnumerable<ReviewViewModel> Reviews`
* `OrganizationController.Details` — зарежда reviews и average rating след основния model
* `OrganizationController.AddReview [POST]` — нов екшън с валидация и TempData съобщения
* `IReviewService` регистриран в `Program.cs`
* `Organization/Details.cshtml` — звездички (Bootstrap Icons) + среден рейтинг в лявата колона; нов таб "Reviews" с форма за логнати потребители и списък с отзиви

## Детайли от master_ux_ui_redirection_plan.md:

### Секция 1 — Премиум хедъри на Index страниците
* `Route/Index.cshtml` → "Дестинации и Маршрути"
* `Organization/Index.cshtml` → "Нашите Транспортни Партньори"
* `Vehicle/Index.cshtml` → "Превозни Средства"
* `Driver/Index.cshtml` → "Нашият Екип"
* `Schedule/Index.cshtml` → "Разписания и Курсове"

### Секция 2 — Редизайн на Organization/Details.cshtml
* Лява колона: профилна карта с лого (`rounded-circle`, 120×120px), Редактирай/Изтрий бутони
* Дясна колона: Bootstrap nav-tabs за Маршрути, Автопарк, Шофьори, Разписания с карточки
* `ImageUrl` добавен в `OrganizationDetailsViewModel` + service зарежда related data чрез Include

### Секция 3 — Empty States + форми
* Всички Index странници имат красив `📭` empty state на Bulgarian
* Organization/Edit Cancel бутон вече сочи към Details (не към Index)

### Секция 4 — Логични Редиректи
* `OrganizationService.CreateAsync` вече връща `Task<Guid>`
* `OrganizationController.Create` [POST] → `RedirectToAction(Details, id=newId)`
* `OrganizationController.Edit` [POST] → `RedirectToAction(Details, id=formModel.Id)`
* `VehicleController`, `DriverController`, `RouteController`, `ScheduleController` → след Create/Edit/Delete вече redirect-ват към `Organization/Details/{organizationId}` вместо към sub-list pages

## ultimate_e2e_self_healing_plan.md детайли:
* **Проект:** `Tests/GoceTransportApp.E2ETests` (NUnit + Microsoft.Playwright.NUnit, net10.0)
* **BaseTest.cs** — cookie consent, LoginAsync, GetOrganizationIdAsync(orgName), SubmitFormAsync, AssertSuccessToastAsync
* **Фаза 1 (CRUD):**
  * `OrganizationUITests.cs` — Create + validation error test (2 теста)
  * `VehicleUITests.cs` — Create vehicle към seeded "Express Lines" org (1 тест)
  * `RouteUITests.cs` — Create route с AJAX street loading (1 тест)
* **Фаза 2 (Business Logic):**
  * `ScheduleUITests.cs` — Create schedule с Vehicle/Route dropdowns (1 тест)
  * `TicketCheckoutTests.cs` — OrgOwner create ticket + Passenger MyTickets (2 теста)
* **DiagnosticTests.cs** — диагностичен тест за login + org create flow (1 тест)
* **Bug fix (Self-Healing):** `loadStreetsInCity.js` — AJAX URL беше `/Street/GetStreetsByCity/${cityId}` (route segment), но endpoint-ът очаква query string `?id=`. Оправено на `?id=${cityId}`
