using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace GetMyItchIoRanking
{
    internal class RankFetcher
    {
        static HttpClient _httpClient = new HttpClient();
        public static async Task<int> GetRanking(RankingCategory category ,RankingType type, int gameID)
        {
            if (!Functions.TryGetValue(category, out var rankingTypes)) throw new NotImplementedException();
            if (!rankingTypes.TryGetValue(type, out var func)) throw new NotImplementedException();

            return await func(gameID);
        }

        const int DelayTimer = 2000;
        public static async Task<int> Fetch(RankingCategory category, int searchedID)
        {
            for (int i = 1; i <= 200; i++)
            {
                Console.Clear();
                Console.WriteLine($"Fetching page {i} for {CategoryStrings[category]}");
                Console.WriteLine($"{(200/i)*100}%");
                int rankCounter = 0;
                string url = $"https://itch.io/{CategoryStrings[category]}/?page={i}&format=json";
                var response = await _httpClient.GetAsync(url);
                if (CheckGameID(await response.Content.ReadAsStringAsync(), searchedID, out int rank))
                {
                    return rank + rankCounter;
                }
                else
                {
                    rankCounter += rank;
                    await Task.Delay(DelayTimer);
                }
            }
            return -1;
        }

        public static async Task<int> Fetch(RankingCategory category, RankingType type, int searchedID)
        {
            for (int i = 1; i <= 200; i++)
            {
                Console.Clear();
                Console.WriteLine($"Fetching page {i} for {CategoryStrings[category]}");
                int rankCounter = 0;
                string url = $"https://itch.io/{CategoryStrings[category]}/{TypeStrings[type]}?page={i}&format=json";
                var response = await _httpClient.GetAsync(url);
                if (CheckGameID(await response.Content.ReadAsStringAsync(), searchedID, out int rank))
                {
                    return rank + rankCounter;
                }
                else
                {
                    rankCounter += rank;
                    await Task.Delay(DelayTimer);
                }
            }
            return -1;
        }

        public static bool CheckGameID(string json, int gameID, out int Rank)
        {
            ItchResponse data = JsonSerializer.Deserialize<ItchResponse>(json);
            const string key = "data-game_id=\"";
            int index = 0;
            int rankCounter = 0;
            while ((index = data.Content.IndexOf(key, index)) != -1)
            {
                index += key.Length;
                rankCounter++;
                int end = data.Content.IndexOf('"', index);
                var id = int.Parse(data.Content.Substring(index, end - index));
                if (id == gameID)
                {
                    Rank = rankCounter;
                    return true;
                }
                index = end;
            }
            Rank = rankCounter;
            return false;
        }
        public enum RankingCategory
        {
            Games,
            Tools,
            GameAssets,
            Comics,
            Books,
            PhysicalGames,
            Soundtracks,
            GameMods,
            Miscellaneous
        }
        public enum RankingType
        {
            Popular,
            NewAndPopular,
            TopSellers,
            TopRated
        }
        public static readonly Dictionary<RankingCategory, string> CategoryStrings = new()
        {
            { RankingCategory.Games, "games" },
            { RankingCategory.Tools, "tools" },
            { RankingCategory.GameAssets, "game-assets" },
            { RankingCategory.Comics, "comics" },
            { RankingCategory.Books, "books" },
            { RankingCategory.PhysicalGames, "physical-games" },
            { RankingCategory.Soundtracks, "soundtracks" },
            { RankingCategory.GameMods, "game-mods" },
            { RankingCategory.Miscellaneous, "miscellaneous" }
        };
        public static readonly Dictionary<RankingType, string> TypeStrings = new()
        {
            { RankingType.Popular, "popular" },
            { RankingType.NewAndPopular, "new-and-popular" },
            { RankingType.TopSellers, "top-sellers" },
            { RankingType.TopRated, "top-rated" }
        };

        static Dictionary<RankingType, Func<int, Task<int>>> Create(RankingCategory category)
        {
            return new()
            {
                { RankingType.Popular, searchedID => Fetch(category, searchedID)},
                { RankingType.NewAndPopular, searchedID => Fetch(category, RankingType.NewAndPopular, searchedID)},
                { RankingType.TopSellers, searchedID => Fetch(category, RankingType.TopSellers, searchedID)},
                { RankingType.TopRated, searchedID => Fetch(category, RankingType.TopRated, searchedID)},
            };
        }

        public static Dictionary<RankingCategory, Dictionary<RankingType, Func<int, Task<int>>>> Functions = new()
        {
            {RankingCategory.Games, Create(RankingCategory.Games)},
            {RankingCategory.Tools, Create(RankingCategory.Tools)},
            {RankingCategory.GameAssets, Create(RankingCategory.GameAssets)},
            {RankingCategory.Comics, Create(RankingCategory.Comics)},
            {RankingCategory.Books, Create(RankingCategory.Books)},
            {RankingCategory.PhysicalGames, Create(RankingCategory.PhysicalGames)},
            {RankingCategory.Soundtracks, Create(RankingCategory.Soundtracks)},
            {RankingCategory.GameMods, Create(RankingCategory.GameMods)},
            {RankingCategory.Miscellaneous, Create(RankingCategory.Miscellaneous)},
        };
    }
}
