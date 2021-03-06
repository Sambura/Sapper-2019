﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Stuff;

namespace Sapper_2019
{
	public partial class MainForm : Form
	{
		private Dictionary<string, NewControl> controls;
		private Dictionary<string, Bitmap> textures;
		private Dictionary<string, Color> colorTheme;
		private Dictionary<string, int> parameters;
		private SortedSet<Coords> toDraw;

		private Bitmap bitmap;
		private Graphics graphics;

		private int controlBoxHeight;
		private int minWidth;
		private int minHeight;

		private Point cursor;
		private bool leftMouse, rightMouse;
		private Point grip;
		private NewControl hovered;
		private bool moving;
		private int resize;

		private Point gameFieldLocation;
		private Rectangle gameFieldBounds;
		private Bitmap gameField;
		private Graphics gfGraphics;

		private GameCell[,] GameField;
		private int gameWidth;
		private int gameHeight;
		private int bombCount;
		private int cellSize;
		private int gridWidth;

		private int[,] dd = new int[8, 2] { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 },
			{ 1, -1 }, { 1, 1 }, { -1, 1 }, { -1, -1 } };

		private void LocateGUI()
		{
			controls["corner_exit"].SetPlace(new Point(Width - controlBoxHeight + 1, 1));
			controls["corner_exit"].SetSize(new Size(controlBoxHeight - 2, controlBoxHeight - 2));
			controls["control_box"].SetPlace(new Point());
			controls["control_box"].SetSize(new Size(Width, controlBoxHeight));

			controls["bottom_grip"].SetPlace(new Point(0, Height - 2));
			controls["bottom_grip"].SetSize(new Size(Width, 2));
			controls["right_grip"].SetPlace(new Point(Width - 2, 0));
			controls["right_grip"].SetSize(new Size(2, Height));
			controls["bottom_right_grip"].SetPlace(new Point(Width - 5, Height - 5));
			controls["bottom_right_grip"].SetSize(new Size(5, 5));

			gridWidth = parameters["GridWidth"];
			int szW = Width - 6;
			int szH = Height - controlBoxHeight - 6;
			cellSize = Math.Min((szW - (gameWidth + 1) * gridWidth) / gameWidth,
				(szH - (gameHeight + 1) * gridWidth) / gameHeight);

			gameField = new Bitmap(cellSize * gameWidth + (gameWidth + 1) * gridWidth
				, cellSize * gameHeight + (gameHeight + 1) * gridWidth);
			gfGraphics = Graphics.FromImage(gameField);
			gameFieldLocation = new Point(3, 3 + controlBoxHeight);
			gameFieldBounds = new Rectangle(3, 3 + controlBoxHeight, cellSize * gameWidth + (gameWidth + 1) * gridWidth
				, cellSize * gameHeight + (gameHeight + 1) * gridWidth);

			controls["game_field"].Bounds = gameFieldBounds;
		}

		private void UpdateControls()
		{
			hovered = null;
			foreach (var control in controls)
			{
				control.Value.UpdateMouseState(cursor, leftMouse, rightMouse);
				if (control.Value.Hovered) hovered = control.Value;
			}
		}

		private void MoveForm()
		{
			Left = Cursor.Position.X - grip.X;
			Top = Cursor.Position.Y - grip.Y;
		}

		private void ResizeForm()
		{
			switch (resize)
			{
				case 3: // Right
					Size = new Size(Math.Max(Cursor.Position.X - Left + 1, minWidth), Height);
					FormResize();
					break;
				case 4: // Bottom-right
					Size = new Size(Math.Max(Cursor.Position.X - Left + 1, minWidth),
						Math.Max(Cursor.Position.Y - Top + 1, minHeight));
					FormResize();
					break;
				case 5: // Bottom
					Size = new Size(Width, Math.Max(Cursor.Position.Y - Top + 1, minHeight));
					FormResize();
					break;
			}
		}

		private void StartRelocaing()
		{
			grip = cursor;
			moving = true;
		}

		private void StopRelocaing()
		{
			moving = false;
		}

		private void CloseForm()
		{
			Close();
		}

		private void UpdateFrame()
		{
			foreach (var i in controls)
			{
				if (i.Value.Name == "") continue;
				if (i.Value == hovered)
					if (i.Value.PressedL) graphics.DrawImage(textures[i.Value.Name + "Pressed"], i.Value.Bounds);
					else graphics.DrawImage(textures[i.Value.Name + "Hovered"], i.Value.Bounds);
				else
					graphics.DrawImage(textures[i.Value.Name + "Normal"], i.Value.Bounds);
			}
			UpdateGameField();
			graphics.DrawImage(gameField, gameFieldLocation);

			viewport.Refresh();
		}

