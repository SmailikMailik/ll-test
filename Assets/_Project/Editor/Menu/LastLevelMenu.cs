namespace LLEditor.Menu
{
    internal static class LastLevelMenu
    {
        private const string RootPath = "Last Level/";

        internal const string OpenMainScenePath = RootPath + "Open Main Scene";
        internal const string ValidateProjectDataPath = RootPath + "Validate Project Data";
        internal const string DeleteSavedUserDataPath = RootPath + "Delete Saved User Data";

        internal const string GamePath = RootPath + "Game/";
        internal const string PresentationPath = RootPath + "Presentation/";
        internal const string UIPath = RootPath + "UI/";
        internal const string UserPath = RootPath + "User/";

        internal const int ContentPriority = 0;
        internal const int OpenMainScenePriority = 100;
        internal const int ValidateProjectDataPriority = 101;
        internal const int DeleteSavedUserDataPriority = 102;
    }
}