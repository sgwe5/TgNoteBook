using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Telegram.Bot.Types;
using TgNoteBook.DataLearn.Models;

namespace TgNoteBook.DataLearn.Configuration;

public class UserConfig : IEntityTypeConfiguration <UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(t => t.Id);
    }
}
