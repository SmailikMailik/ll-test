# Обзор LL.Runtime

`LL.Runtime` - основная runtime-сборка проекта. Этот документ кратко описывает её области, границы времени жизни и
главные потоки данных. Нормативные правила размещения кода и зависимостей находятся в
[`Architecture.md`](../Standards/Architecture.md).

## Области

| Область | Ответственность |
| --- | --- |
| `Bootstrap` | Запуск приложения, отображение прогресса и переход к основной сцене |
| `Composition` | Lifetime scopes, DI-регистрации и выбор конкретных реализаций |
| `Game` | Игровые правила, определения, каталоги и сервисы |
| `Infrastructure` | Снимки коллекций, загрузка, сериализация, хранение и технические адаптеры |
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

`ProjectLifetimeScope` выбирает конкретные политики через `GameDataLoaderFactory`, `UserDefaultsSourceFactory` и
`UserSaveRepositoryFactory`, напрямую создаёт небольшие `ScriptableObjectItemIconCatalogLoader` и
`ScriptableObjectWindowCatalogLoader`, после чего передаёт installers только абстракции загрузки и хранения. Для
ручной подмены источника достаточно изменить composition в project scope; installers и потребители от технологии не
зависят.

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
    -> ScriptableObjectGameDataSource : IDataSource<GameDataDeclaration>
    -> GameDataDeclaration
    -> CompiledDataLoader + GameDataCompiler
    -> GameDataSnapshot
    -> неизменяемые каталоги
```

Источник нормализует Unity-конфигурации. Компилятор проверяет идентификаторы и ссылки, затем создаёт согласованный
снимок. Альтернативный сериализованный источник заканчивается тем же `GameDataDeclaration`, поэтому правила
компиляции не зависят от формата хранения.

### Пользовательские данные

```text
UserDefaultsConfig
    -> IDataSource<UserDefaultsDeclaration>
    -> UserDefaultsDeclaration
    -> UserDefaultsCompiler
    -> UserDefaultsSnapshot -----\
                                  -> UserSessionLoader
IUserSaveRepository -------------/          |
                                            v
                                  UserSnapshotReconciler
                                            |
                                            v
                                  UserSnapshot -> UserState

UserState.Changed -> UserSaveCoordinator -> IUserSaveRepository
```

Источник отделяет Unity-конфигурацию от runtime-представления. Компилятор проверяет начальный ранг, опыт и наличие
всех предметов, на которые ссылаются встроенные правила, карты, оплаты и награды, после чего создаёт неизменяемый
снимок начальных значений. `UserSessionLoader` загружает сохранённый снимок или создаёт новый пользовательский снимок
из начального.
`UserSnapshotReconciler` возвращает явный результат согласования старого сохранения с актуальными героями, рангами,
обязательными предметами и rank-up заданиями. Время истечения заданий поступает через зарегистрированный `TimeProvider`.
`UserState` объединяет изменения составной синхронной операции в одно уведомление, а `UserSaveCoordinator` применяет
короткий debounce, последовательно сохраняет последний снимок, повторяет временно неудачную запись и выполняет
финальный flush при завершении.

UI получает `UserHeroSelectionSnapshot` и наблюдает `IUserItems`, `IUserHeroProgress` и `IUserRankUpAttempts`, но не
получает mutating API. Игровые сервисы изменяют те же singleton-состояния через отдельные `IUserItemsCommands`,
`IUserHeroProgressCommands` и `IUserRankUpAttemptsCommands`.

Подробное описание представлений и преобразований находится в
[`Game-And-User-Data.md`](Game-And-User-Data.md). Модель отдельного прогресса каждого героя и произвольного набора
вариантов повышения определена в [`Ranks-And-Rank-Up.md`](Ranks-And-Rank-Up.md).

### UI-навигация

```text
WindowCatalogConfig
    -> ScriptableObjectWindowCatalogLoader
    -> WindowCatalog
    -> WindowProvider
    -> WindowNavigator
    -> WindowController
    -> Window<TParameters>
```

Игровые сервисы выполняют правила, UI flows координируют сценарии, navigator управляет историей переходов, а окна
отвечают за визуальное поведение.

Каталог иконок строится через такую же границу:

```text
ItemIconCatalogConfig
    -> ScriptableObjectItemIconCatalogLoader
    -> SpriteCatalog<ItemId>

FlagCatalogConfig
    -> ScriptableObjectFlagCatalogLoader
    -> SpriteCatalog<FlagId>
```
