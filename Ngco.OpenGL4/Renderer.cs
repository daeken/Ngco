using OpenTK.Graphics.OpenGL4;
using SkiaSharp;
using System;

namespace Ngco.OpenGL4Renderer {
    public class Renderer : IRenderer {
		readonly GRContext Context;
		GRBackendRenderTargetDesc RenderTarget;

		public int   Width { get; set; }
		public int   Height { get; set; }
		public float Scale { get; set; }

		public Renderer() {
			var glInterface = GRGlInterface.CreateNativeGlInterface();
			Context = GRContext.Create(GRBackend.OpenGL, glInterface);
			
			RenderTarget = new GRBackendRenderTargetDesc {
				Config             = GRPixelConfig.Bgra8888, 
				Origin             = GRSurfaceOrigin.BottomLeft, 
				SampleCount        = 0, 
				StencilBits        = 8, 
				RenderTargetHandle = (IntPtr) 0
			};
		}
		
		public void Render(Action<RICanvas> inside) {
			RenderTarget.Width  = Width;
			RenderTarget.Height = Height;

			Context.ResetContext();

			using(var surface = SKSurface.Create(Context, RenderTarget)) {
				var canvas = surface.Canvas;
				inside(new RICanvas(canvas, Scale));
				canvas.Flush();
			}

			Context.Flush();

			GL.Disable(EnableCap.Blend);
			GL.Disable(EnableCap.VertexProgramPointSize);
			GL.BindVertexArray(0); 
			GL.FrontFace(FrontFaceDirection.Cw);
			GL.Enable(EnableCap.FramebufferSrgb);
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);
			GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
			GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			GL.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
			GL.UseProgram(0);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.DrawBuffer(DrawBufferMode.Back);
			GL.Enable(EnableCap.Dither);
			GL.DepthMask(true);
			GL.Enable(EnableCap.Multisample);
			GL.Disable(EnableCap.ScissorTest);
		}
	}
}