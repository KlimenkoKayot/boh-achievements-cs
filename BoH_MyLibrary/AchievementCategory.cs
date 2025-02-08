using System.Diagnostics.Metrics;

namespace BoH_MyLibrary
{
    public struct AchievementCategory
    {
        private string _id;
        public string Id() => _id;

        private string _label;
        public string Label() => _label;
        
        private bool _iconUnlocked;
        public bool IconUnlocked() => _iconUnlocked;

        private List<Achievement> _achievements;
        public List<Achievement> Achievements() => _achievements;

        public void AddAchievement(Achievement achievement) => _achievements.Add(achievement);
        
        public AchievementCategory()
        {
            _id = "NO_CATEGORY_ACHIEVEMENTS";
            _label = "NO_CATEGORY_ACHIEVEMENTS";
            _iconUnlocked = false;
            _achievements = new List<Achievement>();
        }

        public AchievementCategory(JsonObject obj)
        {
            _id = obj.GetField("id");
            _label = obj.GetField("label");
            _iconUnlocked = obj.GetField("iconUnlocked") == "true";
            _achievements = new List<Achievement>();
        }
        public AchievementCategory(string id, string label, bool iconUnlocked, List<Achievement> achievements)
        {
            _id = id;
            _label = label;
            _iconUnlocked = iconUnlocked;
            _achievements = achievements;
        }
    }
}
