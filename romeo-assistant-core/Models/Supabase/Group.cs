using Postgrest.Attributes;
using Postgrest.Models;

namespace romeo_assistant_core.Models.Supabase;

[Table("group")]
public class Group : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("group_id")]
    public string? GroupId { get; set; }

    [Column("group_name")]
    public string? GroupName { get; set; }

    [Column("active_mode")] public bool ActiveMode { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}