		private void DrawInitial()
		{ 
			graphics.Clear(colorTheme["BG"]);
			graphics.DrawRectangle(new Pen(colorTheme["ControlBoxBG"]), new Rectangle(0, 0, Width - 1, Height - 1));
			graphics.FillRectangle(new SolidBrush(colorTheme["ControlBoxBG"]), controls["control_box"].Bounds);
		}

		private void FormResize()
		{
			bitmap = new Bitmap(Width, Height);
			graphics = Graphics.FromImage(bitmap);
			LocateGUI();
			DrawInitial();
			InitialGameField();
			viewport.Image = bitmap;
		}

		private void UpdateGameField()
		{
			var toDel = new List<Coords>();
			foreach (var i in toDraw)
			{
				if (i.CompareTo(GameCell.Hovered) == 0 || GameField[i.X, i.Y].IsOpened)
				{
					if (GameField[i.X, i.Y].NextFrame())
					{
						toDel.Add(i);
					}
				}
				else
				{
					if (GameField[i.X, i.Y].PrevFrame())
					{
						toDel.Add(i);
					}
				}
				var rect = new Rectangle(i.X * cellSize + gridWidth * (i.X + 1),
					i.Y * cellSize + gridWidth * (i.Y + 1),
					cellSize, cellSize);
				//gfGraphics.Clip = new Region(rect);
				//gfGraphics.Clear(Color.White);
				gfGraphics.DrawImage(textures["Cell" + GameField[i.X, i.Y].BombCount.ToString()],
					rect);
				if (GameField[i.X, i.Y].HasBomb && GameField[i.X, i.Y].IsOpened)
					gfGraphics.DrawImage(textures["Bomb0"], rect);
				gfGraphics.DrawImage(textures[GameField[i.X, i.Y].GetFrameName()],
					rect);
				//gfGraphics.ResetClip();
			}
			foreach (var i in toDel)
			{
				toDraw.Remove(i);
			}
		}

		private void GameMouseMove()
		{
			int x = cursor.X - gameFieldLocation.X;
			x /= cellSize + gridWidth;
			int y = cursor.Y - gameFieldLocation.Y;
			y /= cellSize + gridWidth;
			if (x == gameWidth) x--; if (y == gameHeight) y--;
			if (GameCell.Hovered.X > 0)
			if (!GameField[GameCell.Hovered.X, GameCell.Hovered.Y].IsOpened)
				toDraw.Add(GameCell.Hovered);
			GameCell.Hovered = new Coords(x, y);
			if (!GameField[x, y].IsOpened)
				toDraw.Add(GameCell.Hovered);
		}

		private void GameMouseLeave()
		{
			if (!GameField[GameCell.Hovered.X, GameCell.Hovered.Y].IsOpened)
				toDraw.Add(GameCell.Hovered);
			GameCell.Hovered = new Coords(-1, -1);
		}

		private void GameMouseDown()
		{
			GameCell.PressedL = GameCell.Hovered;
		}

		private void GameMouseUp()
		{
			if (GameCell.PressedL.CompareTo(GameCell.Hovered) == 0 && 
				!GameField[GameCell.PressedL.X, GameCell.PressedL.Y].IsOpened
				&&
				!GameField[GameCell.PressedL.X, GameCell.PressedL.Y].HasFlag)
			{
				if (!GameField[GameCell.PressedL.X, GameCell.PressedL.Y].Inited) GenerateField(GameCell.PressedL);
				if (GameField[GameCell.PressedL.X, GameCell.PressedL.Y].HasBomb)
				{
					GameOver(GameCell.PressedL);
				} else
				{
					OpenCell(GameCell.PressedL);
				}
			}
		}

		private void GameMouseRightClick()
		{
			if (!GameField[0, 0].Inited) return;
			if (!GameField[GameCell.Hovered.X, GameCell.Hovered.Y].IsOpened)
			{
				GameField[GameCell.Hovered.X, GameCell.Hovered.Y].HasFlag =
					!GameField[GameCell.Hovered.X, GameCell.Hovered.Y].HasFlag;
				toDraw.Add(GameCell.Hovered);
				if (GameField[GameCell.Hovered.X, GameCell.Hovered.Y].HasFlag)
					GameField[GameCell.Hovered.X, GameCell.Hovered.Y].SetAnimation("FlagRaise", parameters["FlagRaiseClipCount"]);
				else
					GameField[GameCell.Hovered.X, GameCell.Hovered.Y].SetAnimation("ClosedCellHover", parameters["ClosedCellHoverClipCount"]);
			}
		}

