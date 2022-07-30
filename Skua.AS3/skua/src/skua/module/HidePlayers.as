package skua.module 
{
	import skua.Main;
	
	public class HidePlayers extends Module
	{
		public function HidePlayers()
		{
			super("HidePlayers");
		}
		
		override public function onToggle(game:*):void 
		{
			for (var id:* in game.world.avatars)
			{
				var avatar:* = game.world.avatars[id];
				if (!avatar.isMyAvatar && avatar.pMC)
				{
					avatar.pMC.mcChar.visible = !enabled;
					avatar.pMC.pname.visible = !enabled;
					avatar.pMC.shadow.visible = !enabled;
					if (avatar.petMC)
					{
						avatar.petMC.visible = !enabled;
					}
				}
			}
		}
		
		override public function onFrame(game:*):void
		{
			onToggle(game);
		}
	}
}