# План за Имплементация: Автоматични Имейл Нотификации

## 1. Конфигурация на Услугата (Preparation & Docker Readiness)
* **Цел:** Уверяваме се, че `IEmailSender` е правилно регистриран и взима ключовете си от конфигурацията (`IConfiguration`), а не от хардкоднат стринг.
* **Файл:** `Program.cs` (или `ServiceCollectionExtension`)
  * Провери дали `IEmailSender` (напр. `SendGridEmailSender`) е регистриран като Transient/Scoped сървис.
  * Увери се, че конструкторът му чете ключа чрез `builder.Configuration["SendGrid:ApiKey"]`.

## 2. Създаване на HTML Темплейти (Утилити клас)
* **Цел:** Имейлите трябва да изглеждат професионално, а не като обикновен текст.
* **Нов Файл:** Създай клас `EmailTemplates.cs` в папката `Common` (или `Services.Messaging`).
* **Методи:** Добави статичен метод `GetTicketPurchaseEmail(string passengerName, string routeName, string date, string ticketId)`. Този метод трябва да връща красив HTML стринг (с малко inline CSS, защото email клиентите не четат външни CSS файлове) със съобщение за успешно купен билет.

## 3. Интеграция в TicketService (Бизнес Логика)
* **Файл:** `Services/GoceTransportApp.Services.Data/Tickets/TicketService.cs` (или съответния контролер/сървис, където се създава билетът).
* **Инжектиране:** Добави `IEmailSender emailSender` в конструктора на `TicketService`.
* **Логика на изпращане:** * Намери метода, който финализира покупката/резервацията на билета (`CreateTicketAsync` или подобен).
  * ВЕДНАГА след успешното записване на билета в базата данни (`await dbContext.SaveChangesAsync()`), вземи имейла на потребителя (през навигационното свойство на User-а или UserManager).
  * Генерирай HTML съдържанието чрез `EmailTemplates.GetTicketPurchaseEmail(...)`.
  * Извикай: `await this.emailSender.SendEmailAsync("your-sender@domain.com", "GoceTransportApp", userEmail, "Успешна резервация на билет", htmlContent);`. *(Забележка: Замени sender email-а с този, който е верифициран в твоя SendGrid/SMTP акаунт).*

## 4. Имейл при Отказан Билет (Крайни случаи)
* **Файл:** `TicketService.cs` (или там, където е методът за отказ на билет).
* **Логика:** При успешно анулиране на билет от страна на пътника, изпрати втори вид имейл: "Вашият билет беше успешно анулиран".