package skua
{
	import flash.external.ExternalInterface;
	import skua.remote.RemoteRegistry;
	import skua.module.Modules;
	
	public class Externalizer
	{
		
		public function Externalizer()
		{
			super();
		}
		
		public function init(root:Main):void
		{
			this.addCallback("loadClient", Main.loadGame);
			this.addCallback("getGameObject", Main.getGameObject);
			this.addCallback("getGameObjectS", Main.getGameObjectS);
			this.addCallback("getGameObjectKey", Main.getGameObjectKey);
			this.addCallback("setGameObject", Main.setGameObject);
            this.addCallback("setGameObjectKey", Main.setGameObjectKey);
			this.addCallback("getArrayObject", Main.getArrayObject);
			this.addCallback("setArrayObject", Main.setArrayObject);
			this.addCallback("callGameFunction", Main.callGameFunction);
			this.addCallback("callGameFunction0", Main.callGameFunction0);
			this.addCallback("selectArrayObjects", Main.selectArrayObjects);
			this.addCallback("connectToServer", Main.connectToServer);
			this.addCallback("isNull", Main.isNull);
			
			this.addCallback("clickServer", Main.clickServer);
			
			this.addCallback("isLoggedIn", Main.isLoggedIn);
			this.addCallback("isKicked", Main.isKicked);
			
			this.addCallback("infiniteRange", Main.infiniteRange);
			this.addCallback("canUseSkill", Main.canUseSkill);
			this.addCallback("useSkill", Main.useSkill);
			
			this.addCallback("rejectExcept", Main.rejectExcept);
			
			this.addCallback("walkTo", Main.walkTo);
			this.addCallback("jumpCorrectRoom", Main.jumpCorrectRoom);
			
			this.addCallback("availableMonsters", Main.availableMonstersInCell);
			this.addCallback("attackMonsterName", Main.attackMonsterByName);
			this.addCallback("attackMonsterID", Main.attackMonsterByID);
			this.addCallback("untargetSelf", Main.untargetSelf);
			this.addCallback("attackPlayer", Main.attackPlayer);
			this.addCallback("getMonsterHealth", Main.getMonsterHealth);
			this.addCallback("getMonsterHealthById", Main.getMonsterHealthById);
			
			this.addCallback("buyItemByName", Main.buyItemByName);
			this.addCallback("buyItemByID", Main.buyItemByID);
			this.addCallback("getShopItem", Main.getShopItem);
			this.addCallback("getShopItemByID", Main.getShopItemByID);
			
			this.addCallback("sendClientPacket", Main.sendClientPacket);
			this.addCallback("catchPackets", Main.catchPackets);
			
			this.addCallback("disableDeathAd", Main.disableDeathAd);
			this.addCallback("skipCutscenes", Main.skipCutscenes);
			this.addCallback("hidePlayers", Main.hidePlayers);
			this.addCallback("magnetize", Main.magnetize);
			this.addCallback("disableFX", Main.disableFX);
			this.addCallback("killLag", Main.killLag);
			
			// aura reading
			this.addCallback("auraComparison", Main.auraComparison);
			this.addCallback("isTrue", Main.isTrue);
			this.addCallback("getSubjectAuras", Main.getSubjectAuras);
			this.addCallback("GetAurasValue", Main.GetAurasValue);
			this.addCallback("GetAuraSecondsRemaining", Main.GetAuraSecondsRemaining);
			this.addCallback("HasAnyActiveAura", Main.HasAnyActiveAura);
			this.addCallback("HasAllActiveAuras", Main.HasAllActiveAuras);
			this.addCallback("GetTotalAuraStacks", Main.GetTotalAuraStacks);
			this.addCallback("getAvatar",Main.getAvatar);
			
			this.addCallback("injectScript", Main.injectScript);
			
			this.addCallback("UserID", Main.UserID);
			this.addCallback("gender", Main.Gender);
			
			this.addCallback("lnkCreate", RemoteRegistry.ext_create);
			this.addCallback("lnkDestroy", RemoteRegistry.ext_destroy);
			this.addCallback("lnkGetChild", RemoteRegistry.ext_getChild);
			this.addCallback("lnkDeleteChild", RemoteRegistry.ext_deleteChild);
			this.addCallback("lnkGetValue", RemoteRegistry.ext_getValue);
			this.addCallback("lnkSetValue", RemoteRegistry.ext_setValue);
			this.addCallback("lnkCall", RemoteRegistry.ext_call);
			this.addCallback("lnkGetArray", RemoteRegistry.ext_getArray);
			this.addCallback("lnkSetArray", RemoteRegistry.ext_setArray);
			
			this.addCallback("fcCreate", RemoteRegistry.ext_fcCreate);
			this.addCallback("fcPush", RemoteRegistry.ext_fcPushArgs);
			this.addCallback("fcPushDirect", RemoteRegistry.ext_fcPushArgsDirect);
			this.addCallback("fcClear", RemoteRegistry.ext_fcClearArgs);
			this.addCallback("fcCallFlash", RemoteRegistry.ext_fcCallFlash);
			this.addCallback("fcCall", RemoteRegistry.ext_fcCall);
			
			this.addCallback("modEnable", Modules.enable);
			this.addCallback("modDisable", Modules.disable);
			
			this.debug("Externalizer::init done.");
			this.call("requestLoadGame");
		}
		
		public function addCallback(name:String, func:Function):void
		{
			ExternalInterface.addCallback(name, func);
		}
		
		public function call(name:String, ... rest):*
		{
			return ExternalInterface.call(name, rest);
		}
		
		public function debug(message:String):void
		{
			this.call("debug", message);
		}
	}
}
