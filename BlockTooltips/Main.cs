using System;
using Raptor;
using Raptor.Api;
using Raptor.Api.Commands;
using Raptor.Api.Hooks;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;

namespace BlockTooltips
{
	[ApiVersion(1,0)]
	public class BlockTooltips : TerrariaPlugin
	{				
		public override string Author 
		{
			get { return "Ijwu"; }
		}
		
		public override string Description 
		{
			get { return "Inspect stuff."; }
		}
		
		public override string Name 
		{
			get { return "Block Tooltips"; }
		}
		
		public override Version Version 
		{
			get { return Assembly.GetExecutingAssembly().GetName().Version; }
		}
		
		public override void Initialize()
		{
			Commands.Register(new Command(ToggleTooltips, "tooltip") 
			{
				HelpText = new[]
				{
					"Toggles tile highlight tooltips.",
					"Tooltips appear near the bottom of the screen and depict the tile image and name."
              	}
			});
			                  					
			GameHooks.Draw["Interface"] += OnDrawInterface;
		}
		
		private bool tooltipsEnabled = true;

		public void OnDrawInterface(object o, GameHooks.DrawEventArgs e)
		{
			Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
			
			string tileName = ((TileTypes)tile.type).ToString();
			tileName = Regex.Replace(tileName, "([a-z])([A-Z])", "$1 $2");

			int textWidth = (int)Main.fontMouseText.MeasureString(tileName).X;
				
			int targetX = Main.screenWidth - (int)(Main.screenWidth * .5);
			int targetY = Main.screenHeight - 10;
			
			int width = 128;
			int height = 48;
			
			int difference = 0;
			
			if (textWidth > width-34)
			{
				difference = Math.Abs((width-34)-textWidth);
				width += difference+16;
			}
			else
			{
				width = textWidth+50;
			}
			
			targetX = targetX - width/2;
			targetX += difference/2;
			targetY = targetY - height;
			if (tooltipsEnabled && tile.active())
			{
				e.SpriteBatch.DrawGuiRectangle(new Rectangle(targetX, targetY, width, height), new Color(100, 100, 100, 200));
				if (Main.tileTexture[tile.type] != null)
					e.SpriteBatch.Draw(Main.tileTexture[tile.type], new Vector2(targetX + 16, targetY + 16), new Rectangle?(new Rectangle(tile.frameX, tile.frameY, 16, 16)), Color.White);
				e.SpriteBatch.DrawGuiText(tileName, new Vector2(targetX+34, targetY+16), Color.White, Main.fontMouseText);
			}
		}
		
		public void ToggleTooltips(object o, CommandEventArgs e)
		{
			tooltipsEnabled = !tooltipsEnabled;
			Raptor.Utils.NewSuccessText("Tooltips have been " + (tooltipsEnabled ? "Enabled" : "Disabled"));
		}
		
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			
			if (disposing)
			{
				GameHooks.Draw["Interface"] -= OnDrawInterface;
			}
		}
	}
}