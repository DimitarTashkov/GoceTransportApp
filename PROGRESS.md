# Текущ прогрес на проекта

## GOCE_TRANSPORT_PRODUCTION_LAUNCH_PLAN.md — Стъпки 7–16 (2026-03-31) — ЗАВЪРШЕНИ

### Стъпка 14: SEO — ЗАВЪРШЕНА
* `wwwroot/robots.txt` — Allow public pages, Disallow admin/personal paths, Sitemap pointer
* `wwwroot/sitemap.xml` — статични, discovery и partner URL-и с changefreq и priority
* `_Layout.cshtml` — `<meta name="description">`, canonical URL, Open Graph (og:title/description/image/url), Twitter Card тагове
* `Home/Index.cshtml` + `Schedule/Search.cshtml` — per-page `ViewData["MetaDescription"]` зададен
* Commit: `4755722`

### Стъпка 15: GDPR — ЗАВЪРШЕНА
* `Home/Privacy.cshtml` — пълна GDPR политика: данни, правно основание (GDPR чл. 6), периоди на съхранение, споделяне, права на субектите, контакт с КЗЛД, бисквитки
* `Home/Terms.cshtml` — нова страница с Общи условия: акаунти, билети, анулиране 24ч, организационни лимити, забранено поведение, отговорност, приложимо право
* `HomeController` — нов `Terms()` екшън
* `_Layout.cshtml` footer — "Terms of Service" вече сочи към `Home/Terms` (не `Home/AboutUs`)
* Commit: `c6d19af`

### Стъпка 16: Authorization одит — ЗАВЪРШЕНА
* `OrganizationController.Create [POST]` — добавена проверка на MembershipTier лимит (преди беше само в GET). Предотвратява заобикаляне чрез директен POST
* `ScheduleController.Board [POST]` — вече имаше `HasUserCreatedOrganizationAsync` проверка ✓
* Commit: `935835c`

---

## GOCE_TRANSPORT_PRODUCTION_LAUNCH_PLAN.md — Стъпки 7, 8, 9 (2026-03-31)

### Стъпка 7 + 9: Security Hardening — ЗАВЪРШЕНА

#### 9.3 SecurityHeadersMiddleware
* Нов файл `Web/Middleware/SecurityHeadersMiddleware.cs`
* Headers: `X-Content-Type-Options: nosniff`, `X-Frame-Options: DENY`, `X-XSS-Protection`, `Referrer-Policy`, `Permissions-Policy`, `Content-Security-Policy`
* Регистриран в `Program.cs` преди `UseHttpsRedirection()`

#### 9.2 Rate Limiting
* `AddRateLimiter` в `Program.cs`: Global 100 req/min, Login 10/5min policy, Purchase 20/min policy
* `app.UseRateLimiter()` преди `UseMiddleware<SecurityHeadersMiddleware>()`
* `[EnableRateLimiting("purchase")]` добавен на `TicketController.Purchase [POST]`

#### 7.4 + 9.8 Identity Security
* `IdentityOptionsProvider`: Password — RequireDigit/Lowercase/Uppercase=true, RequiredLength=8
* Lockout: MaxFailedAccessAttempts=5, LockoutTimeSpan=15min, AllowedForNewUsers=true
* `ConfigureApplicationCookie`: HttpOnly=true, SecurePolicy=Always, SameSite=Lax, ExpireTimeSpan=30d, SlidingExpiration=true

### Стъпка 8: UX/UI — Пътникът на първо място — ЗАВЪРШЕНА

#### 8.4 Role-based Navigation
* `IOrganizationService` — нов метод `UserHasOrganizationsAsync(string userId)` (AnyAsync заявка)
* `_LoginPartial.cshtml` — показва различни бързи nav линкове по роля:
  * Owner/Admin: `Dashboard` + `My Organizations` в navbar
  * Passenger: `My Tickets` + `Favorites` в navbar
  * Dropdown съдържа подходящи линкове за всяка роля
* `_Layout.cshtml` — "Get Started" бутонът → "Find a Bus" (само за неаутентикирани), води към Schedule/Index