		private void OpenCell(Coords cell)
		{
			Queue<Coords> queue = new Queue<Coords>();
			queue.Enqueue(cell);
			
			while (queue.Count > 0)
			{
				var p = queue.Dequeue();
				if (GameField[p.X, p.Y].BombCount == 0)
					for (int i = 0; i < 8; i++)
					{
						int xn = p.X + dd[i, 0];
						int yn = p.Y + dd[i, 1];
						if (xn < 0 || yn < 0 || xn == gameWidth || yn == gameHeight || GameField[xn, yn].IsOpened
							|| GameField[xn, yn].HasBomb || queue.Contains(new Coords(xn, yn))) continue;
						queue.Enqueue(new Coords(xn, yn));
					}
				GameField[p.X, p.Y].SetAnimation("CellOpening", parameters["CellOpeningClipCount"]);
				GameField[p.X, p.Y].IsOpened = true;
				toDraw.Add(p);
			}
		}

		private void GameOver(Coords cell)
		{
			for (int x = 0; x < gameWidth; x++)
			{
				for (int y = 0; y < gameHeight; y++)
				{
					if (GameField[x, y].HasBomb)
					{
						GameField[x, y].SetAnimation("CellOpening", parameters["CellOpeningClipCount"]);
						GameField[x, y].IsOpened = true;
						toDraw.Add(new Coords(x, y));
					}
				}
			}
		}

		private void StartGame()
		{
			GameField = new GameCell[gameWidth, gameHeight];
			toDraw = new SortedSet<Coords>();
			for (int x = 0; x < gameWidth; x++)
			{
				for (int y = 0; y < gameHeight; y++)
				{
					GameField[x, y].SetAnimation("ClosedCellHover", parameters["ClosedCellHoverClipCount"]);
				}
			}
		}

		private void GenerateField(Coords startPoint)
		{
			Random generator = new Random((int)(DateTime.Now.Ticks % int.MaxValue));
			SortedSet<Coords> prohibited = new SortedSet<Coords>();
			prohibited.Add(startPoint);
			for (int i = 0; i < 8; i++)
			{
				int xn = startPoint.X + dd[i, 0];
				int yn = startPoint.Y + dd[i, 1];
				if (xn < 0 || yn < 0 || xn == gameWidth || yn == gameHeight) continue;
				prohibited.Add(new Coords(xn, yn));
			}
			int bombLeft = bombCount;
			while (bombLeft > 0)
			{
				int xn = generator.Next(gameWidth);
				int yn = generator.Next(gameHeight);
				if (prohibited.Contains(new Coords(xn, yn))) continue;
				if (GameField[xn, yn].HasBomb) continue;
				bombLeft--;
				GameField[xn, yn].HasBomb = true;
			}
			for (int x = 0; x < gameWidth; x++)
			{
				for (int y = 0; y < gameHeight; y++)
				{
					GameField[x, y].Inited = true;
					for (int i = 0; i < 8; i++)
					{
						int xn = x + dd[i, 0];
						int yn = y + dd[i, 1];
						if (xn < 0 || yn < 0 || xn == gameWidth || yn == gameHeight) continue;
						if (GameField[xn, yn].HasBomb) GameField[x, y].BombCount++;
					}
					if (GameField[x, y].HasBomb) GameField[x, y].BombCount = 9;
				}
			}
		}

		private void InitialGameField()
		{
			gfGraphics.Clear(Color.White);
			Pen pen = new Pen(colorTheme["GridColor"], gridWidth);
			int w = gameField.Width;
			int h = gameField.Height;
			for (int x = 0; x <= gameWidth; x++)
			{
				int X = x * cellSize + x * gridWidth;
				gfGraphics.DrawLine(pen, X, 0, X, h);
			}

			for (int y = 0; y <= gameHeight; y++)
			{
				int Y = y * cellSize + y * gridWidth;
				gfGraphics.DrawLine(pen, 0, Y, w, Y);
			}
			for (int x = 0; x < gameWidth; x++)
			{
				for (int y = 0; y < gameHeight; y++)
				{
					var rect = new Rectangle(x * cellSize + gridWidth * (x + 1),
					y * cellSize + gridWidth * (y + 1),
					cellSize, cellSize);
					gfGraphics.DrawImage(textures[GameField[x, y].GetFrameName()],
						rect);
				}
			}
		}

