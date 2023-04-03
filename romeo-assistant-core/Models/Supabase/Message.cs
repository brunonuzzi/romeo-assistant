using Postgrest.Attributes;
using Postgrest.Models;

namespace romeo_assistant_core.Models.Supabase;

[Table("message")]
public class Message : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("prompt_fk")]
    public int PromptId { get; set; }

    [Reference(typeof(Prompt))]
    public Prompt? Prompt { get; set; }

    [Column("message_type")]
    public MessageType MessageType { get; set; }

    [Column("message_Id")]
    public string? MessageId { get; set; }

    [Column("user_Id")]
    public string? UserId { get; set; }

    [Column("user_name")]
    public string? UserName { get; set; }

    [Column("phone")]
    public string? Phone { get; set; }

    [Column("message")]
    public string? MessageText { get; set; }

    [Column("token_size")]
    public int TokenSize { get; set; }

    [Column("created_at")] public DateTime CreatedAt { get; set; }
}