#### 8.1 Landing Page Redesign + Polish (caaf3ae)
* `Home/Index.cshtml` — пълно пренаписване: ред на секции: Hero → Next Departures → Stats → How It Works → Features → Quick Access → Partner CTA → Info Cards
* Search форма с `id="searchForm"`, `id="fromCitySelect"`, `id="toCitySelect"` за JS таргетиране
* **Popular Routes pills** — динамични бутони от BD (до 4 двойки градове), JS click → pre-fill form + submit
* **Next Departures widget** — AJAX partial view с 60s auto-refresh, spinner при зареждане, луна иконка за "no results"
* Partner CTA декоративни мини-карти скрити на мобилни (`d-none d-lg-block`)
* `loading="lazy"` на illustration image
* BG преводи: `Index.bg.resx` + `_NextDeparturesPartial.bg.resx` — всички нови низове преведени
* Commit: `caaf3ae`

#### 8.2 Schedule/Search Redesign
* `ScheduleDataViewModel` — добавено поле `OrganizationName`
* `SearchSchedulesAsync` — добавен `.Include(s => s.Organization)`, `OrganizationName = s.Organization.Name`, сортиране по `Departure` ascending
* `Search.cshtml` — mobile-first списъчни карти:
  * Час на тръгване: `fs-3 fw-bold text-primary` (най-видим елемент)
  * Маршрут + ден + фирма (info hierarchy)
  * "Details" + "Tickets" / "Login to buy" бутони
  * Компактна search форма отгоре
  * Брой резултати и дата

#### 8.11 PWA Manifest
* `wwwroot/manifest.json` — name, short_name, display=standalone, theme_color=#0d6efd
* `_Layout.cshtml` — добавени `<link rel="manifest">`, `<meta name="theme-color">`, iOS `apple-mobile-web-app-*` meta тагове

### Стъпка 8.6: Organization/Details — скрий Vehicles/Drivers за пътници — ЗАВЪРШЕНА (2026-04-01)
* `Organization/Details.cshtml` — Vehicles и Drivers tab-pane div-ове обвити с `@if (isOwnerOrAdmin)` — съдържанието вече не се рендира в HTML за пасажери (вече беше скрито само визуално чрез tab buttons)

### Стъпка 8.5: Schedule/Index прост режим — ЗАВЪРШЕНА (2026-04-01)
* **Collapsible filters на мобилни** — добавен "Filters" toggle бутон (видим само на `d-md-none`), filter panel обвит с Bootstrap `collapse d-md-block` — на desktop е винаги видим, на мобилни е скриван/показван при клик
* **Timeline/List view** — добавен toggle бутон (grid/list иконки) в sorting bar; при избор на list изгледа се показва `#listView` — компактни редове: час на тръгване (голям/bold), маршрут, ден, превозвач; при grid — стандартните карти
* View preference се записва в `localStorage` и се възстановява при зареждане

### Стъпка 9.4: Input Validation одит — ЗАВЪРШЕНА (2026-04-01)
* `EntityValidationConstants.cs` — добавени нови константи: `DriverConstants.MinExperienceLength/MaxExperienceLength`, `ScheduleConstants.MinDayLength/MaxDayLength`
* `DriverInputModel.cs` — добавени `[MinLength]`/`[MaxLength]` на `DrivingExperience`
* `ScheduleInputModel.cs` + `EditScheduleInputModel.cs` — добавени `[MinLength]`/`[MaxLength]` на `Day`

### Стъпка 9.5: Authorization одит — ЗАВЪРШЕНА (2026-04-01)
* Всички 6 контролера (Route, Vehicle, Driver, Ticket, Schedule, Organization) вече имат `HasUserCreatedOrganizationAsync` проверка на GET и POST Edit/Delete — потвърдено при одит

### Premium Visual Redesign — ЗАВЪРШЕНА (2026-04-01)

