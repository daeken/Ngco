using System;

namespace Ngco {
	public enum Key {
		Ctrl, 
		Alt, 
		Win, 
		Shift, 
		Space, 
		Enter, 
		Backspace, 
		Delete, 
		Tab, 
		Escape, 
		Up, 
		Down, 
		Left, 
		Right
	}

	[Flags]
	public enum Modifier {
		Ctrl = 1, 
		Alt = 2, 
		Win = 4, 
		Shift = 8
	}
}