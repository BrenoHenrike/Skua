package skua
{
	import skua.Externalizer;
	import skua.ExtractedFuncs;
    import adobe.utils.ProductManager;
    import flash.display.Loader;
    import flash.display.LoaderInfo;
    import flash.display.MovieClip;
    import flash.display.Stage;
    import flash.display.StageScaleMode;
    import flash.display.StageAlign;
    import flash.events.Event;
    import flash.events.MouseEvent;
    import flash.events.ProgressEvent;
    import flash.net.URLLoader;
    import flash.net.URLRequest;
    import flash.system.ApplicationDomain;
    import flash.system.LoaderContext;
    import flash.system.Security;
    import flash.utils.getQualifiedClassName;
    import skua.util.SFSEvent;
    import skua.module.Modules;
    
    public class Main extends MovieClip
    {
        public static var instance:Main;
        
        private static var _gameClass:Class;
        private static var _fxStore:* = new Object();
        private static var _fxLastOpt:Boolean = false;
        private static var _handler:*;
        
        private var game:*;
        private var external:skua.Externalizer;
        
        private var sURL:String = "https://game.aq.com/game/";
        private var versionUrl:String = (sURL + "api/data/gameversion");
        private var loginURL:String = (sURL + "api/login/now");
		
        private var sFile:String;
		private var sBG:String = "Generic2.swf";
        private var isEU:Boolean;
        private var urlLoader:URLLoader;
        private var loader:Loader;
        private var vars:Object;
        private var sTitle:String = "<font color=\"#FDAF2D\">Birb</font>";
        
        private var stg:Stage;
        private var gameDomain:ApplicationDomain;
        
        public function Main()
        {
            String.prototype.trim = function():String
            {
                return this.replace(/^\s+|\s+$/g, "");
            };
            
            Main.instance = this;
            
            if (stage) this.init();
            else addEventListener(Event.ADDED_TO_STAGE, this.init);
        }
        
        public static function loadGame():void
		{
            Main.instance.onAddedToStage();
        }
        
        private function init(e:Event = null):void
        {
            removeEventListener(Event.ADDED_TO_STAGE, this.init);
            this.external = new skua.Externalizer();
            this.external.init(this);
        }
        
        private function onAddedToStage():void
        {
            Security.allowDomain("*");
            this.urlLoader = new URLLoader();
            this.urlLoader.addEventListener(Event.COMPLETE, this.onDataComplete);
            this.urlLoader.load(new URLRequest(this.versionUrl));
        }
        
        private function onDataComplete(event:Event):void
        {
            this.urlLoader.removeEventListener(Event.COMPLETE, this.onDataComplete);
            this.vars = JSON.parse(event.target.data);
			this.sFile = ((this.vars.sFile + "?ver=") + Math.random());
            this.loadGame()
        }
        
        private function loadGame():void
        {
            this.loader = new Loader();
            this.loader.contentLoaderInfo.addEventListener(Event.COMPLETE, this.onComplete);
            this.loader.load(new URLRequest(this.sURL + "gamefiles/" + this.sFile));
        }
        
        private function onComplete(event:Event):void
        {
            this.loader.contentLoaderInfo.removeEventListener(Event.COMPLETE, this.onComplete);
            
            this.stg = stage;
            this.stg.removeChildAt(0);
            this.game = this.stg.addChild(this.loader.content);
            this.stg.scaleMode = StageScaleMode.SHOW_ALL;
            this.stg.align = StageAlign.TOP;
            
            for (var param:String in root.loaderInfo.parameters)
            {
                this.game.params[param] = root.loaderInfo.parameters[param];
            }
            
            this.game.params.vars = this.vars;
            this.game.params.sURL = this.sURL;
            this.game.params.sTitle = this.sTitle;
            this.game.params.sBG = this.sBG;
			this.game.params.isEU = this.isEU;
            this.game.params.loginURL = this.loginURL;
            
            this.game.sfc.addEventListener(SFSEvent.onExtensionResponse, this.onExtensionResponse);
            this.gameDomain = LoaderInfo(event.target).applicationDomain;
            
            Modules.init();
            this.stg.addEventListener(Event.ENTER_FRAME, Modules.handleFrame);
            
            this.external.call("loaded");
        }
        
        public function onExtensionResponse(packet:*):void
        {
            this.external.call("pext", JSON.stringify(packet));
        }
        
        public function getGame():*
        {
            return this.game;
        }
        
        public function getExternal():skua.Externalizer
        {
            return this.external;
        }
        
        public static function getGameObject(path:String):String
        {
            var obj:* = _getObjectS(instance.game, path);
            return JSON.stringify(obj);
        }
        
        public static function getGameObjectS(path:String):String
        {
            if (_gameClass == null)
            {
                _gameClass = instance.gameDomain.getDefinition(getQualifiedClassName(instance.game)) as Class;
            }
            var obj:* = _getObjectS(_gameClass, path);
            return JSON.stringify(obj);
        }
		
		public static function getGameObjectKey(path:String, key:String):String
        {
            var obj:* = _getObjectS(instance.game, path);
            var obj2:* = obj[key];
            return (JSON.stringify(obj2));
        }
        
        public static function setGameObject(path:String, value:*):void
        {
            var parts:Array = path.split(".");
            var varName:String = parts.pop();
            var obj:* = _getObjectA(instance.game, parts);
            obj[varName] = value;
        }

        public static function setGameObjectKey(path:String, key:String, value:*):void
        {
            var parts:Array = path.split(".");
            var obj:* = _getObjectA(instance.game, parts);
            obj[key] = value;
        }
        
        public static function getArrayObject(path:String, index:int):String
        {
            var obj:* = _getObjectS(instance.game, path);
            return JSON.stringify(obj[index]);
        }
        
        public static function setArrayObject(path:String, index:int, value:*):void
        {
            var obj:* = _getObjectS(instance.game, path);
            obj[index] = value;
        }
        
        public static function callGameFunction(path:String, ... args):String
        {
            var parts:Array = path.split(".");
            var funcName:String = parts.pop();
            var obj:* = _getObjectA(instance.game, parts);
            var func:Function = obj[funcName] as Function;
            return JSON.stringify(func.apply(null, args));
        }
        
        public static function callGameFunction0(path:String):String
        {
            var parts:Array = path.split(".");
            var funcName:String = parts.pop();
            var obj:* = _getObjectA(instance.game, parts);
            var func:Function = obj[funcName] as Function;
            return JSON.stringify(func.apply());
        }
        
        public static function selectArrayObjects(path:String, selector:String):String
        {
            var obj:* = _getObjectS(instance.game, path);
            if (!(obj is Array))
            {
                instance.external.debug("selectArrayObjects target is not an array");
                return "";
            }
            var array:Array = obj as Array;
            var narray:Array = new Array();
            for (var j:int = 0; j < array.length; j++)
            {
                narray.push(_getObjectS(array[j], selector));
            }
            return JSON.stringify(narray);
        }
        
        public static function _getObjectS(root:*, path:String):*
        {
            return _getObjectA(root, path.split("."));
        }
        
        public static function _getObjectA(root:*, parts:Array):*
        {
            var obj:* = root;
            for (var i:int = 0; i < parts.length; i++)
            {
                obj = obj[parts[i]];
            }
            return obj;
        }
        
        public static function isNull(path:String):String
        {
            try
            {
                return (_getObjectS(instance.game, path) == null).toString();
            }
            catch (ex:Error)
            {
            }
            return "true";
        }
        
        public static function isLoggedIn():String
        {
            return (instance.game != null && instance.game.sfc != null && instance.game.sfc.isConnected).toString();
        }
        
        public static function isKicked():String
        {
            return (instance.game.mcLogin != null && instance.game.mcLogin.warning.visible).toString();
        }
        
        public static function canUseSkill(index:int):String
        {
			var skill:* = instance.game.world.actions.active[index];
            return (instance.game.world.myAvatar.target != null && instance.game.world.myAvatar.target.dataLeaf.intHP > 0 && skua.ExtractedFuncs.actionTimeCheck(skill) && skill.isOK && (!skill.skillLock || !skill.lock)).toString();
        }
        
        public static function walkTo(xPos:int, yPos:int, walkSpeed:int):void
        {
			walkSpeed = (walkSpeed == 8 ? instance.game.world.WALKSPEED : walkSpeed);
			instance.game.world.myAvatar.pMC.walkTo(xPos, yPos, walkSpeed);
			instance.game.world.moveRequest({
				"mc":instance.game.world.myAvatar.pMC,
				"tx":xPos,
				"ty":yPos,
				"sp":walkSpeed
			});
        }
        
        public static function untargetSelf():void
        {
            var target:* = instance.game.world.myAvatar.target;
            if (target && target == instance.game.world.myAvatar)
            {
                instance.game.world.cancelTarget();
            }
        }
        
        public static function attackMonsterByID(id:int):void
        {
            var monster:* = instance.game.world.getMonster(id);
			attackTarget(monster);
        }
		
		public static function attackMonsterByName(name:String):void
		{
			var monster:* = getMonster(name);
			attackTarget(monster);
		}
        
        public static function attackPlayer(name:String):void
        {
            var player:* = instance.game.world.getAvatarByUserName(name.toLowerCase());
            attackTarget(player);
        }
		
		public static function getMonster(name:String):*
		{
			for each (var monster:* in instance.game.world.getMonstersByCell(instance.game.world.strFrame))
			{
				var monName:String = monster.objData.strMonName.toLowerCase();
				if ((monName.indexOf(name.toLowerCase()) > -1 || name == "*") && monster.pMC != null && monster.dataLeaf.intState > 0)
				{
					return monster;
				}
			}
			return null;
		}
		
		public static function availableMonstersInCell():String
		{
			var retMonsters:Array = [];
			for each (var monster:* in instance.game.world.getMonstersByCell(instance.game.world.strFrame))
			{
				if (monster.pMC != null)
				{
					retMonsters.push(monster.objData);
				}
			}
			return JSON.stringify(retMonsters);
		}
		
		private static function attackTarget(target:*):void
		{
			if (target != null && target.pMC != null)
			{
				instance.game.world.setTarget(target);
				instance.game.world.approachTarget();
			}
		}
        
        public static function useSkill(index:int):void
        {
            var skill:* = instance.game.world.actions.active[index];
            if (skua.ExtractedFuncs.actionTimeCheck(skill) && instance.game.world.myAvatar.target.dataLeaf.intHP > 0)
            {
				instance.game.world.testAction(skill);
            }
        }
        
        public static function magnetize():void
        {
            var target:* = instance.game.world.myAvatar.target;
            if (target)
            {
                target.pMC.x = instance.game.world.myAvatar.pMC.x;
                target.pMC.y = instance.game.world.myAvatar.pMC.y;
            }
        }
        
        public static function infiniteRange():void
        {
            for (var i:int = 0; i < 6; i++)
            {
                instance.game.world.actions.active[i].range = 20000;
            }
        }
        
        public static function skipCutscenes():void
        {
            while (instance.game.mcExtSWF.numChildren > 0)
            {
                instance.game.mcExtSWF.removeChildAt(0);
            }
            instance.game.showInterface();
        }
        
        public static function killLag(enable:Boolean):void
        {
            instance.game.world.visible = !enable;
        }
        
        public static function disableFX(enabled:Boolean):void
        {
            if (!_fxLastOpt && enabled)
            {
                _fxStore = new Object();
            }
            _fxLastOpt = enabled;
            for each (var avatar:* in instance.game.world.avatars)
            {
                if (enabled)
                {
                    if (avatar.pMC.spFX != null)
                    {
                        _fxStore[avatar.uid] = avatar.rootClass.spFX;
                    }
                    avatar.rootClass.spFX = null;
                }
                else
                {
                    avatar.rootClass.spFX = _fxStore[avatar.uid];
                }
            }
        }
        
        public static function hidePlayers(enabled:Boolean):void
        {
            for each (var avatar:* in instance.game.world.avatars)
            {
                if (avatar != null && avatar.pnm != null && !avatar.isMyAvatar)
                {
                    if (enabled)
                    {
                        avatar.hideMC();
                    }
                    else if (avatar.strFrame == instance.game.world.strFrame)
                    {
                        avatar.showMC();
                    }
                }
            }
        }
		
		public static function buyItemByName(name:String):void
		{
			for each(var item:* in instance.game.world.shopinfo.items)
			{
				if (item.sName.toLowerCase() == name.toLowerCase())
				{
					instance.game.world.sendBuyItemRequest(item);
					break;
				}
			}
		}
		
		public static function buyItemByID(id:int, shopItemID:int):void
		{
			for each(var item:* in instance.game.world.shopinfo.items)
			{
				if (item.ItemID == id && (shopItemID == 0 || item.ShopItemID == shopItemID))
				{
					instance.game.world.sendBuyItemRequest(item);
					break;
				}
			}
		}
        
        private static function parseDrop(name:*):*
        {
            var ret:* = new Object();
            ret.name = name.toLowerCase().trim();
            ret.count = 1;
            var regex:RegExp = /(.*)\s+x\s*(\d*)/g;
            var result:Object = regex.exec(name.toLowerCase().trim());
            if (result == null)
            {
                return ret;
            } 
            else
            {
                ret.name = result[1];
                ret.count = int(result[2]);
                return ret;
            }
        }
        
        public static function rejectExcept(whitelist:String):void
        {
            var pickup:Array = whitelist.split(",");
            if (instance.game.litePreference.data.bCustomDrops)
            {
                var source:* = instance.game.cDropsUI.mcDraggable ? instance.game.cDropsUI.mcDraggable.menu : instance.game.cDropsUI;
                for (var i: int = 0; i < source.numChildren; i++)
                {
                    var child:* = source.getChildAt(i);
                    if (child.itemObj)
                    {
                        var itemName:String = child.itemObj.sName.toLowerCase();
                        if (pickup.indexOf(itemName) == -1)
                        {
                            child.btNo.dispatchEvent(new MouseEvent(MouseEvent.CLICK));
                        }
                    }
                }
            }
            else
            {
                var children:int = instance.game.ui.dropStack.numChildren;
                for (i = 0; i < children; i++)
                {
                    child = instance.game.ui.dropStack.getChildAt(i);
                    var type:String = getQualifiedClassName(child);
                    if (type.indexOf("DFrame2MC") != -1)
                    {
                        var drop:* = parseDrop(child.cnt.strName.text);
                        var name:* = drop.name;
                        if (pickup.indexOf(name) == -1)
                        {
                            child.cnt.nbtn.dispatchEvent(new MouseEvent(MouseEvent.CLICK));
                        }
                    }
                }
            }
        }
		
        public static function injectScript(uri:String):void
        {
            var ploader:Loader = new Loader();
            ploader.contentLoaderInfo.addEventListener(Event.COMPLETE, onScriptLoaded);
            var context:LoaderContext = new LoaderContext();
            context.allowCodeImport = true;
            ploader.load(new URLRequest(uri), context);
        }
        
        private static function onScriptLoaded(event:Event):void
        {
            try
            {
                var obj:* = LoaderInfo(event.target).loader.content;
                obj.run(instance);
            }
            catch (ex:Error)
            {
                instance.external.debug("Error while running injection: " + ex);
            }
        }
        
		public static function catchPackets():void
        {
            instance.game.sfc.addEventListener(SFSEvent.onDebugMessage, packetReceived);
        }
		
        public static function sendClientPacket(packet:String, type:String):void
        {
            if (_handler == null)
            {
                var cls:Class = Class(instance.gameDomain.getDefinition("it.gotoandplay.smartfoxserver.handlers.ExtHandler"));
                _handler = new cls(instance.game.sfc);
            }
            switch (type)
			{
			  case "xml":
				xmlReceived(packet);
				break;
			  case "json":
				jsonReceived(packet);
				break;
			  case "str":
				strReceived(packet);
				break;
			  default:
				instance.external.debug("Invalid packet type.");
			};
        }
        
        public static function xmlReceived(packet:String):void
        {
            _handler.handleMessage(new XML(packet), "xml");
        }
        
        public static function jsonReceived(packet:String):void
        {
            _handler.handleMessage(JSON.parse(packet)["b"], "json");
        }
        
        public static function strReceived(packet:String):void
        {
            var array:Array = packet.substr(1, packet.length - 2).split("%");
            _handler.handleMessage(array.splice(1, array.length - 1), "str");
        }
        
        public static function packetReceived(packet:*):void
        {
            if (packet.params.message.indexOf("%xt%zm%") > -1)
            {
                instance.external.call("packet", packet.params.message.split(":", 2)[1].trim());
            }
        }
		
        public static function disableDeathAd(enable:Boolean):void
        {
            instance.game.userPreference.data.bDeathAd = !enable;
        }
		
        public static function UserID():int
        {
            return instance.game.world.myAvatar.uid;
        }
		
        public static function Gender():String
        {
            return '"' + instance.game.world.myAvatar.objData.strGender.toUpperCase() + '"';
        }
    }
}	
