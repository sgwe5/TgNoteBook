using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TgNoteBook.DataLearn.Models;
using TgNoteBook.DataLearn.Repos;

namespace TgNoteBook.Commands.Command;

public class Command
{
    private static readonly ConcurrentDictionary<long, bool> _awaitingUserResponse = new ConcurrentDictionary<long, bool>();
    private static readonly ConcurrentDictionary<long, (string Title, string Text)> _userNotes = new ConcurrentDictionary<long, (string Title, string Text)>();
    private static readonly ConcurrentDictionary<long, bool> _awaitingNoteDeletion = new ConcurrentDictionary<long, bool>();
    private static NoteBookRepos _noteBookRepos;

    public static void Initialize(NoteBookRepos noteBookRepos)
    {
        _noteBookRepos = noteBookRepos;
    }

    public static async Task SlashStart(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        var firstName = update.Message.From.FirstName;
        var startKeyboard = new ReplyKeyboardMarkup(
            new List<KeyboardButton[]>()
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Чем полезен ежедневник?"),
                    new KeyboardButton("Меню ежедневника"),
                }
            })
        {
            ResizeKeyboard = true,
        };
        await botClient.SendTextMessageAsync(
            chatId: update.Message.Chat.Id,
            $"Привет, {firstName}! \ud83e\udd1a \n" +
            "Это ежедневник, чтобы ты мог записывать свои дела и не забывать их \u270f\ufe0f",
            replyMarkup: startKeyboard,
            cancellationToken: cancellationToken);
    }

    public static async Task NoteMenu(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var menukeyboard = new ReplyKeyboardMarkup(
            new List<KeyboardButton[]>()
        {
            new KeyboardButton[]
            {
                new KeyboardButton("Добавить заметку"),
                new KeyboardButton("Посмотреть заметки"),
            },
            new KeyboardButton[]
            {
                new KeyboardButton("Удалить заметки"),
                new KeyboardButton("Удалить заметку"),
            }
        })
            {
                ResizeKeyboard = true,
            };
        await botClient.SendTextMessageAsync(
            chatId: update.Message.Chat.Id,
            "Выбери действие",
            replyMarkup: menukeyboard,
            cancellationToken: cancellationToken);
    }

    public static async Task AddNote(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var chatId = update.Message.Chat.Id;
        
        _awaitingUserResponse[chatId] = true;
        _userNotes[chatId] = (null, null);

        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Напиши мне свои дела, которые ты бы хотел сохранить... \ud83d\udcdd"
                  + "Напиши сначала заголовок, потом следующим сообщением полный ежедневник",
            cancellationToken: cancellationToken);
    }

    public static async Task HandleMessage(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        var chatId = update.Message.Chat.Id;
        if (_awaitingUserResponse.TryGetValue(chatId, out var isAwaiting) && isAwaiting)
        {
            if (_userNotes.TryGetValue(chatId, out var note))
            {
                if (note.Title == null)
                {
                    _userNotes[chatId] = (update.Message.Text, null);

                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Отлично! Теперь напишите полный текст заметки.",
                        cancellationToken: cancellationToken);
                }
                else
                {
                    await _noteBookRepos.AddAsync(new NoteBookEntity
                    {
                        TgChatId = chatId,
                        Title = note.Title,
                        Text = update.Message.Text
                    });

                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Ваше сообщение сохранено! ✅",
                        cancellationToken: cancellationToken);

                    _awaitingUserResponse[chatId] = false;
                    _userNotes.TryRemove(chatId, out _);
                }
            }
        }
        else if (_awaitingNoteDeletion.TryGetValue(chatId, out var isAwaitingDeletion) && isAwaitingDeletion)
        {
            var title = update.Message.Text;
            var note = await _noteBookRepos.GetNoteTitle(chatId, title);
            if (note != null)
            {
                await _noteBookRepos.DeleteTitleAsync(chatId, title);
    
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"Заметка с заголовком '{title}' была удалена. 🗑️",
                    cancellationToken: cancellationToken);
            }
            else
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"Заметка с заголовком '{title}' не найдена. ❌",
                    cancellationToken: cancellationToken);
            }

            _awaitingNoteDeletion[chatId] = false;
        }
    }

    public static async Task GetNote(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var chatId = update.Message.Chat.Id;
        var notes = await _noteBookRepos.GetNotes(chatId);
        string messageText;
        
        if (notes.Any())
        {
            var notesMessage = notes.Select(note => $"Заголовок: {note.Title} \n \n" + $"{note.Text} \n \n" + $"Создано в: {note.Created} \n");
            messageText = "Ваши заметки:\n \n" + string.Join("\n", notesMessage); 
        }
        else
        {
            messageText = "У вас нет заметок";
        }
        await botClient.SendTextMessageAsync(
            chatId: update.Message.Chat.Id,
            text: messageText,
            cancellationToken: cancellationToken
        );
    }

    public static async Task NoteFaq(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var chatId = update.Message.Chat.Id;
        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Ежедневник нужен для планирования и организации времени. Он помогает записывать задачи, встречи и цели на день, неделю или месяц. С его помощью легче отслеживать прогресс, расставлять приоритеты и избегать забывчивости. Ежедневник также может служить источником мотивации, когда видишь, как постепенно выполняются поставленные задачи.",
            cancellationToken: cancellationToken);
    }
    
    public static async Task DeleteOneNote(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var chatId = update.Message.Chat.Id;

        _awaitingNoteDeletion[chatId] = true;

        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Введите заголовок заметки, которую вы хотите удалить:",
            cancellationToken: cancellationToken);
    }

    public static async Task DeleteAllNote(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        var chatId = update.Message.Chat.Id;

        var deletekeyboard = new ReplyKeyboardMarkup(
            new List<KeyboardButton[]>()
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Да, удалить"),
                    new KeyboardButton("НЕЕЕЕЕЕТ \ud83d\ude2d"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Вернуться обратно")
                }
            })
        {
            ResizeKeyboard = true,
        };
        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Точно хотите всё удалить?",
            replyMarkup: deletekeyboard,
            cancellationToken: cancellationToken);
    }

    public static async Task MessageByDelete(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        var chatId = update.Message.Chat.Id;

        var note = await _noteBookRepos.GetNotes(chatId);
        if (note.Count != 0)
        {
            await _noteBookRepos.DeleteAllAsync(chatId);
            
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Заметки были удалены. 🗑️",
                cancellationToken: cancellationToken);
        }
        else
        {
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Заметки не найдены. ❌",
                cancellationToken: cancellationToken);
        }
    }
}