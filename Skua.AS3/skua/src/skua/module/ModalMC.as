package skua.module
{
	import flash.display.MovieClip;
	import flash.events.MouseEvent;
	import flash.filters.GlowFilter;
	
	public class ModalMC extends MovieClip
	{
		
		public var cnt:MovieClip;
		
		private var world:MovieClip;
		
		private var rootClass:MovieClip;
		
		private var fData:Object = null;
		
		private var callback:Object = null;
		
		private var mc:MovieClip;
		
		private var qtySel:QtySelectorMC;
		
		private var heightOffset:int = 42;
		
		private var glowColors:Object;
		
		private var glowSizes:Object;
		
		public var isOpen:Boolean = false;
		
		public var greedy:Boolean = false;
		
		public function ModalMC()
		{
			this.glowColors = {"white": 16777215, "red": 16711680, "green": 65280, "blue": 255, "gold": 16750899};
			this.glowSizes = {"medium": 3};
			super();
			addFrameScript(3, this.frame4, 11, this.frame12);
		}
		
		public function init(param1:*):*
		{
			var _loc2_:String = null;
			var _loc3_:String = null;
			this.fData = param1;
			this.callback = param1.callback;
			if (param1.greedy != null)
			{
				this.greedy = param1.greedy;
			}
			this.mc = MovieClip(this);
			this.rootClass = MovieClip(stage.getChildAt(0));
			this.mc.cnt.strBody.autoSize = "center";
			this.mc.cnt.strBody.htmlText = this.fData.strBody;
			if (param1.btns == null || param1.btns == "dual")
			{
				this.mc.cnt.btns.dual.ybtn.buttonMode = true;
				this.mc.cnt.btns.dual.nbtn.buttonMode = true;
				this.mc.cnt.btns.dual.ybtn.addEventListener(MouseEvent.CLICK, this.yClick, false, 0, true);
				this.mc.cnt.btns.dual.ybtn.addEventListener(MouseEvent.MOUSE_OVER, this.yMouseOver, false, 0, true);
				this.mc.cnt.btns.dual.ybtn.addEventListener(MouseEvent.MOUSE_OUT, this.yMouseOut, false, 0, true);
				this.mc.cnt.btns.dual.nbtn.addEventListener(MouseEvent.CLICK, this.nClick, false, 0, true);
				this.mc.cnt.btns.dual.nbtn.addEventListener(MouseEvent.MOUSE_OVER, this.nMouseOver, false, 0, true);
				this.mc.cnt.btns.dual.nbtn.addEventListener(MouseEvent.MOUSE_OUT, this.nMouseOut, false, 0, true);
				this.mc.cnt.btns.mono.visible = false;
			}
			if (param1.btns != null && param1.btns == "mono")
			{
				this.mc.cnt.btns.mono.buttonMode = true;
				this.mc.cnt.btns.mono.addEventListener(MouseEvent.CLICK, this.mClick, false, 0, true);
				this.mc.cnt.btns.mono.addEventListener(MouseEvent.MOUSE_OVER, this.yMouseOver, false, 0, true);
				this.mc.cnt.btns.mono.addEventListener(MouseEvent.MOUSE_OUT, this.yMouseOut, false, 0, true);
				this.mc.cnt.btns.dual.visible = false;
			}
			if (param1.qtySel != null && param1.qtySel.max > 1)
			{
				this.qtySel = QtySelectorMC(addChild(new QtySelectorMC(this, this.rootClass, param1.qtySel.min, param1.qtySel.max, int(param1.qtySel.base) || -1)));
				this.qtySel.y = Math.round(this.mc.cnt.strBody.y + this.mc.cnt.strBody.textHeight + 10);
				this.qtySel.x = Math.round(this.mc.width / 2 - this.qtySel.width / 2);
				this.heightOffset = this.heightOffset + this.qtySel.height + 16;
			}
			this.mc.cnt.bg.height = Math.ceil(this.mc.cnt.strBody.textHeight + this.heightOffset);
			this.mc.cnt.btns.y = this.mc.cnt.bg.height;
			this.mc.x = 960 / 2 - this.mc.width / 2;
			this.mc.y = 550 / 2 - this.mc.height / 2;
			if (param1.glow != null)
			{
				_loc2_ = param1.glow.split(",")[0];
				_loc3_ = param1.glow.split(",")[1];
				this.mc.filters = [new GlowFilter(this.glowColors[_loc2_], 1, this.glowSizes[_loc3_], this.glowSizes[_loc3_], 64, 1)];
			}
		}
		
		public function fClose():void
		{
			this.callback = null;
			if (this.qtySel != null)
			{
				this.qtySel.fClose();
			}
			this.killButtons();
			this.mc.parent.removeChild(this);
		}
		
		private function yClick(param1:MouseEvent):*
		{
			var _loc2_:* = MovieClip(param1.currentTarget);
			var _loc3_:* = MovieClip(this);
			this.setCT(_loc2_.bg, 43775);
			this.fData.params.accept = true;
			_loc3_.mouseChildren = false;
			if (this.qtySel != null)
			{
				_loc3_.fData.params.iQty = this.qtySel.val;
			}
			_loc3_.callback(_loc3_.fData.params);
			this.killButtons();
			_loc3_.gotoAndPlay("out");
		}
		
		private function nClick(param1:MouseEvent):*
		{
			var _loc2_:* = MovieClip(param1.currentTarget);
			var _loc3_:* = MovieClip(this);
			this.setCT(_loc2_.bg, 16711680);
			this.fData.params.accept = false;
			_loc3_.mouseChildren = false;
			_loc3_.callback(this.fData.params);
			this.killButtons();
			_loc3_.gotoAndPlay("out");
		}
		
		private function mClick(param1:MouseEvent):*
		{
			var _loc2_:* = MovieClip(param1.currentTarget);
			var _loc3_:* = MovieClip(this);
			this.setCT(_loc2_.bg, 16711680);
			_loc3_.mouseChildren = false;
			if (_loc3_.callback != null)
			{
				_loc3_.callback(this.fData.params);
			}
			this.killButtons();
			_loc3_.gotoAndPlay("out");
		}
		
		private function yMouseOver(param1:MouseEvent):*
		{
			var _loc2_:* = MovieClip(param1.currentTarget);
			this.setCT(_loc2_.bg, 2236962);
		}
		
		private function yMouseOut(param1:MouseEvent):*
		{
			var _loc2_:* = MovieClip(param1.currentTarget);
			this.setCT(_loc2_.bg, 0);
		}
		
		private function nMouseOver(param1:MouseEvent):*
		{
			var _loc2_:* = MovieClip(param1.currentTarget);
			this.setCT(_loc2_.bg, 2236962);
		}
		
		private function nMouseOut(param1:MouseEvent):*
		{
			var _loc2_:* = MovieClip(param1.currentTarget);
			this.setCT(_loc2_.bg, 0);
		}
		
		private function killButtons():void
		{
			this.mc.cnt.btns.dual.ybtn.removeEventListener(MouseEvent.CLICK, this.yClick);
			this.mc.cnt.btns.dual.ybtn.removeEventListener(MouseEvent.MOUSE_OVER, this.yMouseOver);
			this.mc.cnt.btns.dual.ybtn.removeEventListener(MouseEvent.MOUSE_OUT, this.yMouseOut);
			this.mc.cnt.btns.dual.nbtn.removeEventListener(MouseEvent.CLICK, this.nClick);
			this.mc.cnt.btns.dual.nbtn.removeEventListener(MouseEvent.MOUSE_OVER, this.nMouseOver);
			this.mc.cnt.btns.dual.nbtn.removeEventListener(MouseEvent.MOUSE_OUT, this.nMouseOut);
			this.mc.cnt.btns.mono.removeEventListener(MouseEvent.CLICK, this.mClick);
			this.mc.cnt.btns.mono.removeEventListener(MouseEvent.MOUSE_OVER, this.yMouseOver);
			this.mc.cnt.btns.mono.removeEventListener(MouseEvent.MOUSE_OUT, this.yMouseOut);
			if (this.qtySel != null)
			{
				this.qtySel.killButtons();
			}
		}
		
		private function setCT(param1:*, param2:*):*
		{
			var _loc3_:* = param1.transform.colorTransform;
			_loc3_.color = param2;
			param1.transform.colorTransform = _loc3_;
		}
		
		internal function frame4():*
		{
			stop();
		}
		
		internal function frame12():*
		{
			if (this.stage != null)
			{
				this.fClose();
			}
		}
	}
}