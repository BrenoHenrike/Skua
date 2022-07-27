using Skua.Core.ViewModels;

namespace Skua.Core.Messaging;
public record class RemoveFastTravelMessage(FastTravelItemViewModel FastTravel);
public record class EditFastTravelMessage(/*FastTravelItemViewModel OldFastTravel, */FastTravelItemViewModel FastTravel);
