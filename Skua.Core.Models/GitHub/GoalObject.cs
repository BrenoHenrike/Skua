using Newtonsoft.Json;

namespace Skua.Core.Models.GitHub;

public class GoalObject
{
    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("desc")]
    public string Description { get; set; }

    [JsonProperty("cur_value")]
    public int CurrentValue { get; set; }

    [JsonProperty("goal_value")]
    public int GoalValue { get; set; }
}