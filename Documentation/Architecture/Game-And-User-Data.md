# Игровые и пользовательские данные

Этот документ объясняет, какие представления данных существуют в проекте, как они преобразуются друг в друга и
где искать код каждого этапа.

## Главное различие

Игровые и пользовательские данные решают разные задачи:

- игровые данные описывают общие правила и справочники: ранги, карты, задания, повышения и награды;
- пользовательские данные описывают состояние конкретного игрока: идентификатор, инвентарь, прогресс и активное
  задание повышения ранга.

Обе системы загружаются одним согласованным агрегатом, но жизненный цикл агрегатов различается:

- `GameDataSnapshot` создаётся при запуске и затем только читается;
- `UserSnapshot` создаётся при загрузке сессии, из него строится изменяемый `UserState`, а изменения состояния снова
  сохраняются через `UserSaveDocument`.

## Слои представления данных

| Представление | Для чего предназначено | Может содержать непроверенные данные |
| --- | --- | --- |
| `Config` и `Entry` | Редактирование данных в Unity Inspector | Да |
| `Document` и `DocumentEntry` | Стабильный версионируемый контракт JSON или другого внешнего формата | Да |
| `Declaration` | Единая форма игровых данных, не зависящая от источника | Да |
| `Compiler` | Общая проверка ID и связей, создание доменных объектов | Получает непроверенные данные |
| `Snapshot`, `Catalog`, `Definition` | Внутренние неизменяемые и согласованные данные приложения | Нет |
| `State` | Изменяемое состояние текущей пользовательской сессии | Нет |

`Config`, `Document` и `Declaration` не следует передавать игровым сервисам или UI. Потребители получают только
готовые каталоги, определения, снимки или интерфейсы состояния.

Компиляторы реализуют `IDataCompiler<TDeclaration, TSnapshot>` и принимают в `Compile` только исходную декларацию.
Стабильные каталоги, необходимые для разрешения ссылок, передаются компилятору через конструктор. Validation pipeline
собирает ошибки недоверенных представлений, а guard clauses runtime-объектов отдельно защищают программные контракты.

## Загрузка игровых данных

```mermaid
flowchart LR
    Manifest["GameDataManifestConfig<br/>ссылки на Unity-конфиги"]
    Json["JSON / PlayerPrefs"]
    SO_Source["ScriptableObjectGameDataSource"]
    Serialized_Source["SerializedGameDataSource"]
    Document["GameDataDocument<br/>Version + DocumentEntry[]"]
    Mapper["GameDataDocumentMapper"]
    Declaration["GameDataDeclaration<br/>единая нейтральная форма"]
    Compiler["GameDataCompiler<br/>ID, дубликаты и ссылки"]
    Snapshot["GameDataSnapshot"]
    Catalogs["RankCatalog · CardCatalog<br/>QuestCatalog · RankUpCatalog · RewardCatalog"]

    Manifest --> SO_Source
    SO_Source --> Declaration
    Json --> Serialized_Source
    Serialized_Source --> Document
    Document --> Mapper
    Mapper --> Declaration
    Declaration --> Compiler
    Compiler --> Snapshot
    Snapshot --> Catalogs
```

В текущей композиции `ProjectLifetimeScope` выбирает путь через `ScriptableObjectGameDataSource` и передаёт готовый
`IDataLoader<GameDataSnapshot>` в `GameDataInstaller`. Альтернативные пути через JSON-файл и PlayerPrefs уже
предоставляет `GameDataLoaderFactory`, поэтому переключение не меняет installer или потребителей.

### Физическая раскладка игровых данных

```text
Game/Data/
├── Configuration/
│   └── GameDataManifestConfig.cs
├── Declarations/
│   ├── GameDataDeclaration.cs
│   ├── RankDeclaration.cs
│   ├── CardDeclaration.cs
│   ├── QuestDeclaration.cs
│   ├── RankUpDeclaration.cs
│   ├── PaymentDeclaration.cs
│   ├── RewardDeclaration.cs
│   └── RewardItemDeclaration.cs
├── Persistence/
│   ├── Documents/
│   │   ├── GameDataDocument.cs
│   │   └── *DocumentEntry.cs
│   └── GameDataDocumentMapper.cs
├── Sources/
│   ├── ScriptableObjectGameDataSource.cs
│   └── SerializedGameDataSource.cs
├── GameDataCompiler.cs
├── GameDataLoader.cs
├── GameDataSnapshot.cs
└── IGameDataSource.cs
```

