namespace BoH_MyLibrary
{
    public enum Field
    {
        Id,
        Label,
        Category,
        IconUnlocked,
        DescriptionUnlocked,
        ValidateOnStoreFront,
        SingleDescription,
    }
    public class AchievementFilter
    {
        private Field _field;
        public Field GetField() => _field;
        private string[] _values;
        public string[] GetValues() => _values;
        public AchievementFilter(Field field, string[] values) { _field = field; _values = values; }

        public void Filter(ref AchievementData data)
        {
            List<AchievementCategory> newCategories = [];
            List<AchievementCategory> categories = data.AchievementCategories();
            foreach (AchievementCategory category in categories)
            {
                List<Achievement> achievements = category.Achievements().Where(item => _values.Contains(item.GetByField(_field))).ToList();
                if (achievements.Count == 0) continue;
                AchievementCategory cur = new AchievementCategory(category.Id(), category.Label(), category.IconUnlocked(), achievements);
                newCategories.Add(cur);
            }
            data = new AchievementData(newCategories);
        }
    }
}
