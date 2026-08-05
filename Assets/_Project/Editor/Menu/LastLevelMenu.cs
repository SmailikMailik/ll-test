namespace LLEditor.Menu
{
    internal static class LastLevelMenu
    {
        private const string RootPath = "Last Level/";

        internal static class Content
        {
            internal const string Path = RootPath + "Content/";

            // Priority gaps create separators between ownership groups.
            internal const int GamePriority = 0;
            internal const int PresentationPriority = 20;
            internal const int UIPriority = 40;
            internal const int UserPriority = 60;
        }

        internal static class Scenes
        {
            internal const string Path = RootPath + "Scenes/";
            internal const int Priority = 0;
        }

        internal static class Commands
        {
            internal const string ValidateProjectDataPath = RootPath + "Validate Project Data";
            internal const string DeleteSavedUserDataPath = RootPath + "Delete Saved User Data";

            internal const int ValidateProjectDataPriority = 100;
            internal const int DeleteSavedUserDataPriority = 101;
        }
    }
}