Корень `Game/Data` координирует загрузку агрегата. `Declarations` содержит только единое промежуточное представление,
которое создают все источники и принимает компилятор.

### Раскладка Unity-ассетов

Классы конфигураций остаются рядом со своими фичами в коде, а созданные через Unity конфигурационные ассеты
группируются по владельцу:

```text
Assets/_Project/Configuration/
├── Game/
│   ├── CardCatalogConfig.asset
│   ├── GameDataManifestConfig.asset
│   ├── QuestCatalogConfig.asset
│   ├── RankCatalogConfig.asset
│   ├── RankUpCatalogConfig.asset
│   └── RewardCatalogConfig.asset
├── Presentation/
│   ├── FlagCatalogConfig.asset
│   └── ItemIconCatalogConfig.asset
├── UI/
│   └── WindowCatalogConfig.asset
└── User/
    └── UserDefaultsConfig.asset
```

Папка фичи внутри владельца нужна только для связанной группы из нескольких ассетов или осмысленной вложенной
структуры. Один ассет не получает отдельную папку только ради повторения иерархии C#-кода.

### Файлы игрового потока

| Файл или группа | Ответственность |
| --- | --- |
| `Configuration/GameDataManifestConfig.cs` | Корневой Unity-ассет, который ссылается на конфиги всех игровых каталогов |
| `IGameDataSource.cs` | Контракт источника: вернуть один `GameDataDeclaration` |
| `Sources/ScriptableObjectGameDataSource.cs` | Проверить Unity-конфиги и нормализовать их в `Declaration` |
| `Sources/SerializedGameDataSource.cs` | Загрузить и проверить версию внешнего `GameDataDocument` |
| `Persistence/Documents/GameDataDocument.cs` | Корневой версионируемый внешний контракт игровых данных |
| `Persistence/Documents/*DocumentEntry.cs` | Строки внешнего контракта для рангов, карт, заданий, повышений, оплат и наград |
| `Persistence/GameDataDocumentMapper.cs` | Чистое преобразование `Document` в `Declaration`, без I/O и проверки связей |
| `Declarations/GameDataDeclaration.cs` | Корень source-neutral представления со всеми наборами объявлений |
| `Declarations/*Declaration.cs` | Непроверенные объявления отдельных игровых сущностей |
| `GameDataCompiler.cs` | Найти пустые и повторяющиеся ID, проверить ссылки, создать доменные определения и каталоги |
| `GameDataSnapshot.cs` | Один неизменяемый согласованный результат загрузки |
| `GameDataLoader.cs` | Выполнить цепочку `source.Read()` → `compiler.Compile()` |
| `Composition/Factories/GameDataLoaderFactory.cs` | Собрать загрузчик для выбранной технологии хранения |
| `Composition/Installers/GameDataInstaller.cs` | Загрузить снимок и зарегистрировать его каталоги в DI |

### Состав агрегатов игровых данных

```mermaid
classDiagram
    class GameDataDocument {
        +int Version
        +RankDocumentEntry[] Ranks
        +CardDocumentEntry[] Cards
        +QuestDocumentEntry[] Quests
        +RankUpDocumentEntry[] RankUps
        +RewardDocumentEntry[] Rewards
    }
    class GameDataDeclaration {
        +Ranks
        +Cards
        +Quests
        +RankUps
        +Rewards
    }
    class GameDataSnapshot {
        +RankCatalog Ranks
        +CardCatalog Cards
        +QuestCatalog Quests
        +RankUpCatalog RankUps
        +RewardCatalog Rewards
    }

    GameDataDocument *-- RankDocumentEntry
    GameDataDocument *-- CardDocumentEntry
    GameDataDocument *-- QuestDocumentEntry
    GameDataDocument *-- RankUpDocumentEntry
    GameDataDocument *-- RewardDocumentEntry
    RankUpDocumentEntry *-- PaymentDocumentEntry
    RewardDocumentEntry *-- RewardItemDocumentEntry

    GameDataDeclaration *-- RankDeclaration
    GameDataDeclaration *-- CardDeclaration
    GameDataDeclaration *-- QuestDeclaration
    GameDataDeclaration *-- RankUpDeclaration
    GameDataDeclaration *-- RewardDeclaration
    RankUpDeclaration *-- PaymentDeclaration
    RewardDeclaration *-- RewardItemDeclaration
```

