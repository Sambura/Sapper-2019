using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Sapper_2019
{
	class GUI
	{
		public class Button
		{
			private readonly Action clickAction;
			private readonly string name;
			private Rectangle bounds;

			private bool hovered;

			public bool IsTriggered(Point ptr)
			{
				hovered = bounds.Contains(ptr);
				return hovered;
			}

			public bool IsTriggered()
			{
				return hovered;
			}

			public void Click()
			{
				clickAction();
			}

			public void SetPlace(Point newloc)
			{
				bounds.Location = newloc;
			}

			public void SetSize(Size newsz)
			{
				bounds.Size = newsz;
			}

			public string GetName()
			{
				return name;
			}

			public Rectangle GetRect()
			{
				return bounds;
			}

			public Button(Action clickAction, string name)
			{
				this.clickAction = clickAction;
				this.name = name;
				bounds = new Rectangle();
				hovered = false;
			}
		}
	}
}
