using SmiteCommon.Models.Api;
using SmiteCommon.Models.Items;
using System.Text.Json;

namespace SmiteApi;

internal class Program
{
    static async Task Main(string[] args)
    {
        if(args.Length < 3)
        {
            Console.WriteLine("Please provide the following: <devId> <authKey> <apiQuery>");
        }

        Developer developer = new()
        {
            Id = args[0],
            AuthKey = args[1]
        };

        Configuration configuration = Configuration.Load();
        Client client = new(developer, configuration.Session);
        await client.CheckSession(configuration);

        string query = args[2];
        string path = args.Length > 3 ? args[3] : null;
        object json = null;
        switch (query)
        {
            case Paths.GetItems:
                json = await client.GetItems(configuration.Language);
                break;

            case Paths.GetGods:
                json = await client.GetGods(configuration.Language);
                break;

            default:
                Console.WriteLine($"Unknown query: {query}");
                break;
        }

        if(path != null && json != null)
        {
            try
            {
                string directory = Path.GetDirectoryName(path);
                if (directory != string.Empty && !Directory.Exists(path))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(path, JsonSerializer.Serialize(json, Configuration.JsonSerializerOptions));
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}