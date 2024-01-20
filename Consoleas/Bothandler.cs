using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Consoleas
{
    public class Bothandler
    {
        public static string BotToken;
        public static string firstPath;
        public static string secondPath;
        public static string folderName;
        public static bool once = true;
        public static string zipName;

        public HashSet<long> chatIDS = new HashSet<long>();

        public Bothandler(string botToken)
        {
            BotToken = botToken;
        }
        public async Task Bothandle()
        {
            var client = new TelegramBotClient(BotToken);

            using CancellationTokenSource cts = new CancellationTokenSource();

            ReceiverOptions reseiverOption = new ReceiverOptions()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            client.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandkePollingErrorAsync,
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
            chatIDS.Add(update.Message.Chat.Id);
            Message message;
            if (update.Message != null)
            {
                message = update.Message;
            }
            else
            {
                return;
            }
           


            if (message.Text == "/start")
            {
                await client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Assalomu Aleykum File zip qiluvchi botga hush kelibsiz\nFile ni zip holatda olish uchun file nomini jo'nating",
                    cancellationToken: cancellation);
                return;
            }

            else if (message.Type == MessageType.Text)
            {
                foreach (var id in chatIDS)
                {
                    Console.Write($"{id} - iddagi inson shu pathdagi fileni so'radi - >");
                    folderName = message.Text;

                    string[] drives = Directory.GetLogicalDrives();

                    foreach (var drive in drives)
                    {
                        SearchDirections(drive, folderName);
                    }

                    if (firstPath == "")
                    {
                        await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Topilmadi",
                        cancellationToken: cancellation);

                        return;
                    }

                    zipName = firstPath + ".zip";
                    try
                    {
                        ZipFile.CreateFromDirectory(firstPath, zipName);
                    }
                    catch { }

                    await using Stream stream = System.IO.File.OpenRead($"{zipName}");
                    await client.SendDocumentAsync(chatId: id, document: InputFile.FromStream(stream: stream, fileName: $"{folderName}.zip"), cancellationToken: cancellation);
                    Console.WriteLine($"{zipName}");
                }
                System.IO.File.Delete(zipName);
                firstPath = "";
                secondPath = "";
                folderName = "";
                zipName = "";
                return;
            }

            static string SearchDirections(string folderPath, string folderName)
            {
                try
                {
                    foreach (string file in Directory.GetDirectories(folderPath, folderName))
                    {
                        if (file != null || file != "")
                        {
                            firstPath = file!;
                        }
                    }

                    foreach (string subDir in Directory.GetDirectories(folderPath))
                    {
                        SearchDirections(subDir, folderName);
                    }
                }

                catch (Exception)
                { }

                return "";
            }
        }

        public async Task HandkePollingErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken cancellationToken)
        {
            var ErrorMeassage = ex switch
            {
                ApiRequestException apiEx
                => $"Telegram API Error:\n[{apiEx.ErrorCode}]\n[{apiEx.Message}]",
                _ => ex.ToString()
            }; ;
            Console.WriteLine(ErrorMeassage);
        }
    }
}
