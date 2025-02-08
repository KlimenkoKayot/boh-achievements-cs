namespace BoH_MyLibrary
{
    public static class AchievementParser
    {
        public static Achievement FromJsonToAchievement(JsonObject obj)
        {
            return new Achievement(obj);
        }

        public static AchievementCategory FromJsonToAchievementCategory(JsonObject obj)
        {
            return new AchievementCategory(obj);
        }

        public static AchievementData FromJsonToAchievementData(JsonObject obj)
        {
            AchievementData data = new AchievementData();
            IEnumerable<JsonObject> categories = obj.GetArrayField("achievements");
            
            if (categories == null)
            {
                return data;
            }

            // щас жопа будет
            AchievementCategory lastCategory = new AchievementCategory();
            foreach (JsonObject cur in categories)
            {
                switch (cur.GetField("isCategory"))
                {
                    case (null):
                        {
                            // тут Achievement
                            Achievement currentAchievement = new Achievement(cur);
                            lastCategory.AddAchievement(currentAchievement);
                            break;
                        }
                    default:
                        {
                            // тут AchievementCategory
                            if (lastCategory.Achievements().Count > 0)
                            {
                                data.AddAchievementCategory(lastCategory);
                            }
                            AchievementCategory currentCategory = new AchievementCategory(cur);
                            lastCategory = currentCategory;
                            break;
                        }
                }
            }
            data.AddAchievementCategory(lastCategory);
            return data;
        }
    }
}
