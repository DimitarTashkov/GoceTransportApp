# План за Завършване на Persistent Notifications (Inbox & Камбанка)

## 1. Създаване на MessageService (Бизнес логика)
**Цел:** Услуга, която записва известието в базата и веднага го "излъчва" през SignalR.
* Създай `IMessageService` и `MessageService` в проекта `GoceTransportApp.Services.Data`.
* Инжектирай в него `IRepository<Message>` и `IHubContext<NotificationHub>`.
* **Методи:**
  1. `Task CreateNotificationAsync(string receiverId, string content, string? link = null)`:
     * Създава обект `Message` (ReceiverId = receiverId, Content = content, IsRead = false, CreatedOn = DateTime.UtcNow).
     * Записва го в базата.
     * Веднага след това извиква SignalR: `await hubContext.Clients.User(receiverId).SendAsync("ReceiveNotification", content);`
  2. `Task<IEnumerable<MessageViewModel>> GetUnreadMessagesAsync(string userId)`:
     * Връща списък с последните непрочетени съобщения за даден потребител (сортирани по дата низходящо).
  3. `Task MarkAsReadAsync(string messageId)` (или `int messageId` според типа на ключа).

## 2. API Контролер за Фронтенда (NotificationController)
**Цел:** Ендпойнти (Endpoints), от които камбанката да си дърпа съобщенията чрез AJAX.
* В `GoceTransportApp.Web/Controllers` създай `NotificationController`.
* Добави `[Authorize]` атрибут.
* **Екшъни (връщат JSON):**
  1. `[HttpGet] GetMyNotifications()`: Извиква сървиса и връща непрочетените съобщения на логнатия потребител като JSON.
  2. `[HttpPost] MarkAsRead(string id)`: Маркира дадено съобщение като прочетено.

## 3. Обновяване на Frontend-а (`notifications.js` и UI)
**Цел:** Камбанката да работи с реалните данни от базата.
* Отвори `wwwroot/js/notifications.js`:
  * Напиши функция `loadNotifications()`, която прави AJAX `GET` заявка до `/Notification/GetMyNotifications`.
  * Тази функция трябва да актуализира бройката в червения бадж на камбанката и да напълни Dropdown менюто в `_NotificationsPartial.cshtml` с реалните съобщения.
  * Добави събитие (event listener): когато потребителят кликне върху съобщение от падащото меню, направи AJAX `POST` към `/Notification/MarkAsRead`, намали бройката в баджа и скрий съобщението.
  * Извикай `loadNotifications()` при първоначално зареждане на страницата (`$(document).ready(...)`).
  * При получаване на `ReceiveNotification` (от SignalR), просто извикай отново `loadNotifications()` и покажи Toast.

## 4. Свързване с Бизнес логиката (Triggering)
**Цел:** Да генерираме известия при реални събития в платформата.
* Отвори `ReviewService.cs` (Метода за добавяне на ревю).
* Инжектирай `IMessageService`. 
* След като ревюто се запише, намери собственика на фирмата (`FounderId` на `Organization`) и извикай: 
  `await messageService.CreateNotificationAsync(founderId, $"Имате нов отзив за вашия профил!");`