# План за Ограничаване на Премиум Функционалности (Membership Gating)

## 1. Логика в MessageService (SignalR Нотификации)
**Цел:** Само Pro/Enterprise потребители да генерират известия в реално време.
* **Файл:** `MessageService.cs`.
* **Промяна:** В метода `CreateNotificationAsync`, преди да запишеш в базата и да извикаш SignalR:
  1. Вземи `ReceiverId` (или `FounderId` на съответната организация).
  2. Провери: `if (founder.MembershipTier == MembershipTier.Free) { return; }`
  3. Така, дори кодът да се опита да прати нотификация, тя ще бъде блокирана "тихо" в сървиса за безплатни потребители.

## 2. Ограничаване на "Live Status" в Графика (Schedule Messages)
**Цел:** Възможността за писане на статуси ("Закъснява", "Анулиран") да е само за поддръжници.
* **Бекенд (`ScheduleController.cs`):**
  * В `[HttpPost] Edit` или специалния екшън за статус:
  * Направи проверка: `if (user.MembershipTier == MembershipTier.Free) { return RedirectToAction("Upgrade", "Organization"); }`
* **Фронтенд (`Schedule/Details.cshtml` или `Edit.cshtml`):**
  * Скрий полето `LiveStatus` или бутона за обновяване с проверка:
    ```html
    @if (User.IsInRole("Administrator") || Model.MembershipTier != MembershipTier.Free) {
        } else {
        <p class="text-muted"><i class="bi bi-lock"></i> Надградете за Live Status</p>
    }
    ```

## 3. Ограничаване на Google Maps Спирките (Route Coordinates)
**Цел:** Само платени организации да могат да задават точни Lat/Lng координати.
* **Бекенд (`RouteService.cs`):**
  * При създаване или редактиране на маршрут, ако потребителят е на `Free` план, задавай `Latitude = null` и `Longitude = null`, дори и да са пратени от формата.
* **Фронтенд (`Route/Create.cshtml` и `Route/Edit.cshtml`):**
  * Сложи проверка около секцията с картата:
    ```html
    @if (Model.IsPremiumUser) {
        <div id="map"></div>
    } else {
        <div class="alert alert-warning">
            Интеграцията с Google Maps е достъпна само за Pro и Enterprise планове.
            <a asp-action="Upgrade">Вижте плановете</a>
        </div>
    }
    ```

## 4. Визуализация в Dashboard
**Цел:** Подсещане на потребителя.
* В `Organization/Details`, до премиум функциите сложи малки иконки на катинар 🔒 или текст "Premium", ако потребителят е на Free план.