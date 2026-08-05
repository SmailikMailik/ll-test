# Игровые данные

Игровые данные описывают общие для всех пользователей правила и справочники: ранги, карты, героев, задания,
варианты повышения и награды. После загрузки они неизменяемы и доступны потребителям как доменные каталоги.

Нормативные правила размещения и границ представлений определены в
[`Architecture.md`](../Standards/Architecture.md). Пользовательские defaults, сохранения и состояние сессии описаны в
[`User-Data.md`](User-Data.md), а модель рангов и повышения — в
[`Ranks-And-Rank-Up.md`](Ranks-And-Rank-Up.md).

## Поток данных

```text
GameDataManifestConfig
    -> ScriptableObjectGameDataSource
    -> GameDataDeclaration
    -> CompiledDataLoader + GameDataCompiler
       -> GameDataDeclarationValidator
    -> GameDataSnapshot
    -> RankCatalog, CardCatalog, HeroCatalog, QuestCatalog, RankUpCatalog, RewardCatalog
```

`ProjectLifetimeScope` выбирает источник из `ScriptableObject` через `GameDataLoaderFactory`.
`GameDataInstaller` загружает один `GameDataSnapshot` и регистрирует каждый каталог отдельно.

Альтернативный `SerializedGameDataSource` читает версионируемый `GameDataDocument`, преобразует его в ту же
`GameDataDeclaration` и тем самым повторно использует компилятор и все доменные проверки:

```text
JSON / storage
    -> SerializedGameDataSource
    -> GameDataDocument
    -> GameDataDocumentMapper
    -> GameDataDeclaration
    -> GameDataCompiler (GameDataDeclarationValidator)
```

Источник читает данные синхронно через `IDataSource<TDeclaration>.Read()`. Оба пути сходятся на декларации, поэтому
формат хранения не влияет на compiler, snapshot и потребителей.

## Представления и ответственность

| Представление | Ответственность |
| --- | --- |
| capability `*CatalogConfig` | Unity-authoring одной группы данных |
| `GameDataManifestConfig` | Ссылки на все обязательные игровые конфиги |
| `GameDataDocument` и `*DocumentEntry` | Версионируемый внешний контракт |
| `GameDataDeclaration` и `*Declaration` | Единый непроверенный формат для любого источника |
| `GameDataDeclarationValidator` | Проверка ID, дубликатов и межкаталожных ссылок |
| `GameDataCompiler` | Координация проверки и создание доменных объектов в каноническом порядке |
| `GameDataSnapshot` | Один согласованный результат загрузки |
| `*Catalog` и доменные типы | Неизменяемые данные для игровых сервисов и UI |

`Config`, `Document` и `Declaration` не передаются игровым сервисам или UI. Конфиги отдельных возможностей остаются
рядом со своими владельцами (`Game/Ranks/Configuration`, `Game/Heroes/Configuration` и т. п.); только граница
агрегированной загрузки находится в `Game/Data`.

`GameDataDocument.CurrentVersion` сейчас равен `1`. Миграции старых версий пока не поддерживаются: сериализованный
игровой документ с другой версией отклоняется как несовместимый источник данных. Состав `Document`, `Declaration` и
`Snapshot` одинаков на уровне групп данных: `Ranks`, `Cards`, `Heroes`, `Quests`, `RankUps` и `Rewards`.
Полиморфный JSON-контракт требований повышения подробно описан в
[`Ranks-And-Rank-Up.md`](Ranks-And-Rank-Up.md#представления-данных-и-json-контракт).

## Где искать код

| Путь | Что находится |
| --- | --- |
| `Game/Data/Configuration` | `GameDataManifestConfig` |
| `Game/Data/Sources` | Адаптеры `ScriptableObject` и сериализованного источника |
| `Game/Data/Persistence` | Внешний документ и преобразование в декларацию |
| `Game/Data/Declarations` | Source-neutral непроверенное представление |
| `Game/Data/GameDataDeclarationValidator.cs` | Общая проверка declaration и межкаталожных ссылок |
| `Game/Data/GameDataCompiler.cs` | Сборка доменных каталогов и корневого snapshot-а |
| `Game/Data/GameDataSnapshot.cs` | Корень неизменяемого агрегата |
| `Composition/Factories/GameDataLoaderFactory.cs` | Выбор технологии источника |
| `Composition/Installers/GameDataInstaller.cs` | Загрузка и DI-регистрация каталогов |

Созданные Unity-ассеты лежат в `Assets/_Project/Configuration/Game`; отдельная папка на каждую возможность не нужна,
пока у неё только один ассет.

## Отладка и расширение

Чтобы проследить значение, пройдите цепочку:

```text
capability config -> GameDataManifestConfig -> source -> declaration -> GameDataCompiler (validator) -> catalog
```

При добавлении новой группы игровых данных обычно нужно:

1. Добавить доменные типы и capability-конфиг с валидацией.
2. Включить конфиг в `GameDataManifestConfig` и `ScriptableObjectGameDataSource`.
3. Добавить декларацию, компиляцию и каталог в `GameDataSnapshot`.
4. Если сериализованный источник поддерживается, обновить document, mapper и при несовместимости увеличить версию.
5. Зарегистрировать каталог в `GameDataInstaller` и добавить focused tests.

После изменения границ или состава агрегата следует выполнить проверки из
[`Validation.md`](../Standards/Validation.md).