Файлы разделены, но агрегаты не раздроблены: источник всё ещё возвращает один `GameDataDeclaration`, а сериализатор
всё ещё читает один `GameDataDocument`.

## Загрузка и сохранение пользователя

```mermaid
flowchart LR
    Storage["JSON-файл"]
    Repository["SerializedUserSaveRepository"]
    Document["UserSaveDocument<br/>Version + DocumentEntry"]
    Mapper["UserSaveDocumentMapper"]
    Snapshot["UserSnapshot"]
    Loader["UserSessionLoader"]
    Config["UserDefaultsConfig"]
    Source["IUserDefaultsSource"]
    Declaration["UserDefaultsDeclaration"]
    Compiler["UserDefaultsCompiler"]
    Defaults["UserDefaultsSnapshot"]
    Reconciler["UserSnapshotReconciler"]
    State["UserState"]
    Coordinator["UserSaveCoordinator"]

    Storage --> Repository
    Repository --> Document
    Document --> Mapper
    Mapper --> Snapshot
    Repository --> Loader
    Config --> Source
    Source --> Declaration
    Declaration --> Compiler
    Compiler --> Defaults
    Defaults --> Loader
    Loader --> Reconciler
    Snapshot --> Reconciler
    Reconciler --> Snapshot
    Snapshot --> State
    State -- Changed --> Coordinator
    Coordinator --> Repository
    State -- CreateSnapshot --> Snapshot
    Snapshot --> Mapper
    Mapper --> Document
    Document --> Storage
```

`IUserDefaultsSource` преобразует конкретный источник начальных значений в `UserDefaultsDeclaration`.
`UserDefaultsCompiler` проверяет стартовый ранг, допустимый опыт и наличие всех предметов, используемых встроенными
правилами, картами, оплатами и наградами, после чего создаёт `UserDefaultsSnapshot`. `UserSessionLoader` сначала
пытается загрузить сохранение. `UserSnapshotReconciler` проверяет его относительно текущих игровых каталогов,
добавляет отсутствующие обязательные предметы и очищает устаревшее rank-up задание. Несовместимый прогресс приводит
к явному сбросу на defaults. Во время сессии `UserState` объединяет изменяемые части состояния и сворачивает все
изменения одной синхронной игровой операции в одно уведомление. `UserSaveCoordinator` сохраняет итоговый снимок;
ошибка записи не прерывает уже выполненную игровую операцию и повторно проверяется при следующем изменении.

### Файлы пользовательского потока