#### TASK 1: site.css — добавен SECTION 3 — PREMIUM VISUAL DESIGN SYSTEM
* `.text-gradient` / `.text-gradient-light` — gradient clip text (blue→purple / light blue→lavender)
* `.icon-gradient`, `.icon-gradient-green`, `.icon-gradient-orange`, `.icon-gradient-teal` — gradient icon backgrounds
* `.section-hero` — deep blue-to-purple gradient с две декоративни псевдо-елемент сфери
* `.section-stats` — тъмен gradient band с точков SVG pattern overlay
* `.card-glass` — glassmorphism (backdrop-filter blur + полупрозрачна граница) за тъмни секции
* `.section-badge` — pill badge с gradient tint и uppercase tracking
* `.step-number` / `.steps-row` — numbered gradient circles с desktop connector line
* `.quick-card` — spring-bounce hover lift + gradient border-top reveal + icon rotate
* `.feature-icon` — 52px rounded square с spring-bounce icon hover
* `.section-cta` — тъмен gradient CTA section с декоративна сфера
* `.route-pill` — glass-style pills за популярни маршрути в hero секцията
* `.search-card` — бяла карта с дълбока сянка за floating search form
* `.departures-card` — left-border акцент с translateX hover
* `.navbar-premium` — gradient border-bottom (blue→purple) вместо shadow-sm
* `.stat-number` — 2.5rem/900 weight за hero stats
* `.live-badge` — pulse box-shadow анимация
* `.float-card` — 4s bob анимация за search card
* Reduced-motion guard за всички нови анимации

#### TASK 2: Home/Index.cshtml — пълно пренаписване с premium дизайн
* Hero: `.section-hero` gradient card, floating `.search-card.float-card`, `.live-badge` section-badge, `.text-gradient-light` за "Journey", glassmorphism quick-access pills, `.route-pill` за популярни маршрути
* Stats: `.section-stats` тъмна лента с `.card-glass` карти и `.stat-number.text-gradient-light`
* Next Departures: gradient card-header/footer, `col-lg-10` (от col-lg-9)
* How It Works: `.section-badge` + `.steps-row` с `.step-number` gradient circles + `.step-item`
* Features: лилав gradient background секция, `.section-badge`, `.feature-icon` с 4 gradient варианта, `.feature-item` hover
* Quick Access: `.section-badge`, `.quick-card` с `.qa-icon` gradient circles (4 цветови варианта)
* Partner CTA: `.section-cta` gradient section, `.card-glass` мини-карти в дясната колона
* Info Cards: gradient accent `border-top`, gradient icon circles, gradient Contact бутон

#### TASK 3: _Layout.cshtml — 3 целеви промени
* `<body>` — премахнат `bg-body-tertiary` (позволява hero градиентите да "дишат" без tertiary background)
* `<main>` — `py-5` → `py-4` (по-компактен spacing)
* `<nav>` — `shadow-sm` → `navbar-premium` (gradient долна граница)

### Следващи стъпки от плана (невзети):
* **Стъпки 10-22** от плана (Performance, Monitoring, Email, Deployment, Launch Checklist)



## Ticket Purchase + Manage локализация — ЗАВЪРШЕНО (2026-03-27)

### 1. Покупка на билет за пасажери
* `ITicketService` + `TicketService` — нов метод `PurchaseTicketAsync(string userId, Guid ticketId)`: създава нов `UserTicket` (или увеличава `AvailableTickets` ако вече съществува)
* `TicketController` — нов `[HttpPost] Purchase(string? id, string organizationId)` action: извиква `PurchaseTicketAsync`, redirect → MyTickets
* `_TicketDetailsPartial.cshtml` — добавен бутон "Buy Ticket" (зелен, с иконка shopping-cart) видим само за логнати НЕ-собственици/НЕ-admin; всички текстове локализирани с `@Localizer`
* `Organization/Details.cshtml` Tickets tab — добавен бутон "Buy" (зелен, малък) до всеки билет за логнати пасажери

### 2. Локализация на Identity Manage pages
* `_ViewImports.cshtml` — поправен: добавени и двата namespace-а (`testappHelper.` за моделите + `GoceTransportApp.Web.` за `ManageNavPages`) + `@inject IViewLocalizer Localizer`
* `Index.cshtml` — всички текстове с `@Localizer["..."]`
* `ChangePassword.cshtml` — всички текстове с `@Localizer["..."]`
* `Email.cshtml` — всички текстове с `@Localizer["..."]`
* `PersonalData.cshtml` — всички текстове с `@Localizer["..."]`
* `_ManageNav.cshtml` — навигационните линкове с `@Localizer["..."]`
* **5 `.bg.resx` файла** в `Resources/Areas/Identity/Pages/Account/Manage/`:
  `Index.bg.resx`, `ChangePassword.bg.resx`, `Email.bg.resx`, `PersonalData.bg.resx`, `_ManageNav.bg.resx`



