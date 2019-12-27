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
		public bool PressedL { get; private set; }
		public bool PressedR { get; private set; }
		private bool lastLeft, lastRight;

		public delegate void EventHandler();

		public event EventHandler MouseEnter;
		public event EventHandler MouseLeave;
		public event EventHandler MouseMove;
		public event EventHandler MouseClickLeft;
		public event EventHandler MouseDownLeft;
		public event EventHandler MouseUpLeft;
		public event EventHandler MouseClickRight;
		public event EventHandler MouseDownRight;
		public event EventHandler MouseUpRight;

		/// <summary>
		/// Raises mouse events
		/// </summary>
		/// <param name="ptr">Pointer's location</param>
		/// <param name="leftButton">Is left button is pressed?</param>
		public void UpdateMouseState(Point ptr, bool leftButton, bool rightButton)
		{
			bool hovered = Bounds.Contains(ptr);

			if (hovered && !Hovered) // Hovered now but wasn't earlier
			{
				Hovered = true;
				MouseEnter?.Invoke();
				MouseMove?.Invoke();
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
				PressedL = true;
				MouseDownLeft?.Invoke();
			}
			else if (lastLeft && !leftButton && hovered) // If left button was pressed but not now and hovered
			{
				PressedL = false;
				MouseClickLeft?.Invoke();
				MouseUpLeft?.Invoke();
			}
			else if (!leftButton) // If button is not pressed now
			{
				if (lastLeft) MouseUpLeft?.Invoke();
				PressedL = false;
			}

			if (hovered && rightButton && !lastRight) // Hovered and left button clicked (but wasn't clicked earlier)
			{
				PressedR = true;
				MouseDownRight?.Invoke();
			}
			else if (lastRight && !rightButton && hovered) // If left button was pressed but not now and hovered
			{
				PressedR = false;
				MouseClickRight?.Invoke();
				MouseUpRight?.Invoke();
			}
			else if (!rightButton) // If button is not pressed now
			{
				if (lastRight) MouseUpRight?.Invoke();
				PressedR = false;
			}

			lastLeft = leftButton;
			lastRight = rightButton;
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
