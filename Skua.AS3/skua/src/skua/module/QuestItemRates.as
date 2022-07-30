package skua.module 
{
	import flash.utils.getQualifiedClassName;
	public class QuestItemRates extends Module
	{
		
		public function QuestItemRates() 
		{
			super("QuestItemRates");
			enabled = true;
		}
		
		override public function onFrame(game:*):void 
		{
			if (game.ui.ModalStack.numChildren)
			{
				var cFrame:* = game.ui.ModalStack.getChildAt(0);
				if (getQualifiedClassName(cFrame) == "QFrameMC" && cFrame.cnt.core && cFrame.cnt.core.rewardsRoll)
				{
					for (var i:int = 1; i < cFrame.cnt.core.rewardsRoll.numChildren; i++)
					{
						var rew:* = cFrame.cnt.core.rewardsRoll.getChildAt(i);
						if (rew.strType.text.indexOf("%") == -1)
						{
							for each (var r:* in cFrame.qData.reward)
							{
								if (r.ItemID == rew.ItemID && (!rew.strQ.visible || r.iQty.toString() == rew.strQ.text.substring(1)))
								{
									rew.strType.text += " (" + r.iRate + "%)";
									rew.strType.width = 100;
									rew.strRate.visible = false;
								}
							}
						}
					}
				}
			}
		}
	}
}