## Identity Layout + Локализация — ЗАВЪРШЕНА (2026-03-26)
* `Areas/Identity/Pages/_ViewStart.cshtml` — Layout сменен от `_AdminLayout` на `/Views/Shared/_Layout.cshtml` (глобалния layout с navbar и footer)
* `Areas/Identity/Pages/_ViewImports.cshtml` — поправени неверни `CinemaApp.*` namespace-и на `GoceTransportApp.*`; добавен `@using Microsoft.AspNetCore.Mvc.Localization`
* `Login.cshtml` — добавен `@inject IViewLocalizer Localizer`; всички UI текстове преведени
* `Register.cshtml` — добавен `@inject IViewLocalizer Localizer`; всички UI текстове преведени
* **2 `.bg.resx` файла** в `Resources/Areas/Identity/Pages/Account/`: `Login.bg.resx`, `Register.bg.resx`
* Добавен `py-5` padding за центрирано вертикално позициониране в рамките на глобалния layout

## Premium UI/UX Redesign — ЗАВЪРШЕН (2026-03-25)
Изпълнени 6-те точки от `ultimate_ui_ux_dashboards_plan.md`:

### 1. Глобална Навигация и Хедър
* `_LoginPartial.cshtml` — премахнат "Hello user@email.com!", заменен с профилен dropdown (аватар + "My Account") с бърз достъп до Dashboard, Organizations, Trips, Favorites, Settings, Admin секция и Logout
* `_SelectLanguagePartial.cshtml` — флагове само (emoji), без текстови лейбъли в навигацията; dropdown стилизиран с shadow и rounded corners
* Добавен "Get Started" CTA бутон в navbar (водещ към Organizations)
* Login/Register за неаутентикирани потребители са pill бутони

### 2. Начална Страница (Landing Page)
* Нов Hero layout: лява колона с headline + CTA бутони, дясна колона с Trip Search формата (card)
* Stats bar секция: 500+ Routes, 50+ Organizations, 100+ Cities, 10K+ Tickets
* Нова Partner CTA секция: "Own a Transport Company?" с бутони "Register Organization" и "View Plans" + 4 feature mini-cards
* Запазени: How It Works, Features, Quick Access, CTA, About/Contact cards

### 3. Потребителско Табло (User Dashboard)
* Нов `DashboardController` (user-facing, не Admin)
* Нов `UserDashboardViewModel` в `Web.ViewModels/Dashboard/`
* Нов `Views/Dashboard/Index.cshtml`:
  * Лява колона: профилна карта (аватар, име, email, Membership Tier badge), Quick Links
  * Дясна колона: 4 stats cards (Upcoming, Past, Organizations, Favorites) + табове (Upcoming Trips, Favorites, History)
* Достъпен от профилния dropdown в навигацията

### 4. Табло за Организации (Organization Dashboard)
* `Organization/Details.cshtml` — добавени 4 metric widgets за owner/admin:
  * Active Routes, Vehicles, Drivers, Tickets (цветни кръгове с икони и числа)
  * Показват се само за собственик или администратор
* Empty states сменени от emoji към Font Awesome икони

### 5. Страница за Абонаменти (Pricing)
* `Organization/Upgrade.cshtml` — пълен редизайн:
  * Badge header "Pricing", нов headline "Choose Your Plan"
  * Всяка карта с кръгла иконка (paper-plane, rocket, crown)
  * Подзаглавия (For individual operators, For growing businesses, For large fleets)
  * Enterprise с "Custom Branding" feature
  * Нова Compare Plans таблица с feature matrix (checkmarks/crosses)

### 6. Професионален Footer
* `_Layout.cshtml` footer → bg-dark с 4 колони:
  * Brand & Mission + social media icons
  * For Passengers (Find a Trip, My Trips, Routes, Contact)
  * For Partners (Register Organization, Pricing, Dashboard, Organizations)
  * Contact (адрес, email, телефон, About Us бутон)
* Copyright + Privacy Policy / Terms of Service линкове

## Пълна локализация (en-US / bg-BG) — ЗАВЪРШЕНА (2026-03-24)
Всички Razor view файлове в приложението са локализирани. При избор на 🇧🇬 BG от езиковия превключвател, целият UI се превключва на Български.

