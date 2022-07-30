package skua.remote
{
	import skua.Main;
	
	public class RemoteRegistry
	{
		
		private static var cid:int = 0;
		
		private static var _linked:* = new Object();
		
		public function RemoteRegistry()
		{
			super();
		}
		
		public static function createLinked(parent:*, name:String):LinkedObject
		{
			var id:int = cid++;
			return _linked[id] = new LinkedObject(id, parent, name);
		}
		
		public static function destroy(object:LinkedObject):void
		{
			delete _linked[object.getId()];
		}
		
		public static function getObject(id:int):LinkedObject
		{
			return _linked[id];
		}
		
		public static function ext_create(path:String):String
		{
			var parts:Array = path.split(".");
			var name:String = parts.pop();
			return createLinked(skua.Main._getObjectA(skua.Main.instance.getGame(), parts), name).getId().toString();
		}
		
		public static function ext_destroy(id:int):void
		{
			destroy(_linked[id]);
		}
		
		public static function ext_getChild(id:int, path:String):String
		{
			return getObject(id).getChild(path).getId().toString();
		}
		
		public static function ext_deleteChild(id:int, path:String):void
		{
			getObject(id).removeChild(path);
		}
		
		public static function ext_getValue(id:int):String
		{
			return JSON.stringify(getObject(id).getValue());
		}
		
		public static function ext_setValue(id:int, value:*):void
		{
			getObject(id).setValue(value);
		}
		
		public static function ext_call(id:int, func:String, ... args):String
		{
			return JSON.stringify((getObject(id)["call"] as Function).apply(null, [func].concat(args)));
		}
		
		public static function getFunctionCaller(id:int):FunctionCaller
		{
			return getObject(id).getValue() as FunctionCaller;
		}
		
		public static function ext_fcCreate(obj:int, name:String):String
		{
			var parent:ObjectParent = new ObjectParent(new FunctionCaller(getObject(obj).getValue(), name));
			return createLinked(parent, "_obj").getId().toString();
		}
		
		public static function ext_fcPushArgsDirect(id:int, ... args):void
		{
			getFunctionCaller(id).pushArgs(args);
		}
		
		public static function ext_fcPushArgs(id:int, argIds:Array):void
		{
			var vals:* = [];
			for (var i:int = 0; i < argIds.length; i++)
			{
				vals[i] = getObject(argIds[i]).getValue();
			}
			getFunctionCaller(id).pushArgs(vals);
		}
		
		public static function ext_fcClearArgs(id:int):void
		{
			getFunctionCaller(id).clearArgs();
		}
		
		public static function ext_fcCallFlash(id:int):String
		{
			var val:* = getFunctionCaller(id).call();
			var parent:ObjectParent = new ObjectParent(val);
			return createLinked(parent, "_obj").getId().toString();
		}
		
		public static function ext_fcCall(id:int):String
		{
			return JSON.stringify(getFunctionCaller(id).call());
		}
		
		public static function ext_getArray(id:int, index:int):String
		{
			var parent:ObjectParent = new ObjectParent(getObject(id).getValue()[index]);
			return createLinked(parent, "_obj").getId().toString();
		}
		
		public static function ext_setArray(id:int, index:int, value:*):void
		{
			getObject(id).getValue()[index] = value;
		}
	}
}
