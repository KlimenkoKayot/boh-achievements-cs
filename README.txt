# Book of Hours Achievements Viewer

Консольное приложение на языке **C#** для просмотра достижений и групп достижений из игры **Book of Hours**. Данные о достижениях представлены в формате JSON и хранятся в директориях `./BoH/<locale>/achievements/`. Приложение предоставляет текстовый пользовательский интерфейс (TUI) для удобного просмотра информации.

## Основные функции

- **Просмотр категорий достижений**: Пользователь видит список всех категорий достижений.
- **Навигация**: Перемещение между карточками категорий осуществляется с помощью стрелок вверх и вниз.
- **Разворачивание/свертывание карточек**: По нажатию клавиши **Enter** карточка категории разворачивается, показывая список достижений внутри нее. Повторное нажатие сворачивает карточку.
- **Выход**: Для выхода из режима просмотра используется клавиша **Escape**.

## Установка

1. Убедитесь, что у вас установлена [.NET SDK](https://dotnet.microsoft.com/download) (версия 6.0 или выше).
2. Клонируйте репозиторий:
   ```bash
   git clone https://github.com/klimenkokayot/boh-achievements-cs.git
   ```
3. Перейдите в директорию проекта:
   ```bash
   cd boh-achievements-cs
   ```
4. Соберите проект:
   ```bash
   dotnet build
   ```

## Использование

1. Запустите приложение:
   ```bash
   dotnet run
   ```
2. Используйте следующие клавиши для управления:
   - **Стрелки вверх/вниз**: Перемещение между категориями.
   - **Enter**: Развернуть/свернуть карточку категории.
   - **Escape**: Выйти из приложения.

## Структура данных

Данные о достижениях хранятся в формате JSON в директориях `./BoH/<locale>/achievements/`. Каждый JSON-файл содержит массив достижений, где каждое достижение описывается следующими полями:

- **id**: Уникальный идентификатор достижения.
- **category**: Категория, к которой относится достижение.
- **iconUnlocked**: Иконка, отображаемая для разблокированного достижения.
- **singleDescription**: Указывает, используется ли одно описание для достижения.
- **validateOnStorefront**: Указывает, проверяется ли достижение на стороне клиента.
- **label**: Название достижения.
- **descriptionunlocked**: Описание достижения после его разблокировки.

Пример структуры JSON:
```json
{
  "achievements": [
    {
      "id": "A_AFFAIR_CASKET",
      "category": "A_CATEGORY_AFFAIR",
      "iconUnlocked": "trophy",
      "singleDescription": true,
      "validateOnStorefront": true,
      "label": "The Affair of the Messenger's Casket",
      "descriptionunlocked": "In a hidden fold of the mountains above the city of Cluj lies the enigmatic relic they call Saint Mihail's Coffin..."
    },
    {
      "id": "A_AFFAIR_GRUNEWALD",
      "category": "A_CATEGORY_AFFAIR",
      "iconUnlocked": "trophy",
      "singleDescription": true,
      "validateOnStorefront": true,
      "label": "The Affair of the Grunewald Provision",
      "descriptionunlocked": "The assets of Grunewald's Permanent Circus were disposed of by the Bureau. In hindsight, this was a mistake..."
    }
  ]
}
```

## Зависимости

- [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0) или выше.
- [Newtonsoft.Json](https://www.newtonsoft.com/json) для работы с JSON-файлами (устанавливается через NuGet).

## Как добавить зависимости

Если вы хотите добавить зависимости в проект, используйте команду:

```bash
dotnet add package <название пакета>
```

Например, для добавления Newtonsoft.Json:

```bash
dotnet add package Newtonsoft.Json
```

## Лицензия

Этот проект распространяется под лицензией [MIT](LICENSE).

---

### Как внести вклад

1. Сделайте форк репозитория.
2. Создайте новую ветку для ваших изменений:
   ```bash
   git checkout -b feature/your-feature-name
   ```
3. Зафиксируйте изменения:
   ```bash
   git commit -m "Добавлен новый функционал"
   ```
4. Отправьте изменения в ваш форк:
   ```bash
   git push origin feature/your-feature-name
   ```
5. Создайте Pull Request в основном репозитории.
