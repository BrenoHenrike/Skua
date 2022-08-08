using Skua.Core.ViewModels;

namespace Skua.Core.Messaging;
public sealed record RemoveFastTravelMessage(FastTravelItemViewModel FastTravel);
public sealed record EditFastTravelMessage(FastTravelItemViewModel FastTravel);
