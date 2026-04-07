# План за Интеграция на Google и Facebook Login

## 1. Инсталиране на NuGet Пакети
**Цел:** Добавяне на официалните библиотеки за автентикация в `GoceTransportApp.Web`.
* Инсталирай `Microsoft.AspNetCore.Authentication.Google`.
* Инсталирай `Microsoft.AspNetCore.Authentication.Facebook`.

## 2. Конфигурация в Program.cs
**Цел:** Регистриране на провайдърите в DI контейнера.
* Намери секцията `builder.Services.AddAuthentication()`. (Ако липсва, добави я след конфигурацията на Identity).
* Добави следния код:
  ```csharp
  builder.Services.AddAuthentication()
      .AddGoogle(options =>
      {
          options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "dummy-client-id";
          options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "dummy-client-secret";
      })
      .AddFacebook(options =>
      {
          options.AppId = builder.Configuration["Authentication:Facebook:AppId"] ?? "dummy-app-id";
          options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"] ?? "dummy-app-secret";
      });

      3. Обновяване на UI (Изгледи за Вход и Регистрация)
Цел: Показване на красиви бутони за външен логин.

Файлове за редакция: Areas/Identity/Pages/Account/Login.cshtml и Register.cshtml.

Намери секцията Use another service to log in (ако е генерирана по подразбиране).

Увери се, че формата итерира през Model.ExternalLogins и генерира бутони.

Стилизиране: Приложи Bootstrap класове, за да изглеждат добре (напр. използвай иконки от Bootstrap Icons bi bi-google и bi bi-facebook).

Пример за дизайн на бутон:
<button type="submit" class="btn btn-outline-dark w-100 mb-2" name="provider" value="@provider.Name" title="Log in using your @provider.DisplayName account"><i class="bi bi-@provider.Name.ToLower()"></i> Вход с @provider.DisplayName</button>

4. Конфигурационни файлове
В appsettings.Development.json добави структурата:

JSON
"Authentication": {
  "Google": {
    "ClientId": "",
    "ClientSecret": ""
  },
  "Facebook": {
    "AppId": "",
    "AppSecret": ""
  }
}
(Стойностите да останат празни, потребителят ще използва Secret Manager за реалните ключове).