package skua
{
	import flash.events.KeyboardEvent;
	import flash.text.TextField;
	import skua.Externalizer;
	import skua.ExtractedFuncs;
	import adobe.utils.ProductManager;
	import flash.display.Loader;
	import flash.display.LoaderInfo;
	import flash.display.MovieClip;
	import flash.display.Stage;
	import flash.display.StageScaleMode;
	import flash.display.StageAlign;
	import flash.display.DisplayObject;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.events.ProgressEvent;
	import flash.net.URLLoader;
	import flash.net.URLRequest;
	import flash.system.ApplicationDomain;
	import flash.system.LoaderContext;
	import flash.system.Security;
	import flash.utils.getQualifiedClassName;
	import flash.utils.setTimeout;
	import flash.utils.clearTimeout;
	import flash.utils.Timer;
	import flash.events.TimerEvent;
	import skua.util.SFSEvent;
	import flash.utils.getQualifiedClassName;
	import skua.module.Modules;
	import skua.module.ModalMC;
	
	public class Main extends MovieClip
	{
		public static var instance:Main;
		
		private static var _gameClass:Class;
		private static var _fxStore:* = new Object();
		private static var _fxLastOpt:Boolean = false;
		private static var _handler:*;
		
		private var game:*;
		private var external:skua.Externalizer;
		
		private var sURL:String = 'https://game.aq.com/game/';
		private var versionUrl:String = (sURL + 'api/data/gameversion');
		private var loginURL:String = (sURL + 'api/login/now');
		
		private var sFile:String;
		private var sBG:String = 'Generic2.swf';
		private var isEU:Boolean;
		private var urlLoader:URLLoader;
		private var loader:Loader;
		private var vars:Object;
		private var sTitle:String = '<font color="#FDAF2D">Better Performance</font>';
		
		private var stg:Stage;
		private var gameDomain:ApplicationDomain;
		
		public function Main()
		{
			String.prototype.trim = function():String
			{
				return this.replace(/^\s+|\s+$/g, '');
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
			Security.allowDomain('*');
			this.urlLoader = new URLLoader();
			this.urlLoader.addEventListener(Event.COMPLETE, this.onDataComplete);
			this.urlLoader.load(new URLRequest(this.versionUrl));
		}
		
		private function onDataComplete(event:Event):void
		{
			this.urlLoader.removeEventListener(Event.COMPLETE, this.onDataComplete);
			this.vars = JSON.parse(event.target.data);
			this.sFile = ((this.vars.sFile + '?ver=') + Math.random());
			this.loadGame()
		}
		
		private function loadGame():void
		{
			this.loader = new Loader();
			this.loader.contentLoaderInfo.addEventListener(Event.COMPLETE, this.onComplete);
			this.loader.load(new URLRequest(this.sURL + 'gamefiles/' + this.sFile));
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
			
			this.game.stage.addEventListener(KeyboardEvent.KEY_DOWN, this.key_StageGame);
			
			this.external.call('loaded');
		}
		
		public function onExtensionResponse(packet:*):void
		{
			this.external.call('pext', JSON.stringify(packet));
		}
		
		public function key_StageGame(kbArgs:KeyboardEvent):void
		{
			if (!(kbArgs.target is TextField || kbArgs.currentTarget is TextField))
			{
				if (kbArgs.keyCode == this.game.litePreference.data.keys['Bank'])
				{
					if (this.game.stage.focus == null || (this.game.stage.focus != null && !('text' in this.game.stage.focus)))
					{
						this.game.world.toggleBank();
					}
				}
			}
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
		
		public static function jumpCorrectRoom(cell:String, pad:String, autoCorrect:Boolean = true, clientOnly:Boolean = false):void
		{
			var prevCell:String = instance.game.world.strFrame;
			if (!autoCorrect)
			{
				instance.game.world.moveToCell(cell, pad, clientOnly);
				return;
			}
			else
			{
				var users:Array = instance.game.world.areaUsers;
				users.splice(users.indexOf(instance.game.sfc.myUserName), 1);
				users.sort();
				if (users.length <= 1)
				{
					instance.game.world.moveToCell(cell, pad, clientOnly);
				}
				else
				{
					var usersCell:String = instance.game.world.strFrame;
					var usersPad:String = "Left";
					for (var i:int = 0; i < users.length; i++)
					{
						usersCell = instance.game.world.uoTree[users[i]].strFrame;
						usersPad = instance.game.world.uoTree[users[i]].strPad;
						if (cell == usersCell && pad != usersPad)
							break;
					}
					instance.game.world.moveToCell(cell, usersPad, clientOnly);
				}
				
				var jumpTimer:Timer = new Timer(50, 1);
				jumpTimer.addEventListener(TimerEvent.TIMER, jumpTimerEvent);
				jumpTimer.start();
				
				function jumpTimerEvent(e:TimerEvent):void
				{
					jumpCorrectPad(cell, clientOnly);
					jumpTimer.stop();
					jumpTimer.removeEventListener(TimerEvent.TIMER, jumpTimerEvent);
				}
			}
		}
		
		public static function jumpCorrectPad(cell:String, clientOnly:Boolean = false):void
		{
			var cellPad:String = 'Left';
			var padArr:Array = getCellPads();
			if (padArr.indexOf(cellPad) >= 0)
			{
				if (instance.game.world.strPad === cellPad)
					return;
				instance.game.world.moveToCell(cell, cellPad, clientOnly);
			}
			else
			{
				cellPad = padArr[0];
				if (instance.game.world.strPad === cellPad)
					return;
				instance.game.world.moveToCell(cell, cellPad, clientOnly);
			}
		}
		
		public static function getCellPads():Array
		{
			var cellPads:Array = new Array();
			var padNames:RegExp = /(Spawn|Center|Left|Right|Up|Down|Top|Bottom)/;
			var cellPadsCnt:int = instance.game.world.map.numChildren;
			for (var i:int = 0; i < cellPadsCnt; ++i)
			{
				var child:DisplayObject = instance.game.world.map.getChildAt(i);
				if (padNames.test(child.name))
				{
					cellPads.push(child.name);
				}
			}
			return cellPads;
		}
		
		private static function getProperties(obj:*):String
		{
			var p:*;
			var res:String = '';
			var val:String;
			var prop:String;
			for (p in obj)
			{
				prop = String(p);
				if (prop && prop !== '' && prop !== ' ')
				{
					val = String(obj[p]);
					res += prop + ': ' + val + ', ';
				}
			}
			res = res.substr(0, res.length - 2);
			return res;
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
			var parts:Array = path.split('.');
			var varName:String = parts.pop();
			var obj:* = _getObjectA(instance.game, parts);
			obj[varName] = value;
		}
		
		public static function setGameObjectKey(path:String, key:String, value:*):void
		{
			var parts:Array = path.split('.');
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
			var parts:Array = path.split('.');
			var funcName:String = parts.pop();
			var obj:* = _getObjectA(instance.game, parts);
			var func:Function = obj[funcName] as Function;
			return JSON.stringify(func.apply(null, args));
		}
		
		public static function callGameFunction0(path:String):String
		{
			var parts:Array = path.split('.');
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
				instance.external.debug('selectArrayObjects target is not an array');
				return '';
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
			return _getObjectA(root, path.split('.'));
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
			return 'true';
		}
		
		private static function killLoginModals():void
		{
			var loc2_:MovieClip = null;
			var loc1_:MovieClip = instance.game.mcLogin.ModalStack;
			var loc3_:int = 0;
			while (loc3_ < loc1_.numChildren)
			{
				loc2_ = loc1_.getChildAt(loc3_) as MovieClip;
				if ("fClose" in loc2_)
				{
					loc2_.fClose();
				}
				loc3_++;
			}
		}
		
		public static function connectToServer(server:String):String
		{
			var serverData:Object = JSON.parse(server);
			var objLogin:Object = null;
			
			var connectionServerTimer:Timer = new Timer(500, 50);
			connectionServerTimer.addEventListener(TimerEvent.TIMER, connectingServer);
			connectionServerTimer.start();
			
			function connectingServer(e:Event):void
			{
				if (objLogin != null)
				{
					connectServer(serverData, objLogin);
					connectionServerTimer.stop();
					connectionServerTimer.removeEventListener(TimerEvent.TIMER, connectingServer);
				}
				objLogin = JSON.parse(getGameObjectS("objLogin"));
			}
			
			return true.toString();
		}
		
		private static function connectServer(server:Object, objLoginData:Object):*
		{
			var _loc2_:ModalMC = null;
			var _loc3_:Object = null;
			var _loc4_:* = undefined;
			instance.game.showTracking("4");
			if (!instance.game.serialCmdMode)
			{
				if ((_loc4_ = server).bOnline == 0)
				{
					instance.game.MsgBox.notify("Server currently offline!");
				}
				else if (_loc4_.iCount >= _loc4_.iMax)
				{
					instance.game.MsgBox.notify("Server is Full!");
				}
				else if (_loc4_.iChat > 0 && objLoginData.bCCOnly == 1)
				{
					instance.game.MsgBox.notify("Account Restricted to Moglin Sage Server Only.");
				}
				else if (_loc4_.iChat > 0 && objLoginData.iAge < 13 && objLoginData.iUpgDays < 0)
				{
					instance.game.MsgBox.notify("Ask your parent to upgrade your account in order to play on chat enabled servers.");
				}
				else if (_loc4_.bUpg == 1 && objLoginData.iUpgDays < 0)
				{
					_loc2_ = new ModalMC();
					_loc3_ = {};
					_loc3_.strBody = "Member Server! Do you want to upgrade your account to access this premium server now?";
					_loc3_.params = {};
					_loc3_.glow = "white,medium";
					_loc3_.btns = "dual";
					instance.game.mcLogin.ModalStack.addChild(_loc2_);
					_loc2_.init(_loc3_);
				}
				else if (Number(_loc4_.iMax) % 2 > 0)
				{
					_loc2_ = new ModalMC();
					_loc3_ = {};
					_loc3_.strBody = "Testing Server! Do you want to switch to the testing game client?";
					_loc3_.params = {};
					_loc3_.glow = "white,medium";
					_loc3_.btns = "dual";
					instance.game.mcLogin.ModalStack.addChild(_loc2_);
					_loc2_.init(_loc3_);
				}
				else if (_loc4_.iLevel > 0 && objLoginData.iEmailStatus <= 2)
				{
					_loc2_ = new ModalMC();
					_loc3_ = {};
					_loc3_.strBody = "This server requires a confirmed email address.";
					_loc3_.params = {};
					_loc3_.glow = "red,medium";
					_loc3_.btns = "mono";
					instance.game.mcLogin.ModalStack.addChild(_loc2_);
					_loc2_.init(_loc3_);
				}
				else
				{
					instance.game.objServerInfo = _loc4_;
					instance.game.chatF.iChat = _loc4_.iChat;
					killLoginModals();
					instance.game.connectTo(_loc4_.sIP, _loc4_.iPort);
				}
			}
		}
		
		public static function isTrue():String
		{
			return true.toString();
		}
		
		public static function auraComparison(target:String, operator:String, auraName:String, auraValue:int):String
		{
			var aura:Object = null;
			var auras:Object = null;
			try
			{
				auras = target == 'Self' ? instance.game.world.myAvatar.dataLeaf.auras : instance.game.world.myAvatar.target.dataLeaf.auras;
			}
			catch (e:Error)
			{
				return false.toString();
			}
			
			for (var i:int = 0; i < auras.length; i++)
			{
				aura = auras[i];
				if (aura.nam.toLowerCase() == auraName.toLowerCase())
				{
					var actualValue:int = (aura.val == undefined || aura.val == null) ? 1 : parseInt(aura.val);
					if (operator == 'Greater' && actualValue > auraValue)
					{
						return true.toString();
					}
					if (operator == 'Less' && actualValue < auraValue)
					{
						return true.toString();
					}
					if (operator == 'Equal' && actualValue == auraValue)
					{
						return true.toString();
					}
					if (operator == 'GreaterOrEqual' && actualValue >= auraValue)
					{
						return true.toString();
					}
					if (operator == 'LessOrEqual' && actualValue <= auraValue)
					{
						return true.toString();
					}
				}
			}
			return false.toString();
		}
		
		public static function getSubjectAuras(subject:String):String
		{
			var aura:Object = null;
			var auras:Object = null;
			try
			{
				auras = subject == 'Self' ? instance.game.world.myAvatar.dataLeaf.auras : instance.game.world.myAvatar.target.dataLeaf.auras;
			}
			catch (e:Error)
			{
				return '[]';
			}
			
			var auraArray:Array = new Array();
			for (var i:int = 0; i < auras.length; i++)
			{
				aura = auras[i];
				auraArray.push({'name': aura.nam, 'value': aura.val == undefined ? 0 : aura.val, 'passive': aura.passive, 'timeStamp': aura.ts, 'duration': parseInt(aura.dur), 'potionType': aura.potionType, 'cat': aura.cat, 't': aura.t, 's': aura.s, 'fx': aura.fx, 'animOn': aura.animOn, 'animOff': aura.animOff, 'msgOn': aura.msgOn, 'isNew': aura.isNew});
			}
			return JSON.stringify(auraArray);
		}
		
		public static function GetAurasValue(subject:String, auraName:String):String
		{
			var aura:Object = null;
			var auras:Object = null;
			try
			{
				auras = subject == 'Self' ? instance.game.world.myAvatar.dataLeaf.auras : instance.game.world.myAvatar.target.dataLeaf.auras;
			}
			catch (e:Error)
			{
				return '0';
			}
			
			for (var i:int = 0; i < auras.length; i++)
			{
				aura = auras[i];
				if (aura.nam.toLowerCase() == auraName.toLowerCase())
				{
					return (aura.val == undefined || aura.val == null ? 1 : aura.val).toString();
				}
			}
			return '0';
		}
		
		public static function HasAnyActiveAura(subject:String, auraNames:String):String
		{
			var auraList:Array = auraNames.split(',');
			var auras:Object = null;
			try
			{
				auras = subject == 'Self' ? instance.game.world.myAvatar.dataLeaf.auras : instance.game.world.myAvatar.target.dataLeaf.auras;
			}
			catch (e:Error)
			{
				return false.toString();
			}
			
			for (var i:int = 0; i < auras.length; i++)
			{
				var aura:Object = auras[i];
				for (var j:int = 0; j < auraList.length; j++)
				{
					if (aura.nam.toLowerCase() == auraList[j].toLowerCase().trim())
					{
						return true.toString();
					}
				}
			}
			return false.toString();
		}
		
		public static function HasAllActiveAuras(subject:String, auraNames:String):String
		{
			var auraList:Array = auraNames.split(',');
			var auras:Object = null;
			try
			{
				auras = subject == 'Self' ? instance.game.world.myAvatar.dataLeaf.auras : instance.game.world.myAvatar.target.dataLeaf.auras;
			}
			catch (e:Error)
			{
				return false.toString();
			}
			
			var foundCount:int = 0;
			for (var i:int = 0; i < auraList.length; i++)
			{
				for (var j:int = 0; j < auras.length; j++)
				{
					if (auras[j].nam.toLowerCase() == auraList[i].toLowerCase().trim())
					{
						foundCount++;
						break;
					}
				}
			}
			return (foundCount == auraList.length).toString();
		}
		
		public static function GetTotalAuraStacks(subject:String, auraNamePattern:String):String
		{
			var auras:Object = null;
			try
			{
				auras = subject == 'Self' ? instance.game.world.myAvatar.dataLeaf.auras : instance.game.world.myAvatar.target.dataLeaf.auras;
			}
			catch (e:Error)
			{
				return '0';
			}
			
			var totalStacks:int = 0;
			var pattern:String = auraNamePattern.toLowerCase();
			
			for (var i:int = 0; i < auras.length; i++)
			{
				var aura:Object = auras[i];
				var auraName:String = aura.nam.toLowerCase();
				
				if (auraName == pattern || auraName.indexOf(pattern) != -1)
				{
					var stacks:int = (aura.val == undefined || aura.val == null) ? 1 : parseInt(aura.val);
					totalStacks += stacks;
				}
			}
			return totalStacks.toString();
		}
		
		public static function GetAuraSecondsRemaining(subject:String, auraName:String):String
		{
			var aura:Object = null;
			var auras:Object = null;
			try
			{
				auras = subject == 'Self' ? instance.game.world.myAvatar.dataLeaf.auras : instance.game.world.myAvatar.target.dataLeaf.auras;
			}
			catch (e:Error)
			{
				return '0';
			}
			
			for (var i:int = 0; i < auras.length; i++)
			{
				aura = auras[i];
				if (aura.nam.toLowerCase() == auraName.toLowerCase())
				{
					var currentTime:Number = new Date().getTime();
					var auraTime:Number = parseFloat(aura.ts);
					var duration:Number = parseFloat(aura.dur) * 1000; // Convert to milliseconds
					var expiresAt:Number = auraTime + duration;
					var remaining:Number = Math.max(0, (expiresAt - currentTime) / 1000); // Convert to seconds
					return Math.floor(remaining).toString();
				}
			}
			return '0';
		}
		
		public static function getAvatar(id:int):String
		{
			return JSON.stringify(instance.game.world.avatars[id].objData);
		}
		
		public static function clickServer(serverName:String):String
		{
			var source:* = instance.game.mcLogin.sl.iList;
			for (var i:int = 0; i < source.numChildren; i++)
			{
				var child:* = source.getChildAt(i);
				
				if (child.tName.ti.text.toLowerCase().indexOf(serverName.toLowerCase()) > -1)
				{
					child.dispatchEvent(new MouseEvent(MouseEvent.CLICK));
					return true.toString();
				}
			}
			return false.toString();
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
			instance.game.world.moveRequest({'mc': instance.game.world.myAvatar.pMC, 'tx': xPos, 'ty': yPos, 'sp': walkSpeed});
		}
		
		public static function untargetSelf():void
		{
			var target:* = instance.game.world.myAvatar.target;
			if (target && target == instance.game.world.myAvatar)
			{
				instance.game.world.cancelTarget();
			}
		}
		
		public static function attackMonsterByID(id:int):String
		{
			var bestTarget:* = getBestMonsterTargetByID(id);
			return attackTarget(bestTarget);
		}
			
		public static function attackMonsterByName(name:String):String
		{
			var bestTarget:* = getBestMonsterTarget(name);
			return attackTarget(bestTarget);
		}
		
		public static function attackPlayer(name:String):String
		{
			var player:* = instance.game.world.getAvatarByUserName(name.toLowerCase());
			return attackTarget(player);
		}
		
		public static function getMonster(name:String):*
		{
			for each (var monster:* in instance.game.world.getMonstersByCell(instance.game.world.strFrame))
			{
				var monName:String = monster.objData.strMonName.toLowerCase();
				if ((monName.indexOf(name.toLowerCase()) > -1 || name == '*') && monster.pMC != null)
				{
					return monster;
				}
			}
		return null;
		}
	
		public static function getBestMonsterTarget(name:String):*
		{
			var targetCandidates:Array = [];
			
			for each (var monster:* in instance.game.world.getMonstersByCell(instance.game.world.strFrame))
			{
				var monName:String = monster.objData.strMonName.toLowerCase();
				if ((monName.indexOf(name.toLowerCase()) > -1 || name == '*') && monster.pMC != null)
				{
					targetCandidates.push(monster);
				}
			}
			
			if (targetCandidates.length == 0)
				return null;
			
			targetCandidates.sort(function(a:*, b:*):Number {
				var aHP:int = (a.dataLeaf && a.dataLeaf.intHP) ? a.dataLeaf.intHP : 0;
				var bHP:int = (b.dataLeaf && b.dataLeaf.intHP) ? b.dataLeaf.intHP : 0;
				
				var aAlive:Boolean = aHP > 0;
				var bAlive:Boolean = bHP > 0;
				
				if (aAlive != bAlive) {
					return aAlive ? -1 : 1;
				}
				
				if (aHP != bHP) {
					return aHP - bHP; 
				}
				
				var aMapID:int = a.objData ? a.objData.MonMapID : 0;
				var bMapID:int = b.objData ? b.objData.MonMapID : 0;
				return aMapID - bMapID;
			});
			return targetCandidates[0];
		}
	
		public static function getBestMonsterTargetByID(id:int):*
		{
			var targetCandidates:Array = [];
			
			for each (var monster:* in instance.game.world.getMonstersByCell(instance.game.world.strFrame))
			{
				if (monster.pMC != null && monster.objData && (monster.objData.MonMapID == id || monster.objData.MonID == id))
				{
					targetCandidates.push(monster);
				}
			}
			
			if (targetCandidates.length == 0)
				return null;
			
			targetCandidates.sort(function(a:*, b:*):Number {
				var aHP:int = (a.dataLeaf && a.dataLeaf.intHP) ? a.dataLeaf.intHP : 0;
				var bHP:int = (b.dataLeaf && b.dataLeaf.intHP) ? b.dataLeaf.intHP : 0;
				
				var aAlive:Boolean = aHP > 0;
				var bAlive:Boolean = bHP > 0;
				
				if (aAlive != bAlive) {
					return aAlive ? -1 : 1;
				}
				
				if (aHP != bHP) {
					return aHP - bHP;
				}
				
				var aMapID:int = a.objData ? a.objData.MonMapID : 0;
				var bMapID:int = b.objData ? b.objData.MonMapID : 0;
				return aMapID - bMapID;
			});
			return targetCandidates[0];
		}
		
		public static function getMonsterHealth(param1:String): String
		{
			var curMonster:Object = getMonster(param1);
			if (curMonster && curMonster.dataLeaf)
			{
				return curMonster.dataLeaf.intHP.toString();
			}
			return "0";
		}
		
		public static function getMonsterHealthById(param1:int): String
		{
			var curMonster:Object = instance.game.world.getMonster(param1);
			if (curMonster && curMonster.dataLeaf)
			{
				return curMonster.dataLeaf.intHP.toString();
			}
			return "0";
		}
		
		public static function availableMonstersInCell():String
		{
			var retMonsters:Array = [];
			for each (var monster:* in instance.game.world.getMonstersByCell(instance.game.world.strFrame))
			{
				if (monster.pMC != null)
				{
					var monsterData:Object = new Object();
					for (var prop:String in monster.objData)
					{
						monsterData[prop] = monster.objData[prop];
					}
					if (monster.dataLeaf)
					{
						monsterData.intHP = monster.dataLeaf.intHP;
						monsterData.intHPMax = monster.dataLeaf.intHPMax;
						monsterData.intState = monster.dataLeaf.intState;
						monsterData.auras = monster.dataLeaf.auras
					}
					retMonsters.push(monsterData);
				}
			}
			return JSON.stringify(retMonsters);
		}
		
		public function requestDoomArenaPVPQueue():void
		{
			instance.game.world.rootClass.sfc.sendXtMessage("zm", "PVPQr", ["doomarena", 0], "str", instance.game.world.rootClass.world.curRoom);
		}
		
		private static function attackTarget(target:*):String
		{
			if (target != null && target.pMC != null)
			{
				instance.game.world.setTarget(target);
				instance.game.world.approachTarget();
				return true.toString();
			}
			return false.toString();
		}
		
		public static function useSkill(index:int):String
		{
			var skill:* = instance.game.world.actions.active[index];
			if (skua.ExtractedFuncs.actionTimeCheck(skill))
			{
				instance.game.world.testAction(skill);
				return true.toString();
			}
			
			return false.toString();
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
		
		public static function buyItemByName(name:String, quantity:int = -1):void
		{
			for each (var item:* in instance.game.world.shopinfo.items)
			{
				if (item.sName.toLowerCase() == name.toLowerCase())
				{
					if (quantity == -1)
						instance.game.world.sendBuyItemRequest(item);
					else
					{
						var buyItem:* = new Object();
						buyItem = item;
						buyItem.iSel = item;
						buyItem.iQty = quantity;
						buyItem.iSel.iQty = quantity;
						buyItem.accept = 1;
						instance.game.world.sendBuyItemRequestWithQuantity(buyItem);
					}
					break;
				}
			}
		}
		
		public static function buyItemByID(id:int, shopItemID:int, quantity:int = -1):void
		{
			for each (var item:* in instance.game.world.shopinfo.items)
			{
				if (item.ItemID == id && (shopItemID == 0 || item.ShopItemID == shopItemID))
				{
					if (quantity == -1)
						instance.game.world.sendBuyItemRequest(item);
					else
					{
						var buyItem:* = new Object();
						buyItem = item;
						buyItem.iSel = item;
						buyItem.iQty = quantity;
						buyItem.iSel.iQty = quantity;
						buyItem.accept = 1;
						instance.game.world.sendBuyItemRequestWithQuantity(buyItem);
					}
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
			var pickup:Array = whitelist.split(',');
			if (instance.game.litePreference.data.bCustomDrops)
			{
				var source:* = instance.game.cDropsUI.mcDraggable ? instance.game.cDropsUI.mcDraggable.menu : instance.game.cDropsUI;
				for (var i:int = 0; i < source.numChildren; i++)
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
					if (type.indexOf('DFrame2MC') != -1)
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
				instance.external.debug('Error while running injection: ' + ex);
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
				var cls:Class = Class(instance.gameDomain.getDefinition('it.gotoandplay.smartfoxserver.handlers.ExtHandler'));
				_handler = new cls(instance.game.sfc);
			}
			switch (type)
			{
			case 'xml': 
				xmlReceived(packet);
				break;
			case 'json': 
				jsonReceived(packet);
				break;
			case 'str': 
				strReceived(packet);
				break;
			default: 
				instance.external.debug('Invalid packet type.');
			}
			;
		}
		
		public static function xmlReceived(packet:String):void
		{
			_handler.handleMessage(new XML(packet), 'xml');
		}
		
		public static function jsonReceived(packet:String):void
		{
			_handler.handleMessage(JSON.parse(packet)['b'], 'json');
		}
		
		public static function strReceived(packet:String):void
		{
			var array:Array = packet.substr(1, packet.length - 2).split('%');
			_handler.handleMessage(array.splice(1, array.length - 1), 'str');
		}
		
		public static function packetReceived(packet:*):void
		{
			if (packet.params.message.indexOf('%xt%zm%') > -1)
			{
				instance.external.call('packet', packet.params.message.split(':', 2)[1].trim());
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
