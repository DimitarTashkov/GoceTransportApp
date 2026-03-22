# Цялостен План за UI/UX Модернизация (GoceTransportApp - Bootstrap 5)

## 1. Основна Директива (System Rules)
* **Строго правило:** Забранено е писането на Custom CSS. Дизайнът трябва да се изгради изцяло чрез Utility класовете на Bootstrap 5 (Spacing, Borders, Shadows, Backgrounds, Colors).
* **Естетика:** Търсим модерен, ефирен SaaS дизайн. Повече празно пространство (whitespace), заоблени ъгли (`rounded-4`), меки сенки (`shadow-sm`) и премахване на всички груби, черни рамки.
* **Абсолютна Консистентност:** Всички преизползваеми елементи (Header, Footer, Cards, Forms) трябва да изглеждат **напълно еднакво** на всяка една страница. Не допускай разминаване в padding, margin или стиловете на бутоните между различните изгледи.

## 2. Глобален Layout и Навигация (`_Layout.cshtml`)
* **Background:** Задай на `<body>` клас `bg-body-tertiary` (или `bg-light`), за да изпъкват белите елементи (Cards).
* **Navbar (Header):** Направи навигацията бяла и залепена горе: `navbar navbar-expand-lg navbar-light bg-white sticky-top shadow-sm`.
* **Footer:** Направи го минималистичен: `bg-white text-muted py-4 border-top mt-auto`.
* **⚠️ КРИТИЧНО ЗА ЛИНКОВЕТЕ:** Преди да запазиш `_Layout.cshtml`, задължително провери всички линкове (в Header и Footer). Увери се, че атрибутите `asp-area`, `asp-controller` и `asp-action` са запазени правилно и водят към съществуващи страници в приложението. Нито един линк не трябва да бъде счупен!

## 3. Модернизация на Partial Views (Карти)
**Файлове за промяна:** Всички `_*Card*.cshtml` и `_*Details*.cshtml` файлове (напр. `_DriverCardPartial.cshtml`, `_RouteCardPartial.cshtml`).
* **Контейнер на картата:** Основният `div` трябва да бъде: `card border-0 shadow-sm rounded-4 h-100`.
* **Снимки (ако има):** Изображенията вътре в картите да имат клас `rounded-top-4` или да са оформени като аватар с `rounded-circle`.
* **Card Body:** `card-body p-4`.
* **Бутони вътре:** `btn btn-outline-primary rounded-pill w-100 mt-3`.

## 4. Модернизация на Формите (Create & Edit & Identity)
**Файлове за промяна:** Всички `Create.cshtml`, `Edit.cshtml`, `Login.cshtml` и `Register.cshtml`.
* **Оформление:** Формите не трябва да са на цял екран. Центрирай ги в колона: `<div class="col-md-8 col-lg-6 mx-auto">`.
* **Card Wrapper:** Обвий самата `<form>` в карта: `<div class="card border-0 shadow-sm rounded-4"><div class="card-body p-5">`.
* **Floating Labels:** **Задължително** конвертирай всички полета към Floating Labels (`form-floating mb-4` с инпути `form-control form-control-lg rounded-3`).
* **Submit Бутони:** Главните бутони за запазване да са: `btn btn-primary btn-lg w-100 rounded-pill shadow-sm`. 

## 5. Таблици и Списъци (Index & Administration)
* Ако списъкът е Grid (мрежа от карти), използвай `row row-cols-1 row-cols-md-3 g-4`.
* Ако списъкът е таблица:
  * Обвий таблицата в `<div class="table-responsive bg-white rounded-4 shadow-sm p-3">`.
  * Самата таблица да има класове: `table table-borderless table-hover align-middle mb-0`.

## 6. Начална страница (Home/Index)
**Файл за промяна:** `Views/Home/Index.cshtml`
* Създай впечатляваща Hero секция най-горе: `<div class="py-5 text-center">`
* Главно заглавие (H1): `display-4 fw-bold text-dark mb-3`.
* Подзаглавие (Lead): `lead text-muted mb-5`.