package skua.module 
{
	public class DisableCollisions extends Module
	{
		private var _old:*;
		private var _oldR:*;
		
		public function DisableCollisions() 
		{
			super("DisableCollisions");
		}
		
		override public function onToggle(game:*):void 
		{
			if (enabled)
			{
				_old = game.world.arrSolid;
				_oldR = game.world.arrSolidR;
				game.world.arrSolid = [];
				game.world.arrSolidR = [];
			}
			else
			{
				game.world.arrSolid = _old;
				game.world.arrSolidR = _oldR;
			}
		}
		
		override public function onFrame(game:*):void 
		{
			onToggle(game);
		}
	}

}