### Обхват — 55+ view файла + 55+ `.bg.resx` resource файла:
* **Shared:** `_Layout.cshtml`, `_LoginPartial.cshtml`, `_SelectLanguagePartial.cshtml`
* **Organization (13):** Index, Create, Edit, Delete, Details, UserOrganizations, Favorites, Upgrade, Routes, Drivers, Vehicles, Schedules, Tickets
* **Schedule (7):** Index, Search, Create, Edit, Delete, Details, Passengers
* **Ticket (6):** Index, Create, Edit, Delete, Details, MyTickets
* **Route (5):** Index, Create, Edit, Delete, Details
* **Vehicle (5):** Index, Create, Edit, Delete, Details
* **Driver (5):** Index, Create, Edit, Delete, Details
* **City (5):** Index, Create, Edit, Details, AddStreet
* **Street (3):** Index, Create, Edit
* **ContactForm (4):** Index, Create, Details, Delete
* **Home (3):** Index, AboutUs, Privacy

### Инфраструктура:
* `CultureController.SetCulture [POST]` — записва избрания език в куки (1 година)
* `_SelectLanguagePartial.cshtml` — dropdown 🇬🇧/🇧🇬 в navbar-а с активен маркер
* `Program.cs` — `AddLocalization`, `AddViewLocalization`, `UseRequestLocalization` (default: en-US)
* Resource файлове в `Resources/Views/<Controller>/<View>.bg.resx`



## Локализация Schedule и Ticket views — ЗАВЪРШЕНА (2026-03-24)
* **13 view файла** обновени с `@using Microsoft.AspNetCore.Mvc.Localization` + `@inject IViewLocalizer Localizer`
* **Schedule views (7):** `Index`, `Search`, `Create`, `Edit`, `Delete`, `Details`, `Passengers`
* **Ticket views (6):** `Index`, `Create`, `Edit`, `Delete`, `Details`, `MyTickets`
* Всички hardcoded UI текстове (h1/h2, бутони, лейбъли, table headers, empty-state съобщения, sort options, badge labels, confirm dialogs) заменени с `@Localizer["..."]`
* **13 `.bg.resx` файла** създадени в `Resources/Views/Schedule/` и `Resources/Views/Ticket/`
* C# блокове (`ViewData["Title"]`, helper variables, sort tuples values използват `.Value` на IHtmlLocalizedString) НЕ са засегнати; asp-* tag helpers запазени непроменени
* Специфика: сортиращите опции в `Schedule/Index.cshtml` използват `Localizer["Sort: Default"].Value` за вграждане в C# tuple

## Локализация Organization views — ЗАВЪРШЕНА (2026-03-24)
* **13 view файла** обновени с `@using Microsoft.AspNetCore.Mvc.Localization` + `@inject IViewLocalizer Localizer`:
  `Index`, `Create`, `Edit`, `Delete`, `Details`, `UserOrganizations`, `Favorites`, `Upgrade`, `Routes`, `Drivers`, `Vehicles`, `Schedules`, `Tickets`
* Всички hardcoded UI текстове (h1/h2, бутони, лейбъли, placeholders, table headers, tab labels, empty-state съобщения, review форма) заменени с `@Localizer["..."]`
* **13 `.bg.resx` файла** създадени в `Resources/Views/Organization/`
* C# блокове (`ViewData["Title"]`, helper variables) НЕ са засегнати; asp-* tag helpers запазени непроменени

## Локализация Route / Vehicle / Driver views — ЗАВЪРШЕНА (2026-03-24)
* **15 view файла** обновени с `@using Microsoft.AspNetCore.Mvc.Localization` + `@inject IViewLocalizer Localizer`
* Всички hardcoded UI текстове (h1/h2, бутони, лейбъли, placeholders, table headers, empty-state съобщения) заменени с `@Localizer["..."]`
* **15 `.bg.resx` файла** създадени (нова директорийна структура `Resources/Views/Route|Vehicle|Driver/`)
  * `Route/Index.bg.resx`, `Create.bg.resx`, `Edit.bg.resx`, `Delete.bg.resx`, `Details.bg.resx`
  * `Vehicle/Index.bg.resx`, `Create.bg.resx`, `Edit.bg.resx`, `Delete.bg.resx`, `Details.bg.resx`
  * `Driver/Index.bg.resx`, `Create.bg.resx`, `Edit.bg.resx`, `Delete.bg.resx`, `Details.bg.resx`
* C# блокове (`ViewData["Title"]`, helper variables) НЕ са засегнати; asp-* tag helpers запазени непроменени

## advanced_features_and_localization_plan.md — ЗАВЪРШЕН (всички 4 фази)

