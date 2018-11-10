using System;
using Ngco.OpenGL4Renderer;
using Ngco.Widgets;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using PrettyPrinter;

namespace Ngco.Standalone {
	public class AppWindow : GameWindow {
		public readonly new Context Context;

		public AppWindow() : base(
			1280, 720, new GraphicsMode(32, 24, 8, 0), "Ngco.Standalone",
			GameWindowFlags.Default, DisplayDevice.Default, 4, 1, GraphicsContextFlags.ForwardCompatible
		) => Context = new Context(new Renderer { Scale = 2 });

		protected override void OnRenderFrame(FrameEventArgs e) {
			Context.Renderer.Width = Width;
			Context.Renderer.Height = Height;
			Context.Render();
			SwapBuffers();
		}

		protected override void OnMouseMove(MouseMoveEventArgs e) =>
			Context.MouseMove(new Point((int) Math.Round(e.X / Context.Renderer.Scale), (int) Math.Round(e.Y / Context.Renderer.Scale)));

		MouseButton FromOpenTK(OpenTK.Input.MouseButton button) {
			switch(button) {
				case OpenTK.Input.MouseButton.Left:
					return MouseButton.Left;
				case OpenTK.Input.MouseButton.Right:
					return MouseButton.Right;
				case OpenTK.Input.MouseButton.Middle:
					return MouseButton.Middle;
				default:
					return 0;
			}
		}

		protected override void OnMouseDown(MouseButtonEventArgs e) =>
			Context.MouseDown(FromOpenTK(e.Button));

		protected override void OnMouseUp(MouseButtonEventArgs e) =>
			Context.MouseUp(FromOpenTK(e.Button));

		Key? ConvertKey(OpenTK.Input.Key key) {
			switch(key) {
				case OpenTK.Input.Key.LControl:
				case OpenTK.Input.Key.RControl:
					return Key.Ctrl;
				case OpenTK.Input.Key.LAlt:
				case OpenTK.Input.Key.RAlt:
					return Key.Alt;
				case OpenTK.Input.Key.LWin:
				case OpenTK.Input.Key.RWin:
					return Key.Win;
				case OpenTK.Input.Key.LShift:
				case OpenTK.Input.Key.RShift:
					return Key.Shift;
				case OpenTK.Input.Key.Space: return Key.Space;
				case OpenTK.Input.Key.Enter: return Key.Enter;
				case OpenTK.Input.Key.BackSpace: return Key.Backspace;
				case OpenTK.Input.Key.Delete: return Key.Delete;
				case OpenTK.Input.Key.Tab: return Key.Tab;
				case OpenTK.Input.Key.Escape: return Key.Escape;
				case OpenTK.Input.Key.Up: return Key.Up;
				case OpenTK.Input.Key.Down: return Key.Down;
				case OpenTK.Input.Key.Left: return Key.Left;
				case OpenTK.Input.Key.Right: return Key.Right;
			}
			return null;
		}

		protected override void OnKeyDown(KeyboardKeyEventArgs e) {
			var key = ConvertKey(e.Key);
			if(key != null)
				Context.HandleKeyDown(key.Value);
		}

		protected override void OnKeyUp(KeyboardKeyEventArgs e) {
			var key = ConvertKey(e.Key);
			if(key != null)
				Context.HandleKeyUp(key.Value);
		}

		protected override void OnKeyPress(KeyPressEventArgs e) =>
			Context.HandleKeyPress(e.KeyChar);
	}
}