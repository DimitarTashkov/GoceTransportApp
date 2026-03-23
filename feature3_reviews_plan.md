# План за Имплементация: Рейтинг и Отзиви за Организации

## 1. Създаване на Модела (Data Layer)
* **Нов Entity модел:** Създай `Review.cs` в `Data.Models`.
  * Свойства: `Id` (Guid), `OrganizationId` (Guid, Foreign Key), `Organization` (навигационно), `PassengerId` (string, Foreign Key към ApplicationUser), `Passenger` (навигационно), `Rating` (int, от 1 до 5), `Comment` (string, Max 500, Optional), `CreatedOn` (DateTime).
* **DbContext:** Добави `DbSet<Review> Reviews { get; set; }` в `ApplicationDbContext`.
* **Миграция:** Направи и приложи миграция (чрез терминала: `dotnet ef migrations add AddReviews` и `dotnet ef database update`).

## 2. Бизнес Логика (Services Layer)
* **IReviewService / ReviewService:** Създай тези интерфейс и клас.
  * Метод `AddReviewAsync(string userId, string organizationId, int rating, string comment)`.
  * Метод `GetReviewsForOrganizationAsync(string organizationId)` -> връща ViewModel с данните за потребителя, рейтинга, датата и коментара.
* **Валидация (Crucial):** В `AddReviewAsync` провери дали потребителят реално е пътувал с тази организация (проверка в `Tickets`/`UserTickets` за минали дати). Ако не е, хвърли грешка/върни false.

## 3. Обновяване на Изгледите (UI/UX)
* **Модел за детайли:** В `OrganizationDetailsViewModel` добави `double AverageRating` и `IEnumerable<ReviewViewModel> Reviews`.
* **Изглед `Details.cshtml`:**
  * В лявата колона (под логото на организацията) добави звездички (чрез Bootstrap Icons `<i class="bi bi-star-fill text-warning"></i>`) и средния рейтинг (напр. 4.5/5).
  * В дясната колона добави нов Таб: **"Отзиви"**.
  * Вътре в таба покажи списък с коментарите.
  * Най-отгоре в таба добави малка форма (само за логнати потребители): Падащо меню за оценка (1-5 звезди), текстово поле за коментар и бутон "Остави отзив". Формата да сочи към нов метод `[HttpPost] AddReview` в `OrganizationController`.

## 4. Контролер (Controller Layer)
* В `OrganizationController` добави метод `AddReview(string organizationId, int rating, string comment)`.
* Извикай сървиса. Ако върне грешка (потребителят не е пътувал с тях), върни `TempData["ErrorMessage"] = "Може да оставите отзив само ако сте пътували с този превозвач."`.
* Успех: `TempData["SuccessMessage"] = "Благодарим за отзива!"` и редирект към `Details` на организацията.