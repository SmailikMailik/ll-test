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
| `Presentation` | Локализация, форматирование, иконки, флаги, портреты и представление игровых значений |
| `UI` | Компоненты, окна, навигация, визуальные состояния и UI-потоки |
| `User` | Начальные данные, сохранение, снимки и изменяемое состояние пользователя |
| `Validation` | Общие правила, результаты и отчёты валидации |

Все области находятся в одной физической сборке. Допустимые направления зависимостей контролирует
`tools/Validate-Architecture.ps1`.

## Границы времени жизни

| Scope | Время жизни | Основные регистрации |
| --- | --- | --- |
| `ProjectLifetimeScope` | Весь процесс | Каталог окон, presentation-каталоги, локализация, отчётность валидации, игровые данные, пользовательский жизненный цикл и игровые сервисы |
| `BootstrapLifetimeScope` | Сцена `Bootstrap` | Прогресс запуска, `BootstrapFlow` и три `IBootstrapOperation` |
| `MainLifetimeScope` | Сцена `Main` | `WindowController`, `HeroPortraitView`, модальные подтверждения, `RankUpService`, `RankUpFlow`, `UpgradeFlow` и его стартовая точка |

`ProjectLifetimeScope` создаётся один раз через `VContainerSettings`. Сценовые scopes наследуют его регистрации и
добавляют только объекты своего времени жизни.

Scopes оставляют небольшую локальную композицию на виду, крупные подсистемы подключают через `IInstaller`, а
factories создают конкретные политики хранения и загрузки без DI-регистрации.

`ProjectLifetimeScope` выбирает конкретные политики через `GameDataLoaderFactory`, `UserDefaultsSourceFactory` и
`UserSaveRepositoryFactory`. Он напрямую создаёт небольшие loaders для каталога окон, иконок предметов, флагов и
вариантов портретов героев, после чего передаёт installer-ам только абстракции загрузки и хранения. Для ручной подмены
источника достаточно изменить composition в project scope; installer-ы и потребители от технологии не зависят.

## Основные потоки

Схемы в этом обзоре показывают только устойчивый путь от источника или инициатора через координирующую границу к
результату. Внутренние ветвления и частные случаи остаются в поясняющем тексте и специализированных документах, чтобы
обзорные схемы отражали реальную архитектуру, но оставались компактными и не требовали горизонтальной прокрутки.

### Запуск приложения

```text
Bootstrap scene
    -> BootstrapFlow
    -> три IBootstrapOperation параллельно
    -> Main scene activation
```

`BootstrapFlow` параллельно координирует `LocalizationBootstrapOperation`, `MinDisplayBootstrapOperation` и
`SceneLoadingBootstrapOperation`, объединяет их прогресс и активирует сцену `Main`, когда готовы все три операции.

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
снимок с каталогами рангов, карт, героев, квестов, вариантов повышения ранга и наград. `GameDataInstaller` регистрирует
как весь `GameDataSnapshot`, так и каждый каталог отдельно. Альтернативный сериализованный источник заканчивается тем
же `GameDataDeclaration`, поэтому правила компиляции не зависят от формата хранения.

### Пользовательские данные

```text
UserDefaultsConfig
    -> IDataSource<UserDefaultsDeclaration>
    -> CompiledDataLoader + UserDefaultsCompiler
    -> UserDefaultsSnapshot -----\
                                  -> UserSessionLoader -> UserSnapshot -> UserState
IUserSaveRepository -------------/

UserState.Changed -> UserSaveCoordinator -> IUserSaveRepository.Save()
```

Источник отделяет Unity-конфигурацию от runtime-представления. Компилятор проверяет начальный ранг, опыт и наличие
всех предметов, на которые ссылаются встроенные правила, карты, оплаты и награды, после чего создаёт неизменяемый
снимок начальных значений. `UserSessionLoader` координирует загрузку: передаёт успешно загруженный snapshot в
`UserSnapshotReconciler`, а при отсутствии, повреждении, неподдерживаемой версии или несовместимости создаёт новый
пользовательский snapshot из defaults. `UserSnapshotReconciler` возвращает явный результат согласования сохранения
с актуальными героями, рангами, обязательными предметами и rank-up заданиями. Время истечения заданий поступает через
зарегистрированный `TimeProvider`. `UserInstaller` регистрирует части загруженного snapshot-а отдельно, создаёт
`UserItems` и `UserHeroes` как singleton-объекты для read-only и command-интерфейсов и объединяет их уведомления в
`UserState`.
`UserState` сводит изменения составной синхронной операции в одно уведомление, а `UserSaveCoordinator` применяет
короткий debounce, сохраняет последний снимок, повторяет неудачную запись и выполняет финальный flush при завершении.

UI получает `UserHeroSelectionSnapshot` и наблюдает `IUserItems`, `IUserHeroProgress` и `IUserRankUpAttempts`, но не
получает mutating API. Игровые сервисы изменяют те же singleton-состояния через отдельные `IUserItemsCommands`,
`IUserHeroProgressCommands` и `IUserRankUpAttemptsCommands`.

Подробные описания находятся в [`Game-Data.md`](Game-Data.md) и [`User-Data.md`](User-Data.md). Модель отдельного
прогресса каждого героя и произвольного набора вариантов повышения определена в
[`Ranks-And-Rank-Up.md`](Ranks-And-Rank-Up.md).

### UI-навигация

```text
WindowCatalogConfig
    -> ScriptableObjectWindowCatalogLoader
    -> WindowCatalog
    -> WindowProvider

UI flows
    -> WindowController
       |-> WindowProvider -> Window<TParameters>
       \-> WindowNavigator -> история и видимость окон
```

Игровые сервисы выполняют правила, UI flows координируют сценарии, `WindowController` получает или создаёт окно через
`WindowProvider` и передаёт его в `WindowNavigator`, navigator управляет историей и видимостью, а окна отвечают за
визуальное поведение. `UpgradeFlowStartup` открывает первое окно сцены `Main`.

Независимые presentation-ресурсы строятся через такую же границу загрузки и регистрируются как singleton-каталоги:

```text
ItemIconCatalogConfig
    -> ScriptableObjectItemIconCatalogLoader
    -> SpriteCatalog<ItemId>

FlagCatalogConfig
    -> ScriptableObjectFlagCatalogLoader
    -> SpriteCatalog<FlagId>

HeroPortraitCatalogConfig
    -> ScriptableObjectHeroPortraitCatalogLoader
    -> SpriteVariantCatalog<HeroId, HeroPortraitSize>
```
