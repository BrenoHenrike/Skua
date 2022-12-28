using Google.Protobuf.Collections;

namespace Skua.gRPC.Server;

public class ScriptContainer
{
    public readonly string BaseUrl = "https://raw.githubusercontent.com/brenohenrike/skua/master/repos";
    public Dictionary<GrpcScriptRepo, RepeatedField<GrpcScriptInfo>> Scripts { get; } = new();
    public RepeatedField<GrpcScriptRepo> Repos { get; } = new();
}
