package skua
{
	
	public class ExtractedFuncs
	{
		public function ExtractedFuncs()
		{
			super();
		}
		
		public static function actionTimeCheck(param1:*):Boolean
		{
			var finalCD:* = 0;
			var currentTime:* = new Date().getTime();
			var hasteMultiplier:* = 1 - Math.min(Math.max(Main.instance.getGame().world.myAvatar.dataLeaf.sta.$tha, -1), 0.5);
			if (param1.auto)
			{
				if (Main.instance.getGame().world.autoActionTimer.running)
				{
					trace("AA TIMER SELF-CLIPPING");
					return false;
				}
				return true;
			}
			if (currentTime - Main.instance.getGame().world.GCDTS < Main.instance.getGame().world.GCD)
			{
				return false;
			}
			if (param1.OldCD != null)
			{
				finalCD = Math.round(param1.OldCD * hasteMultiplier);
			}
			else
			{
				finalCD = Math.round(param1.cd * hasteMultiplier);
			}
			if (currentTime - param1.ts >= finalCD)
			{
				delete param1.OldCD;
				return true;
			}
			return false;
		}
	}
}