		private void ReadTheme()
		{
			colorTheme = new Dictionary<string, Color>();
			textures = new Dictionary<string, Bitmap>();
			parameters = new Dictionary<string, int>();
			string path = "Resources\\Themes\\Classic\\";
			var di = new FileInfo(path + "Theme.txt");
			var reader = di.OpenText();
			while (!reader.EndOfStream)
			{
				var s = reader.ReadLine().Replace('\t', ' ').Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				if (s.Length == 0) continue;
				char param = s[0][0];
				switch (param)
				{
					case '#':
						continue;

					case 'C':
						colorTheme.Add(s[1], ColorTranslator.FromHtml(s[2]));
						break;

					case 'T':
						{
							string prefix = s[1];
							string fileName = s[2];
							for (int i = 2; i < s.Length; i++) if (fileName[fileName.Length - 1] != '"') fileName += s[i];
							fileName = fileName.Substring(1, fileName.Length - 2);
							var bit = new Bitmap(path + fileName);
							textures.Add(prefix + "Source", bit);
							for (int i = 0; i < 3; i++)
							{
								s = reader.ReadLine().Replace('\t', ' ').Split(new char[1] { ' ' },
									StringSplitOptions.RemoveEmptyEntries);
								if (s.Length == 0) continue;
								if (s[0][0] == '#')
								{
									i--;
									continue;
								}
								textures.Add(prefix + s[0], ImagingPlus.GetClip(bit,
									int.Parse(s[1]), int.Parse(s[2]), int.Parse(s[3]), int.Parse(s[4])));
							}
							break;
						}

					case 'P':
						parameters.Add(s[1], int.Parse(s[2]));
						break;

					case 'A':
						{
							int clipCount = int.Parse(s[2]);
							string prefix = s[1];
							string fileName = s[3];
							for (int i = 3; i < s.Length; i++) if (fileName[fileName.Length - 1] != '"') fileName += s[i];
							fileName = fileName.Substring(1, fileName.Length - 2);
							var bit = new Bitmap(path + fileName);
							textures.Add(prefix + "Source", bit);
							parameters.Add(prefix + "ClipCount", clipCount);
							for (int i = 0; i < clipCount; i++)
							{
								s = reader.ReadLine().Replace('\t', ' ').Split(new char[1] { ' ' },
									StringSplitOptions.RemoveEmptyEntries);
								if (s.Length == 0) continue;
								if (s[0][0] == '#')
								{
									i--;
									continue;
								}
								textures.Add(prefix + i.ToString(), ImagingPlus.GetClip(bit,
									int.Parse(s[0]), int.Parse(s[1]), int.Parse(s[2]), int.Parse(s[3])));
							}
							break;
						}

					default:
						MessageBox.Show("Theme file reading error: unexpected prefix: " + param + "\n[" + di.FullName + "]"
							, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						break;
				}
			}
		}
	}

	public struct GameCell
	{
		public static Coords Hovered { get; set; }
		public static Coords PressedL { get; set; }
		public static Coords PressedR { get; set; }
		public bool HasBomb { get; set; }
		public bool HasFlag { get; set; }
		public bool IsOpened { get; set; }
		public bool Inited { get; set; }
		public int BombCount { get; set; }

		private int clipCount;
		private int index;
		private string animation;

		public void SetAnimation(string anim, int clipCount)
		{
			animation = anim;
			this.clipCount = clipCount;
			index = 0;
		}

		public bool NextFrame()
		{
			if (index == clipCount - 1) return true;
			index++;
			return false;
		}

		public bool PrevFrame()
		{
			if (index == 0) return true;
			index--;
			return false;
		}

		public string GetFrameName()
		{
			return animation + index.ToString();
		}

		public void SetBombCount(int bombCount)
		{
			BombCount = bombCount;
		}
	}

	public struct Coords : IComparable<Coords>
	{
		public int X { get; set; }
		public int Y { get; set; }

		public int CompareTo(Coords other)
		{
			if (X == other.X)
			{
				if (Y == other.Y) return 0;
				if (Y < other.Y) return 1;
				return -1;
			}
			if (X < other.X) return 1;
			return -1;
		}

		public Coords(int x, int y)
		{
			X = x;
			Y = y;
		}
	}
}