| Файл или группа | Ответственность |
| --- | --- |
| `Configuration/UserDefaultsConfig.cs` | Unity-authoring начальных значений пользователя |
| `Defaults/Sources/IUserDefaultsSource.cs` | Контракт любого источника начальных значений |
| `Defaults/Sources/ScriptableObjectUserDefaultsSource.cs` | Преобразование Unity-конфига в source-neutral declaration |
| `Defaults/Declarations/*Declaration.cs` | Не проверенное представление начальных значений без зависимости от источника |
| `Defaults/UserDefaultsCompiler.cs` | Runtime-проверка ссылок и построение согласованного снимка начальных значений |
| `Defaults/UserDefaultsSnapshot.cs` | Неизменяемые начальные значения для создания нового пользователя |
| `Persistence/Documents/UserSaveDocument.cs` | Корневой версионируемый контракт сохранения |
| `Persistence/Documents/*DocumentEntry.cs` | Сериализуемые части identity, progress, rank-up quest и inventory |
| `Persistence/UserSaveDocumentMapper.cs` | Двустороннее преобразование `UserSaveDocument` ↔ `UserSnapshot` |
| `Persistence/IUserSaveRepository.cs` | Семантический контракт загрузки и сохранения пользователя |
| `Persistence/SerializedUserSaveRepository.cs` | I/O, проверка версии и классификация ошибок загрузки |
| `Persistence/UserLoadResult.cs` и `UserLoadStatus.cs` | Результат загрузки: loaded, not found, corrupted или unsupported version |
| `Persistence/UserSessionLoader.cs` | Выбрать согласованный сохранённый снимок либо создать значения по умолчанию |
| `Persistence/UserSnapshotReconciler.cs` | Согласовать сохранение с текущими рангами, предметами и rank-up заданием |
| `Snapshots/UserSnapshot.cs` | Корень неизменяемого снимка пользователя |
| `Snapshots/*Snapshot.cs` | Неизменяемые части identity, inventory, progress и rank-up quest |
| `State/UserState.cs` | Объединить части сессии и пакетировать уведомления составных операций |
| `State/Items`, `State/Progress`, `State/RankUp` | Read-интерфейсы, command-интерфейсы и состояние пользовательских возможностей |
| `Persistence/UserSaveCoordinator.cs` | Сохранять новый снимок после изменения состояния |
| `Composition/Factories/UserDefaultsSourceFactory.cs` | Выбрать конкретный источник начальных значений |
| `Composition/Factories/UserSaveRepositoryFactory.cs` | Собрать JSON-сериализацию, файловое хранилище и репозиторий |
| `Composition/Installers/UserInstaller.cs` | Зарегистрировать загрузку снимка, состояние и автосохранение в DI |

## Как проследить данные при отладке

Для игровых данных:

1. Найти исходное значение в capability-конфиге и ссылку на него в `GameDataManifestConfig`.
2. Проверить преобразование в `ScriptableObjectGameDataSource`.
3. Проверить соответствующий `Declarations/*Declaration`.
4. Проверить в `GameDataCompiler` валидацию и создание доменного типа.
5. Найти итоговый `Catalog` в `GameDataSnapshot`.

Для начальных пользовательских данных:

1. Найти значение в `UserDefaultsConfig`.
2. Проверить преобразование в `ScriptableObjectUserDefaultsSource`.
3. Проверить соответствующий `Defaults/Declarations/*Declaration`.
4. Проверить runtime-валидацию в `UserDefaultsCompiler`.
5. Найти итоговое значение в `UserDefaultsSnapshot`.

Для сохранённого состояния пользователя:

1. Проверить JSON и соответствующий `*DocumentEntry`.
2. Проверить преобразование в `UserSaveDocumentMapper`.
3. Найти значение в соответствующем `*Snapshot`.
4. Проверить изменяемую реализацию в `User/State`.
5. При проблеме сохранения пройти `UserState.Changed` → `UserSaveCoordinator` → `SerializedUserSaveRepository`.

UI получает только read-интерфейсы `IUserItems`, `IUserProgress` и `IUserRankUpQuest`. Изменения выполняют игровые
сервисы через соответствующие `*Commands` интерфейсы. Например, кнопка добавления карты вызывает
`CardCollectionService`, а не изменяет пользовательский инвентарь напрямую.

## Как добавить новую группу игровых данных

1. Добавить доменный `Definition` и `Catalog` в capability, которому принадлежат данные.
2. Добавить capability-конфиг и его валидацию.
3. Добавить ссылку на конфиг в `GameDataManifestConfig`.
4. Добавить отдельный `Declarations/*Declaration.cs` и коллекцию в `GameDataDeclaration`.
5. Обновить `ScriptableObjectGameDataSource`.
6. Если поддерживается сериализованный источник, добавить `*DocumentEntry.cs`, поле в `GameDataDocument` и маппинг.
7. Обновить `GameDataCompiler` и `GameDataSnapshot`.
8. Зарегистрировать новый каталог в `GameDataInstaller`.
9. Запустить `tools/Validate-Architecture.ps1` и тесты.

При изменении внешнего формата следует решить, совместимо ли изменение со старым JSON. Для несовместимого изменения
нужно увеличить `CurrentVersion` и явно определить миграцию либо поведение для неподдерживаемой версии.
