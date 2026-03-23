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