### Фаза 1: Любими Организации (Favorites)
* **Нов модел:** `UserFavoriteOrganization` (Many-to-Many) с composite PK (UserId + OrganizationId)
* **EF миграция:** `AddAdvancedFeatures` — таблица `UserFavoriteOrganizations` + `MembershipTier` колона в `AspNetUsers`
* **Сервис:** `ToggleFavoriteAsync`, `IsOrganizationFavoriteAsync`, `GetFavoriteOrganizationsByUserIdAsync` в `OrganizationService`
* **UI:** Бутон "Follow/Unfollow" в `Organization/Details.cshtml` (само за не-собственици); нова страница `Organization/Favorites.cshtml` с card grid
* **Navbar:** Линк "Favorites" (сърчице) в `_LoginPartial.cshtml`

### Фаза 2: Membership Tiers
* **Нов enum:** `MembershipTier` (Free=1, Pro=3, Enterprise=999) в `Data.Models/Enumerations/`
* **ApplicationUser:** Добавено `MembershipTier` свойство с default `Free`
* **Бизнес логика:** В `OrganizationController.Create [GET]` — брои съществуващите организации и сравнява с `(int)user.MembershipTier`; ако лимита е достигнат → redirect към `Upgrade`
* **UI:** Нова страница `Organization/Upgrade.cshtml` с красиви pricing cards (Free/Pro/Enterprise)

### Фаза 3: Tickets (верифицирана)
* My Tickets + My Trips вече съществуват от предишни сесии; нова функционалност не е нужна

### Фаза 4: Локализация (en-US / bg-BG)
* **Program.cs:** `AddLocalization(ResourcesPath="Resources")`, `AddViewLocalization()`, `AddDataAnnotationsLocalization()`, `UseRequestLocalization()` с default `en-US`
* **CultureController:** POST action `SetCulture` — записва избрания език в куки (`CookieRequestCultureProvider`)
* **_SelectLanguagePartial.cshtml:** Dropdown 🇬🇧/🇧🇬 с form → POST към CultureController; показва текущия език
* **Resource файлове:** `Resources/Views/Shared/_Layout.bg.resx` + `_LoginPartial.bg.resx` с пълни превод на менюто на Български
* **_Layout.cshtml + _LoginPartial.cshtml:** Всички navbar линкове използват `@Localizer["..."]`
* **Резултат:** При избор на 🇧🇬 BG — менюто се превключва на Български; при 🇬🇧 EN — English (запазено в куки за 1 година)



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

## Локализация City / Street / ContactForm / Home views — ЗАВЪРШЕНА (2026-03-24)
* **14 view файла** обновени с `@using Microsoft.AspNetCore.Mvc.Localization` + `@inject IViewLocalizer Localizer`
* **City views (5):** `Index`, `Create`, `Edit`, `Details`, `AddStreet`
  * Забележка: `City/Delete.cshtml` не съществуваше в проекта (не е намерен при Glob); пропуснат
* **Street views (3):** `Index`, `Create`, `Edit`
  * Забележка: `Street/Delete.cshtml` и `Street/Details.cshtml` не съществуват в проекта
* **ContactForm views (4):** `Index`, `Create`, `Details`, `Delete`
* **Home views (3):** `Index`, `AboutUs`, `Privacy`
* Всички hardcoded UI текстове заменени с `@Localizer["..."]`
* **14 `.bg.resx` файла** създадени в:
  * `Resources/Views/City/` — `Index.bg.resx`, `Create.bg.resx`, `Edit.bg.resx`, `Details.bg.resx`, `AddStreet.bg.resx`
  * `Resources/Views/Street/` — `Index.bg.resx`, `Create.bg.resx`, `Edit.bg.resx`
  * `Resources/Views/ContactForm/` — `Index.bg.resx`, `Create.bg.resx`, `Details.bg.resx`, `Delete.bg.resx`
  * `Resources/Views/Home/` — `Index.bg.resx`, `AboutUs.bg.resx`, `Privacy.bg.resx`
* `Home/Index.cshtml` — всички секции (hero, trip search, how it works, features, quick access, CTA, about/contact cards) локализирани
* `Home/AboutUs.cshtml` — всички параграфи, заглавия и list items преведени на Български
* C# блокове (`ViewData["Title"]`) НЕ са засегнати; asp-* tag helpers запазени непроменени

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
