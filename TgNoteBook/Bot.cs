using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TgNoteBook;

public class Bot
{
    private static ITelegramBotClient _botClient;
    private static ReceiverOptions _receiverOptions;
    public async void Start()
    {
        _botClient = new TelegramBotClient("7470726372:AAEgAt3O3SxbJduE4h8Rx7pOZsfN-b8xK9A");

        _receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[]
            {
                UpdateType.Message,
            },
            ThrowPendingUpdates = true,
        };
        using var cts = new CancellationTokenSource();

        _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token);

        var me = await _botClient.GetMeAsync();
        Console.WriteLine($"{me.FirstName} запущен");
        await Task.Delay(-1);

    }
    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    {
                        var callbackQuery = update.CallbackQuery;
                        var user = callbackQuery.From;

                        Console.WriteLine("Пришло сообщение");
                        return;
                    }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
    private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        var ErrorMessage = error switch
        {
            ApiRequestException apiRequestException
            => $"Telegram API Error: \n [{apiRequestException.ErrorCode}] \n [{apiRequestException.Message}]",
            _ => error.ToString()
        };
        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}


