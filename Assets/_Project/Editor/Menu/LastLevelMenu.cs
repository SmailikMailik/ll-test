namespace LLEditor.Menu
{
    internal static class LastLevelMenu
    {
        private const string RootPath = "Last Level/";

        internal const string ValidateProjectDataPath = RootPath + "Validate Project Data";
        internal const string DeleteSavedUserDataPath = RootPath + "Delete Saved User Data";

        internal const string ContentPath = RootPath + "Content/";
        internal const string ScenesPath = RootPath + "Scenes/";

        internal const int GameContentPriority = 0;
        internal const int ProjectContentPriority = 20;
        internal const int UserContentPriority = 40;
        internal const int ScenePriority = 0;
        internal const int ValidateProjectDataPriority = 100;
        internal const int DeleteSavedUserDataPriority = 101;
    }
}