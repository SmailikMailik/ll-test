namespace LLEditor.Menu
{
    internal static class LLMenu
    {
        internal const string RootPath = "Tools/LL/";

        internal const string OpenMainScenePath = RootPath + "Open Main Scene";
        internal const string DeleteSavedUserDataPath = RootPath + "Delete Saved User Data";

        internal const string GamePath = RootPath + "Game/";
        internal const string UserPath = RootPath + "User/";
        internal const string PresentationPath = RootPath + "Presentation/";
        internal const string UIPath = RootPath + "UI/";

        internal const int OpenMainScenePriority = 0;
        internal const int DeleteSavedUserDataPriority = 1;
        internal const int ContentPriority = 100;
    }
}