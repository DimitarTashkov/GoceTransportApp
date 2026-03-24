# План за Разширени Функционалности: Favorites, Membership и Локализация

## 1. Фаза 1: Любими Организации (Subscribe / Favorites)
**Цел:** Потребителите да могат да "харесват" или "следват" превозвачи и да ги виждат в свой списък.
* **Data Layer:** Създай свързваща таблица `UserFavoriteOrganization` (Many-to-Many) с `UserId` (string) и `OrganizationId` (Guid). Добави `DbSet` в `ApplicationDbContext`.
* **Service Layer:** В `OrganizationService` добави методи: `ToggleFavoriteAsync(userId, orgId)` и `GetFavoriteOrganizationsByUserIdAsync(userId)`.
* **UI/UX:** * В `Organization/Details` добави бутон "Следвай" (или сърчице). Ако потребителят вече я следва, бутонът да е "Отмени следването" (с различен цвят). Бутонът трябва да вика POST екшън.
  * Добави нова страница в менюто (за логнати потребители): **"Любими Превозвачи"**, където се рендерира списъкът с техните запазени организации.

## 2. Фаза 2: Membership Tiers & Лимити за Създаване
**Цел:** Да ограничим колко организации може да създаде даден потребител на базата на неговия план.
* **Data Layer:** Към `ApplicationUser` добави ново свойство `MembershipTier` (Enum: `Free` = 1, `Pro` = 3, `Enterprise` = 999) с дефолтна стойност `Free`.
* **Бизнес Логика:** * В `OrganizationService.CreateAsync` (или в контролера), преди да създадеш организацията, преброй колко организации вече има този `FounderId`.
  * Ако `count >= (int)user.MembershipTier`, хвърли грешка или върни `false`.
* **UI/UX:** * Ако потребителят е достигнал лимита си и кликне "Добави Организация", пренасочи го към нова страница **"Upgrade Plan" (Абонаменти)**, където има красиви ценови картички (Pricing Cards) за Pro и Enterprise.

## 3. Фаза 3: Купуване на Билети (Затвърждаване)
**Цел:** Пътникът трябва да може да си "добави/купи" билет.
* Увери се, че в `Schedule/Details` или търсачката има работещ бутон "Купи билет", който създава запис в `Tickets` (или `UserTickets`), свързвайки `PassengerId` и `ScheduleId`.
* Увери се, че страницата **"Моите пътувания" (My Tickets)** съществува, извлича тези билети и ги показва подредени по дата.

## 4. Фаза 4: Локализация (Български и Английски език)
**Цел:** Пълна i18n поддръжка чрез ресурсите на ASP.NET Core.
* **Program.cs Setup:**
  * Добави `builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");`.
  * Добави `builder.Services.AddControllersWithViews().AddViewLocalization();`.
  * Конфигурирай `RequestLocalizationOptions` с поддържани култури: `en-US` и `bg-BG`. Дефолтната да е `bg-BG`. Включи го в middleware пайплайна (`app.UseRequestLocalization()`).
* **Language Switcher (Превключвател):**
  * Създай `CultureController` с екшън `SetCulture(string culture, string returnUrl)`, който запазва избрания език в куки (`CookieRequestCultureProvider.MakeCookieValue`).
  * Добави Partial View `_SelectLanguagePartial.cshtml` в навигацията (с падащо меню за 🇧🇬 БГ и 🇬🇧 EN), което вика този контролер.
* **Ресурси (Resources):** Създай папка `Resources` и демонстрирай локализацията, като преведеш главното меню в `_Layout.cshtml` (напр. използвайки `@inject IViewLocalizer Localizer` и `@Localizer["Home"]`).

## 5. Процедура на Изпълнение
Като Principal Engineer, изпълни плана фаза по фаза. ЗАДЪЛЖИТЕЛНО генерирай и приложи Entity Framework миграция (`AddAdvancedFeatures`) след Фаза 1 и 2. Не чупи съществуващите E2E тестове.