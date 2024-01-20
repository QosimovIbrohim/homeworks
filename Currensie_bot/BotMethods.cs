using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Currensie_bot
{
    public class BotMethods
    {
        public static string BotToken;
        List<string> currencyList = new List<string>
            {
            "AED", "AUD", "CAD", "CHF", "CNY", "DKK", "EGP",
            "EUR", "GBP", "ISK", "JPY", "KRW", "KWD", "KZT",
            "LBP", "MYR", "NOK", "PLN", "RUB", "SEK", "SGD",
            "TRY", "UAH", "USD"
             };

        public BotMethods(string tkn) { BotToken = tkn; }
        public async Task BotHandle()
        {
            var client = new TelegramBotClient(BotToken);

            using CancellationTokenSource cts = new CancellationTokenSource();

            ReceiverOptions reseiverOption = new ReceiverOptions()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            client.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: reseiverOption,
                cancellationToken: cts.Token
                );
            var me = await client.GetMeAsync();
            Console.WriteLine($"Start listeneing for @{me.Username}");
            Console.ReadLine();
            cts.Cancel();

        }

        public async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellation)
        {
            Message message;
            string messageText = "";
            if (update.Message != null)
            {
                message = update.Message;
            }
            else { return; }

            if (message.Text != null)
            {
                messageText = message.Text;
            }
            else
            {
                return;
            }
            var chatId = message.Chat.Id;

            if (isInCurrensyList(messageText))
            {
                string s = await SearchWithCode(messageText);
                await client.SendTextMessageAsync(
                chatId: chatId,
                text: $"{s} sum",
                cancellationToken: cancellation
                );
                return;
            }
            if (message.Text == "/start")
            {

                var replyKeyboard = new ReplyKeyboardMarkup(
                         new List<KeyboardButton[]>()
                          {
                        new KeyboardButton[]
                        {
                            new KeyboardButton("AED"),
                            new KeyboardButton("AUD"),
                            new KeyboardButton("CAD"),
                            new KeyboardButton("CHF"),
                            new KeyboardButton("CNY"),
                            new KeyboardButton("DKK"),
                            new KeyboardButton("EGP"),

                        },
                        new KeyboardButton[]
                        {
                            new KeyboardButton("EUR"),
                            new KeyboardButton("GBP"),
                            new KeyboardButton("ISK"),
                            new KeyboardButton("JPY"),
                            new KeyboardButton("KRW"),
                            new KeyboardButton("KWD"),
                            new KeyboardButton("KZT"),

                        },
                         new KeyboardButton[]
                        {
                            new KeyboardButton("LBP"),
                            new KeyboardButton("MYR"),
                            new KeyboardButton("NOK"),
                            new KeyboardButton("PLN"),
                            new KeyboardButton("RUB"),
                            new KeyboardButton("SEK"),
                            new KeyboardButton("SGD"),

                        },
                         new KeyboardButton[]
                        {
                            new KeyboardButton("TRY"),
                            new KeyboardButton("UAH"),
                            new KeyboardButton("USD"),
                        }

                          })
                {
                    ResizeKeyboard = true
                };

                await client.SendTextMessageAsync(
                    chatId: chatId,
                    cancellationToken: cancellation,
                    text: "Assalomu Aleykum, Botga hush kelibsiz",
                    replyMarkup: replyKeyboard
                    );
                return;
            }
        }

        public async Task HandlePollingErrorAsync(ITelegramBotClient client, Exception ex, CancellationToken cancellation)
        {
            var ErrorMeassage = ex switch
            {
                ApiRequestException apiEx
                => $"Telegram API Error:\n[{apiEx.ErrorCode}]\n[{apiEx.Message}]",
                _ => ex.ToString()
            }; ;
            Console.WriteLine(ErrorMeassage);
        }
        public async Task<string> SearchWithCode(string code)
        {
            HttpClient client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://nbu.uz/uz/exchange-rates/json/");

            var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();
            List<JObject> lst = JsonConvert.DeserializeObject<List<JObject>>(body)!;
            foreach (var item in lst)
            {
                if (item["code"].ToString() == code)
                    return $"{item["cb_price"]}";
            }
            return "error";
        }
        public bool isInCurrensyList(string name)
        {
            foreach (var item in currencyList)
            {
                if (name == item)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
