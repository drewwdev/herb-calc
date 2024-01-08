using System.Text.Json;
using RestSharp;

class Program
{
    static readonly Dictionary<string, List<string>> SeedToPotionMap =
        new()
        {
            {
                "Guam seed",
                new List<string> { "Attack potion" }
            },
            {
                "Marrentill seed",
                new List<string> { "Antipoison" }
            },
            {
                "Tarromin seed",
                new List<string> { "Strength potion", "Serum 207" }
            },
            {
                "Harralander seed",
                new List<string>
                {
                    "Compost potion",
                    "Restore potion",
                    "Energy potion",
                    "Combat potion"
                }
            },
            {
                "Ranarr seed",
                new List<string> { "Defence Potion", "Prayer potion" }
            },
            {
                "Toadflax seed",
                new List<string> { "Agility potion", "Saradomin brew" }
            },
            {
                "Irit seed",
                new List<string> { "Super Attack", "Superantipoison" }
            },
            {
                "Avantoe seed",
                new List<string> { "Fishing potion", "Super energy", "Hunter potion" }
            },
            {
                "Kwuarm seed",
                new List<string> { "Super strength" }
            },
            {
                "Snapdragon seed",
                new List<string> { "Super restore" }
            },
            {
                "Cadantine seed",
                new List<string> { "Super defence" }
            },
            {
                "Lantadyme seed",
                new List<string> { "Antifire potion", "Magic potion" }
            },
            {
                "Dwarf weed seed",
                new List<string> { "Ranging potion", "Ancient brew", "Menaphite remedy" }
            },
            {
                "Torstol seed",
                new List<string> { "Zamorak brew" }
            }
        };

    static readonly Dictionary<string, int> ItemNameToIdMap =
        new()
        {
            { "Guam leaf", 249 },
            { "Marrentill", 251 },
            { "Tarromin", 253 },
            { "Harralander", 255 },
            { "Ranarr weed", 257 },
            { "Toadflax", 2998 },
            { "Irit leaf", 259 },
            { "Avantoe", 261 },
            { "Kwuarm", 263 },
            { "Snapdragon", 3000 },
            { "Cadantine", 265 },
            { "Lantadyme", 2481 },
            { "Dwarf weed", 267 },
            { "Torstol", 269 },
            { "Guam seed", 5291 },
            { "Marrentill seed", 5292 },
            { "Tarromin seed", 5293 },
            { "Harralander seed", 5294 },
            { "Ranarr seed", 5295 },
            { "Toadflax seed", 5296 },
            { "Irit seed", 5297 },
            { "Avantoe seed", 5298 },
            { "Kwuarm seed", 5299 },
            { "Snapdragon seed", 5300 },
            { "Cadantine seed", 5301 },
            { "Lantadyme seed", 5302 },
            { "Dwarf weed seed", 5303 },
            { "Torstol seed", 5304 },
            { "Attack potion(3)", 121 },
            { "Antipoison(3)", 175 },
            { "Strength potion(3)", 115 },
            { "Serum 207(3)", 3410 },
            { "Compost potion(3)", 6472 },
            { "Restore potion(3)", 127 },
            { "Energy potion(3)", 3010 },
            { "Combat potion(3)", 9741 },
            { "Defence potion(3)", 133 },
            { "Prayer potion(3)", 139 },
            { "Agility potion(3)", 3034 },
            { "Saradomin brew(3)", 6687 },
            { "Super attack(3)", 145 },
            { "Superantipoison(3)", 181 },
            { "Fishing potion(3)", 151 },
            { "Super energy(3)", 3018 },
            { "Hunter potion(3)", 10000 },
            { "Super strength(3)", 157 },
            { "Super restore(3)", 3026 },
            { "Super defence(3)", 163 },
            { "Antifire potion(3)", 2454 },
            { "Magic potion(3)", 3042 },
            { "Ranging potion(3)", 169 },
            { "Zamorak brew(3)", 189 }
        };

    static void Main()
    {
        Console.WriteLine("Welcome to OSRS Potion Calculator!");

        string seed = GetUserChoice(
            "Select the seed you are using:",
            SeedToPotionMap.Keys.ToArray()
        );

        if (!SeedToPotionMap.ContainsKey(seed))
        {
            Console.WriteLine($"Error: Invalid seed selection - {seed}");
            return;
        }

        List<string> possiblePotions = SeedToPotionMap[seed];
        string potion = GetUserChoice(
            "Select the potion you intend to make:",
            possiblePotions.ToArray()
        );

        ConvertSeedsToPotions(seed, potion);
    }

    static string GetUserChoice(string prompt, string[] options)
    {
        Console.WriteLine(prompt);
        for (int i = 0; i < options.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {options[i]}");
        }

        int choiceIndex;
        do
        {
            Console.Write("Enter the number corresponding to your choice: ");
        } while (
            !int.TryParse(Console.ReadLine(), out choiceIndex)
            || choiceIndex < 1
            || choiceIndex > options.Length
        );

        return options[choiceIndex - 1];
    }

    static int ConvertSeedsToPotions(string seed, string potion)
    {
        int averageHerbsPerPatch = 9;
        string formattedPotionName = potion + "(3)";
        ;

        // get seed id from name
        int seedId = ItemNameToIdMap[seed];
        // get potion id from name
        int potionId = ItemNameToIdMap[formattedPotionName];

        // Fetch live GE prices for seed, and potion
        int seedPrice = GetItemPrice(seedId);
        int potionPrice = GetItemPrice(potionId);

        int totalCost = seedPrice * averageHerbsPerPatch;
        int totalPotions = potionPrice * averageHerbsPerPatch;
        int profit = totalPotions - totalCost;

        Console.WriteLine(
            $"Converting {averageHerbsPerPatch} {seed} to {averageHerbsPerPatch} {potion}(3) costs {totalCost} coins and profits {profit} coins."
        );

        return totalPotions;
    }

    static int GetItemPrice(int itemId)
    {
        var apiUrl = $"https://prices.runescape.wiki/api/v1/osrs/latest?id={itemId}";

        var client = new RestClient(apiUrl);
        var request = new RestRequest { Method = Method.Get, RequestFormat = DataFormat.Json };
        request.AddHeader("User-Agent", "seed-to-potion-profit-calc/1.0");

        var response = client.Execute<JsonElement>(request);

        if (
            response.IsSuccessful
            && response.Data.TryGetProperty("data", out var data)
            && data.TryGetProperty(itemId.ToString(), out var itemData)
            && itemData.TryGetProperty("high", out var high)
        )
        {
            return high.GetInt32();
        }

        Console.WriteLine($"Error fetching or parsing price data for item ID {itemId}");
        return -1;
    }
}
