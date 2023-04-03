using Postgrest.Attributes;
using Postgrest.Models;

namespace romeo_assistant_core.Models.Supabase;

[Table("prompt")]
public class Prompt : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("group_fk")]
    public int GroupId { get; set; }

    [Reference(typeof(Group))]
    public Group? Group { get; set; }

    [Column("prompt_text")]
    public string? PromptText { get; set; }

    [Column("token_size")]
    public int TokenSize { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}