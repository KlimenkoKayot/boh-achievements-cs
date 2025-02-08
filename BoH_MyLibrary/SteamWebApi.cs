using System.Net;

namespace BoH_MyLibrary
{
    public static class SteamWebApi
    {
        private static string GetGameAchievementsString(string gameid)
        {
            var client = new WebClient();
            string response = client.DownloadString("http://api.steampowered.com/ISteamUserStats/GetGlobalAchievementPercentagesForApp/v0002/?gameid=1028310&format=json");
            return response;
        }

        public static JsonObject GetGameAchievementsJson(string gameid)
        {
            return new JsonObject(GetGameAchievementsString(gameid));
        }
    }
}