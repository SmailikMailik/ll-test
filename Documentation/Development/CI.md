# Локальные команды CI

Репозиторий предоставляет не зависящие от CI-провайдера точки входа. Запускайте их из корня проекта при закрытом
Unity Editor. Задайте `UNITY_EDITOR_PATH`, если Unity не установлен ни по одному из путей, обнаруживаемых
`tools/ci/Common.ps1`.

Текущий workflow GitHub Actions только упаковывает `main` и проверяет целостность архива; он не запускает эти точки
входа для валидации, тестирования или сборки Unity. Добавьте лицензированный Unity runner, прежде чем считать
публикацию архива проверкой качества кода.

## Валидация

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/ci/Validate.ps1
```

Запускает структурную валидацию архитектуры, импортирует и компилирует Unity-проект, проверяет включённые сцены сборки
и запускает валидацию данных проекта. Перед запуском Unity команда выполняет фикстуры валидаторов соглашений,
структурную валидацию архитектуры, валидацию стиля C# и валидацию соглашений Odin Inspector. Логи записываются в
`artifacts/unity-validation.log`.

Во время работы используйте направленные проверки:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/tests/Test-ConventionValidators.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools/Validate-Architecture.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools/Validate-CodeStyle.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools/Validate-OdinInspector.ps1
```

Соответствие видов изменений проверкам определено в [`Validation.md`](../Standards/Validation.md); этот документ
определяет только порядок запуска команд.

## Тестирование

Размещение тестов и соглашения по их созданию определены в [`Testing.md`](../Standards/Testing.md).

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/ci/Test.ps1 -Platform EditMode
powershell -NoProfile -ExecutionPolicy Bypass -File tools/ci/Test.ps1 -Platform PlayMode
```

Записывает совместимый с NUnit XML и логи Unity в `artifacts`.

## Сборка

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/ci/Build.ps1 `
  -Target StandaloneWindows64 `
  -Output Builds/Windows/LL-Test.exe `
  -Version 1.0.0 `
  -BuildNumber 1
```

Стандартные пути вывода поддерживаются для `StandaloneWindows64`, `Android` и `WebGL`. Для остальных targets
необходимо передать явный путь вывода. Сцены сборки читаются из `EditorBuildSettings`; `Bootstrap` должен быть первой
включённой сценой.

## Окружение CI

Конвейер должен публиковать весь каталог `artifacts`, даже если команда Unity завершилась с ошибкой. Поддерживаются
следующие переменные окружения:

- `UNITY_EDITOR_PATH`: полный путь к исполняемому файлу Unity.
- `LL_BUILD_OUTPUT`: путь вывода player-сборки; задаётся `Build.ps1`.
- `LL_BUILD_VERSION`: `PlayerSettings.bundleVersion`; задаётся `Build.ps1`.
- `LL_BUILD_NUMBER`: положительный код версии Android; задаётся `Build.ps1`.

Не храните данные активации Unity, учётные данные подписи или учётные данные магазинов в репозитории. Передавайте их
через хранилище секретов выбранного CI-провайдера.
