using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Sapper_2019
{
	public class NewControl
	{
		public string Name { get; }
		public Rectangle Bounds;

		/// <summary>
		/// Is the cursor above this control?
		/// </summary>
		public bool Hovered { get; private set; }
		/// <summary>
		/// Is this control pressed? (Mouse button hold)
		/// </summary>
		public bool Pressed { get; private set; }
		private bool lastLeft;

		public delegate void EventHandler();

		public event EventHandler MouseEnter;
		public event EventHandler MouseLeave;
		public event EventHandler MouseMove;
		public event EventHandler MouseClickLeft;
		public event EventHandler MouseDownLeft;
		public event EventHandler MouseUpLeft;

		/// <summary>
		/// Raises mouse events
		/// </summary>
		/// <param name="ptr">Pointer's location</param>
		/// <param name="leftButton">Is left button is pressed?</param>
		public void UpdateMouseState(Point ptr, bool leftButton)
		{
			bool hovered = Bounds.Contains(ptr);

			if (hovered && !Hovered) // Hovered now but wasn't earlier
			{
				Hovered = true;
				MouseEnter?.Invoke();
			}
			else if (hovered && Hovered) // Hovered now and was earlier
			{
				MouseMove?.Invoke();
			}
			else if (!hovered && Hovered) // Unhovered now but was earlier
			{
				Hovered = false;
				MouseLeave?.Invoke();
			}

			if (hovered && leftButton && !lastLeft) // Hovered and left button clicked (but wasn't clicked earlier)
			{
				Pressed = true;
				MouseDownLeft?.Invoke();
			}
			else if (lastLeft && !leftButton && hovered) // If left button was pressed but not now and hovered
			{
				Pressed = false;
				MouseClickLeft?.Invoke();
				MouseUpLeft?.Invoke();
			}
			else if (!leftButton) // If button is not pressed now
			{
				if (lastLeft) MouseUpLeft?.Invoke();
				Pressed = false;
			}

			lastLeft = leftButton;
		}

		public void SetPlace(Point newloc)
		{
			Bounds.Location = newloc;
		}

		public void SetSize(Size newsz)
		{
			Bounds.Size = newsz;
		}

		public NewControl(string name)
		{
			Name = name;
			Bounds = new Rectangle();
		}
	}
}
