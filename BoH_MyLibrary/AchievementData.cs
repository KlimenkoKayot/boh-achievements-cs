namespace BoH_MyLibrary
{
    public struct AchievementData
    {
        private List<AchievementCategory> _achievementCategories;
        public List<AchievementCategory> AchievementCategories() => _achievementCategories;

        public void AddAchievementCategory(AchievementCategory category) => _achievementCategories.Add(category);

        public AchievementData()
        {
            _achievementCategories = new List<AchievementCategory>();
        }
        public AchievementData(List<AchievementCategory> achievementCategories) => _achievementCategories = achievementCategories;
    }
}
