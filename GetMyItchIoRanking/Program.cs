using System.CommandLine;
using System.Runtime.InteropServices;

namespace GetMyItchIoRanking
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                RunNoArg();
            }
        }


        //shitty code be like...
        public static void RunNoArg()
        {
            Console.Clear();
            Console.WriteLine("Enter GameID");
            Console.Write("\n>");
            var returned = Console.ReadLine();
            if(int.TryParse(returned, out int gameID))
            {
                Console.Clear();
                Console.WriteLine("Enter the category of your app");
                foreach (var category in Enum.GetValues(typeof(RankFetcher.RankingCategory)))
                {
                    Console.WriteLine($"{(int)category} - {RankFetcher.CategoryStrings[(RankFetcher.RankingCategory)category]}");
                }

                inputReadCategory:
                Console.Write("\n>");
                var CategoryInput = Console.ReadLine();
                if (!int.TryParse(CategoryInput, out int CategoryResultint) || CategoryResultint >= Enum.GetValues(typeof(RankFetcher.RankingCategory)).Length)
                {
                    Console.WriteLine("Invalid value entered, try again");
                    goto inputReadCategory;
                }

                Console.Clear();
                Console.WriteLine("Enter the ranking");
                foreach (var type in Enum.GetValues(typeof(RankFetcher.RankingType)))
                {
                    Console.WriteLine($"{(int)type} - {RankFetcher.TypeStrings[(RankFetcher.RankingType)type]}");
                }

                inputRead:
                Console.Write("\n>");
                var TypeInput = Console.ReadLine();
                if(!int.TryParse(TypeInput, out int TypeResultint) || TypeResultint >= Enum.GetValues(typeof(RankFetcher.RankingType)).Length)
                {
                    Console.WriteLine("Invalid value entered, try again");
                    goto inputRead;
                }

                var task = RankFetcher.GetRanking((RankFetcher.RankingCategory)CategoryResultint, (RankFetcher.RankingType)TypeResultint, gameID);
                task.Wait();
                if(task.Result != -1)
                {
                    Console.WriteLine($"Your app is ranked #{task.Result} in {RankFetcher.TypeStrings[(RankFetcher.RankingType)TypeResultint]}");
                }
                else
                {
                    Console.WriteLine("An error occurred while fetching the ranking.");
                }
            }
            else
            {
                Console.WriteLine("Invalid value entered.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
/*
            Games,
            Tools,
            GameAssets,
            Comics,
            Books,
            PhysicalGames,
            Soundtracks,
            GameMods,
            Miscellaneous
*/