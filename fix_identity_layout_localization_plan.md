# План за Интеграция и Локализация на Identity Area

## 1. Унифициране на Layout-а (Дизайна)
**Цел:** Страниците за Login и Register трябва да използват главния `_Layout.cshtml` на приложението, за да имат същата навигация и фуутър.
* **Файл:** Намери `Areas/Identity/Pages/_ViewStart.cshtml`.
* **Действие:** Промени пътя към Layout-а да сочи към глобалния:
  ```csharp
  @{
      Layout = "/Views/Shared/_Layout.cshtml";
  }