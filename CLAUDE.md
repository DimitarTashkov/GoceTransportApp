# GoceTransportApp - Системни Правила и Контекст

## 1. Технологичен Стек
* **Backend:** ASP.NET Core MVC (.NET 8/6)
* **Database:** Entity Framework Core (SQL Server)
* **Frontend:** Строго Bootstrap 5. Забранено е писането на custom CSS файлове. Разчитаме на Bootstrap utility класове.
* **Архитектура:** N-Tier (Web, Services.Data, Services.Mapping, Data.Models).

## 2. Правила за работа (Cost & Token Optimization)
* **Не сканирай целия проект:** Когато получаваш задача, търси и отваряй САМО файловете, свързани със задачата (Micro-targeting).
* **Стил на писане:** Пиши чист код. Използвай съществуващия AutoMapper (`IHaveCustomMappings`, `IMapFrom`, `IMapTo`) за DTO-тата.
* **Команди:** Използвай `dotnet build` след сериозни промени, за да си сигурен, че кодът се компилира.

## 3. Работен процес (Handoff)
* В края на всяка задача или сесия, Агентът задължително трябва да обнови файла `PROGRESS.md`, за да запази контекста за следващото стартиране.