using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TgNoteBook.Commands.Command;
using TgNoteBook.DataLearn.Repos;

namespace TgNoteBook;

public class Bot
{
    private static ITelegramBotClient _botClient;
    private static ReceiverOptions _receiverOptions;
    private static NoteBookRepos _noteBookRepos;
    public Bot(NoteBookRepos noteBookRepos)
    {
        _noteBookRepos = noteBookRepos;
        Command.Initialize(noteBookRepos);
    }
    public async void Start()
    {
        _botClient = new TelegramBotClient("7470726372:AAEgAt3O3SxbJduE4h8Rx7pOZsfN-b8xK9A");

        _receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[]
            {
                UpdateType.Message,
                UpdateType.CallbackQuery,
            },
            ThrowPendingUpdates = true,
        };
        using var cts = new CancellationTokenSource();
        _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token);
        var me = await _botClient.GetMeAsync(cts.Token);
        Console.WriteLine($"{me.FirstName} запущен");
        await Task.Delay(-1, cts.Token);
    }

    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var chatId = update.Message.Chat.Id;
        if (update.Type == UpdateType.Message && update.Message!.Text != null)
        {
                switch (update.Message.Text)
                {
                    case "/start":
                        Command.SlashStart(botClient, update, cancellationToken);
                        break;
                    case "Меню ежедневника":
                        Command.NoteMenu(botClient, update, cancellationToken);
                        break;
                    case "Добавить заметку":
                        Command.AddNote(botClient, update, cancellationToken);
                        break;
                    case "Посмотреть заметки":
                        Command.GetNote(botClient, update, cancellationToken);
                        break;
                    case "Удалить заметку":
                        Command.DeleteOneNote(botClient, update, cancellationToken);
                        break;
                    case "Удалить заметки":
                        Command.DeleteAllNote(botClient, update, cancellationToken);
                        break;
                    case "Чем полезен ежедневник?":
                        Command.NoteFaq(botClient, update, cancellationToken);
                        break;
                    case "Да, удалить":
                        Command.MessageByDelete(botClient, update, cancellationToken);
                        break;
                    case "НЕЕЕЕЕЕТ \ud83d\ude2d" or "Вернуться обратно":
                        Command.NoteMenu(botClient, update, cancellationToken);
                        break;
                    default:
                        Command.HandleMessage(botClient, update, cancellationToken);
                        break;
                }
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


