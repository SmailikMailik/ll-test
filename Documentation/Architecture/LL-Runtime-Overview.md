# Обзор LL.Runtime

`LL.Runtime` - основная runtime-сборка проекта. Этот документ кратко описывает её области, границы времени жизни и
главные потоки данных. Нормативные правила размещения кода и зависимостей находятся в
[`ARCHITECTURE.md`](../../ARCHITECTURE.md).

## Области

| Область | Ответственность |
| --- | --- |
| `Bootstrap` | Запуск приложения, отображение прогресса и переход к основной сцене |
| `Composition` | Lifetime scopes, DI-регистрации и выбор конкретных реализаций |
| `Game` | Игровые правила, определения, каталоги и сервисы |
| `Infrastructure` | Загрузка, сериализация, хранение и технические адаптеры |
| `Presentation` | Локализация, форматирование, иконки и представление игровых значений |
| `UI` | Компоненты, окна, навигация, визуальные состояния и UI-потоки |
| `User` | Начальные данные, сохранение, снимки и изменяемое состояние пользователя |
| `Validation` | Общие правила, результаты и отчёты валидации |

Все области находятся в одной физической сборке. Допустимые направления зависимостей контролирует
`tools/Validate-Architecture.ps1`.

## Границы времени жизни

| Scope | Время жизни | Основные регистрации |
| --- | --- | --- |
| `ProjectLifetimeScope` | Весь процесс | Каталоги окон, локализация, игровые данные, пользовательское состояние и игровые сервисы |
| `BootstrapLifetimeScope` | Сцена `Bootstrap` | Прогресс запуска, `BootstrapFlow` и три `IBootstrapOperation` |
| `MainLifetimeScope` | Сцена `Main` | `WindowController`, модальные адаптеры, `RankUpFlow` и `UpgradeFlow` |

`ProjectLifetimeScope` создаётся один раз через `VContainerSettings`. Сценовые scopes наследуют его регистрации и
добавляют только объекты своего времени жизни.

Scopes оставляют небольшую локальную композицию на виду, крупные подсистемы подключают через `IInstaller`, а
factories создают конкретные политики хранения и загрузки без DI-регистрации.

`ProjectLifetimeScope` выбирает конкретные политики через `GameDataLoaderFactory` и `UserSaveRepositoryFactory`, после
чего передаёт installers только `IDataLoader<GameDataSnapshot>` и `IUserSaveRepository`. Для ручной подмены источника
достаточно изменить соответствующий factory-вызов в project scope; installers и потребители от технологии не зависят.

## Основные потоки

### Запуск приложения

```text
Bootstrap scene
    -> BootstrapFlow
    -> LocalizationBootstrapOperation
       + MinDisplayBootstrapOperation
       + SceneLoadingBootstrapOperation
    -> Main scene
```

`BootstrapFlow` параллельно координирует операции, объединяет их прогресс и активирует сцену `Main` после завершения
обязательной работы.

### Игровые данные

```text
GameDataManifestConfig
    -> ScriptableObjectGameDataSource
    -> GameDataDeclaration
    -> GameDataCompiler
    -> GameDataSnapshot
    -> неизменяемые каталоги
```

Источник нормализует Unity-конфигурации. Компилятор проверяет идентификаторы и ссылки, затем создаёт согласованный
снимок. Альтернативный сериализованный источник заканчивается тем же `GameDataDeclaration`, поэтому правила
компиляции не зависят от формата хранения.

### Пользовательские данные

```text
UserDefaultsConfig -----\
                         -> UserSessionLoader -> UserSnapshot -> UserState
IUserSaveRepository ----/

UserState.Changed -> UserSaveCoordinator -> IUserSaveRepository
```

`UserSessionLoader` загружает сохранённый снимок или создаёт начальный. `UserState` остаётся единственным изменяемым
агрегатом сессии, а `UserSaveCoordinator` сохраняет новый снимок после успешных изменений.

Подробное описание представлений и преобразований находится в
[`Game-And-User-Data.md`](Game-And-User-Data.md).

### UI-навигация

```text
WindowCatalogConfig
    -> WindowCatalog
    -> WindowProvider
    -> WindowNavigator
    -> WindowController
    -> Window<TParameters>
```

Игровые сервисы выполняют правила, UI flows координируют сценарии, navigator управляет историей переходов, а окна
отвечают за визуальное поведение.
