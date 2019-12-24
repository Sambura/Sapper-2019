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
			private Rectangle bounds;

			private bool triggered;

			public bool IsTriggered(Point ptr)
			{
				triggered = bounds.Contains(ptr);
				return triggered;
			}

			public bool IsTriggered()
			{
				return triggered;
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

			public Button(Action clickAction)
			{
				this.clickAction = clickAction;
				bounds = new Rectangle();
				triggered = false;
			}
		}
	}
}
