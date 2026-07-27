namespace LL.Upgrades
{
    internal readonly struct ExperienceApplication
    {
        internal int GrantedExperience { get; }
        internal int AppliedExperience { get; }
        internal int LostExperience { get; }
        internal bool HasLoss => LostExperience > 0;

        internal ExperienceApplication(int grantedExperience, int appliedExperience)
        {
            GrantedExperience = grantedExperience;
            AppliedExperience = appliedExperience;
            LostExperience = grantedExperience - appliedExperience;
        }
    }
}