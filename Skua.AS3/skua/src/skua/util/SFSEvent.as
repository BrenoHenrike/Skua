package skua.util
{
	import flash.events.Event;
	
	public class SFSEvent extends Event
	{
		public static const onAdminMessage:String = "onAdminMessage";
		public static const onBuddyList:String = "onBuddyList";
		public static const onBuddyListError:String = "onBuddyListError";
		public static const onBuddyListUpdate:String = "onBuddyListUpdate";
		public static const onBuddyPermissionRequest:String = "onBuddyPermissionRequest";
		public static const onBuddyRoom:String = "onBuddyRoom";
		public static const onConfigLoadFailure:String = "onConfigLoadFailure";
		public static const onConfigLoadSuccess:String = "onConfigLoadSuccess";
		public static const onConnection:String = "onConnection";
		public static const onConnectionLost:String = "onConnectionLost";
		public static const onCreateRoomError:String = "onCreateRoomError";
		public static const onDebugMessage:String = "onDebugMessage";
		public static const onExtensionResponse:String = "onExtensionResponse";
		public static const onJoinRoom:String = "onJoinRoom";
		public static const onJoinRoomError:String = "onJoinRoomError";
		public static const onLogin:String = "onLogin";
		public static const onLogout:String = "onLogout";
		public static const onModeratorMessage:String = "onModMessage";
		public static const onObjectReceived:String = "onObjectReceived";
		public static const onPrivateMessage:String = "onPrivateMessage";
		public static const onPublicMessage:String = "onPublicMessage";
		public static const onRandomKey:String = "onRandomKey";
		public static const onRoomAdded:String = "onRoomAdded";
		public static const onRoomDeleted:String = "onRoomDeleted";
		public static const onRoomLeft:String = "onRoomLeft";
		public static const onRoomListUpdate:String = "onRoomListUpdate";
		public static const onRoomVariablesUpdate:String = "onRoomVariablesUpdate";
		public static const onRoundTripResponse:String = "onRoundTripResponse";
		public static const onSpectatorSwitched:String = "onSpectatorSwitched";
		public static const onUserCountChange:String = "onUserCountChange";
		public static const onUserEnterRoom:String = "onUserEnterRoom";
		public static const onUserLeaveRoom:String = "onUserLeaveRoom";
		public static const onUserVariablesUpdate:String = "onUserVariablesUpdate";
		
		public var params:Object;
		
		public function SFSEvent(param1:String, param2:Object)
		{
			super(param1);
			this.params = param2;
		}
		
		public override function clone():Event
		{
			return new SFSEvent(this.type, this.params);
		}
		
		public override function toString():String
		{
			return formatToString("SFSEvent", "type", "bubbles", "cancelable", "eventPhase", "params");
		}
	}
}