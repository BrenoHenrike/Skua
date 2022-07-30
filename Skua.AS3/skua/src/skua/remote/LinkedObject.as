package skua.remote
{
	
	public class LinkedObject
	{
		private var _id:int;
		private var _parent:*;	
		private var _name:String;
		private var _children:*;
		
		public function LinkedObject(id:int, parent:*, name:String)
		{
			super();
			this._id = id;
			this._parent = parent;
			this._name = name;
			this._children = new Object();
		}
		
		public function getId():int
		{
			return this._id;
		}
		
		public function getValue():*
		{
			return this._parent[this._name];
		}
		
		public function setValue(value:*):void
		{
			this._parent[this._name] = value;
		}
		
		public function removeChild(name:String):void
		{
			var linked:LinkedObject = this._children[name];
			if (linked != null)
			{
				RemoteRegistry.destroy(linked);
				delete this._children[name];
			}
		}
		
		public function getChild(path:String):LinkedObject
		{
			if (this._children[path] != null)
			{
				return this._children[path];
			}
			var parts:Array = path.split(".");
			var name:String = parts.pop();
			var parent:* = this._parent[_name];
			for (var i:int = 0; i < parts.length; i++)
			{
				parent = parent[parts[i]];
			}
			return this._children[path] = RemoteRegistry.createLinked(parent, name);
		}
		
		public function call(name:String, ... args):LinkedObject
		{
			var func:Function = this._parent[this._name][name] as Function;
			var result:* = func.apply(null, args);
			var parent:ObjectParent = new ObjectParent(result);
			return RemoteRegistry.createLinked(parent, "_obj");
		}
		
		public function stringify():String
		{
			return JSON.stringify(this._parent[this._name]);
		}
	}
}
