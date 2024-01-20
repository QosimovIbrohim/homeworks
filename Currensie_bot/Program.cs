using Currensie_bot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class Project
{
    static async Task Main(string[] args)
    {
        const string MyBotToken = "6472325940:AAGj_B19PayR_mNqhtpQsA9rbFZS9MRz3tM";
        BotMethods bot = new BotMethods(MyBotToken);
        await bot.BotHandle();
    }
}