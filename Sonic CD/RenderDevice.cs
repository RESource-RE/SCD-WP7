// Decompiled with JetBrains decompiler
// Type: Retro_Engine.RenderDevice
// Assembly: Sonic CD, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D35AF46A-1892-4F52-B201-E664C9200079
// Assembly location: C:\Users\koishi\Documents\REProjects\SCD-WP7-REDO\Data\Sonic CD.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Retro_Engine
{

    public static class RenderDevice
    {
      public const int NUM_TEXTURES = 6;
      private static GraphicsDevice gDevice;
      private static BasicEffect effect;
      private static Matrix projection2D;
      private static Matrix projection3D;
      private static RenderTarget2D renderTarget;
      private static SpriteBatch screenSprite;
      private static Rectangle screenRect;
      private static RasterizerState rasterState = new RasterizerState();
      public static Texture2D[] gfxTexture = new Texture2D[6];
      public static int orthWidth;
      public static int viewWidth;
      public static int viewHeight;
      public static float viewAspect;
      public static int bufferWidth;
      public static int bufferHeight;
      public static int highResMode = 0;
      public static bool useFBTexture = true;

      public static void InitRenderDevice(GraphicsDevice graphicsRef)
      {
        RenderDevice.gDevice = graphicsRef;
        RenderDevice.effect = new BasicEffect(RenderDevice.gDevice);
        RenderDevice.effect.TextureEnabled = true;
        GraphicsSystem.SetupPolygonLists();
        for (int index = 0; index < 6; ++index)
          RenderDevice.gfxTexture[index] = new Texture2D(RenderDevice.gDevice, 1024 /*0x0400*/, 1024 /*0x0400*/, false, SurfaceFormat.Bgra5551);
        RenderDevice.renderTarget = new RenderTarget2D(RenderDevice.gDevice, 400, 240 /*0xF0*/, false, SurfaceFormat.Bgr565, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        RenderDevice.rasterState.CullMode = CullMode.None;
        RenderDevice.gDevice.RasterizerState = RenderDevice.rasterState;
        RenderDevice.gDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        RenderDevice.screenSprite = new SpriteBatch(RenderDevice.gDevice);
      }

      public static void UpdateHardwareTextures()
      {
        RenderDevice.gDevice.Textures[0] = (Texture) null;
        GraphicsSystem.SetActivePalette((byte) 0, 0, 240 /*0xF0*/);
        GraphicsSystem.UpdateTextureBufferWithTiles();
        GraphicsSystem.UpdateTextureBufferWithSortedSprites();
        RenderDevice.gfxTexture[0].SetData<ushort>(GraphicsSystem.texBuffer);
        for (byte paletteNum = 1; paletteNum < (byte) 6; ++paletteNum)
        {
          GraphicsSystem.SetActivePalette(paletteNum, 0, 240 /*0xF0*/);
          GraphicsSystem.UpdateTextureBufferWithTiles();
          GraphicsSystem.UpdateTextureBufferWithSprites();
          RenderDevice.gfxTexture[(int) paletteNum].SetData<ushort>(GraphicsSystem.texBuffer);
        }
        GraphicsSystem.SetActivePalette((byte) 0, 0, 240 /*0xF0*/);
      }

      public static void SetScreenDimensions(int width, int height)
      {
        InputSystem.touchWidth = width;
        InputSystem.touchHeight = height;
        RenderDevice.viewWidth = InputSystem.touchWidth;
        RenderDevice.viewHeight = InputSystem.touchHeight;
        RenderDevice.bufferWidth = (int) ((float) RenderDevice.viewWidth / (float) RenderDevice.viewHeight * 240f);
        RenderDevice.bufferWidth += 8;
        RenderDevice.bufferWidth = RenderDevice.bufferWidth >> 4 << 4;
        if (RenderDevice.bufferWidth > 400)
          RenderDevice.bufferWidth = 400;
        RenderDevice.viewAspect = 0.75f;
        GlobalAppDefinitions.HQ3DFloorEnabled = RenderDevice.viewHeight >= 480;
        if (RenderDevice.viewHeight >= 480)
        {
          GraphicsSystem.SetScreenRenderSize(RenderDevice.bufferWidth, RenderDevice.bufferWidth);
          RenderDevice.bufferWidth *= 2;
          RenderDevice.bufferHeight = 480;
        }
        else
        {
          RenderDevice.bufferHeight = 240 /*0xF0*/;
          GraphicsSystem.SetScreenRenderSize(RenderDevice.bufferWidth, RenderDevice.bufferWidth);
        }
        RenderDevice.orthWidth = GlobalAppDefinitions.SCREEN_XSIZE * 16 /*0x10*/;
        RenderDevice.projection2D = Matrix.CreateOrthographicOffCenter(4f, (float) (RenderDevice.orthWidth + 4), 3844f, 4f, 0.0f, 100f);
        RenderDevice.projection3D = Matrix.CreatePerspectiveFieldOfView(1.83259571f, RenderDevice.viewAspect, 0.1f, 2000f) * Matrix.CreateScale(1f, -1f, 1f) * Matrix.CreateTranslation(0.0f, -0.045f, 0.0f);
        RenderDevice.screenRect = new Rectangle(0, 0, RenderDevice.viewWidth, RenderDevice.viewHeight);
      }

      public static void FlipScreen()
      {
        RenderDevice.gDevice.SetRenderTarget(RenderDevice.renderTarget);
        RenderDevice.effect.Texture = RenderDevice.gfxTexture[GraphicsSystem.texPaletteNum];
        RenderDevice.effect.World = Matrix.Identity;
        RenderDevice.effect.View = Matrix.Identity;
        RenderDevice.effect.Projection = RenderDevice.projection2D;
        RenderDevice.effect.LightingEnabled = false;
        RenderDevice.effect.VertexColorEnabled = true;
        RenderDevice.gDevice.RasterizerState = RenderDevice.rasterState;
        if (GraphicsSystem.render3DEnabled)
        {
          foreach (EffectPass pass in RenderDevice.effect.CurrentTechnique.Passes)
          {
            pass.Apply();
            RenderDevice.gDevice.BlendState = BlendState.Opaque;
            RenderDevice.gDevice.SamplerStates[0] = SamplerState.PointClamp;
            if (GraphicsSystem.gfxIndexSizeOpaque > (ushort) 0)
              RenderDevice.effect.GraphicsDevice.DrawUserIndexedPrimitives<DrawVertex>(PrimitiveType.TriangleList, GraphicsSystem.gfxPolyList, 0, (int) GraphicsSystem.gfxVertexSizeOpaque, GraphicsSystem.gfxPolyListIndex, 0, (int) GraphicsSystem.gfxIndexSizeOpaque);
          }
          RenderDevice.gDevice.BlendState = BlendState.NonPremultiplied;
          RenderDevice.effect.World = Matrix.CreateTranslation(GraphicsSystem.floor3DPos) * Matrix.CreateRotationY((float) (3.1415927410125732 * (180.0 + (double) GraphicsSystem.floor3DAngle) / 180.0));
          RenderDevice.effect.Projection = RenderDevice.projection3D;
          foreach (EffectPass pass in RenderDevice.effect.CurrentTechnique.Passes)
          {
            pass.Apply();
            if (GraphicsSystem.indexSize3D > (ushort) 0)
              RenderDevice.effect.GraphicsDevice.DrawUserIndexedPrimitives<DrawVertex3D>(PrimitiveType.TriangleList, GraphicsSystem.polyList3D, 0, (int) GraphicsSystem.vertexSize3D, GraphicsSystem.gfxPolyListIndex, 0, (int) GraphicsSystem.indexSize3D);
          }
          RenderDevice.effect.World = Matrix.Identity;
          RenderDevice.effect.Projection = RenderDevice.projection2D;
          foreach (EffectPass pass in RenderDevice.effect.CurrentTechnique.Passes)
          {
            pass.Apply();
            int primitiveCount = (int) GraphicsSystem.gfxIndexSize - (int) GraphicsSystem.gfxIndexSizeOpaque;
            if (primitiveCount > 0)
              RenderDevice.effect.GraphicsDevice.DrawUserIndexedPrimitives<DrawVertex>(PrimitiveType.TriangleList, GraphicsSystem.gfxPolyList, (int) GraphicsSystem.gfxVertexSizeOpaque, (int) GraphicsSystem.gfxVertexSize - (int) GraphicsSystem.gfxVertexSizeOpaque, GraphicsSystem.gfxPolyListIndex, 0, primitiveCount);
          }
        }
        else
        {
          foreach (EffectPass pass in RenderDevice.effect.CurrentTechnique.Passes)
          {
            pass.Apply();
            RenderDevice.gDevice.BlendState = BlendState.Opaque;
            RenderDevice.gDevice.SamplerStates[0] = SamplerState.PointClamp;
            if (GraphicsSystem.gfxIndexSizeOpaque > (ushort) 0)
              RenderDevice.effect.GraphicsDevice.DrawUserIndexedPrimitives<DrawVertex>(PrimitiveType.TriangleList, GraphicsSystem.gfxPolyList, 0, (int) GraphicsSystem.gfxVertexSizeOpaque, GraphicsSystem.gfxPolyListIndex, 0, (int) GraphicsSystem.gfxIndexSizeOpaque);
            RenderDevice.gDevice.BlendState = BlendState.NonPremultiplied;
            int primitiveCount = (int) GraphicsSystem.gfxIndexSize - (int) GraphicsSystem.gfxIndexSizeOpaque;
            if (primitiveCount > 0)
              RenderDevice.effect.GraphicsDevice.DrawUserIndexedPrimitives<DrawVertex>(PrimitiveType.TriangleList, GraphicsSystem.gfxPolyList, (int) GraphicsSystem.gfxVertexSizeOpaque, (int) GraphicsSystem.gfxVertexSize - (int) GraphicsSystem.gfxVertexSizeOpaque, GraphicsSystem.gfxPolyListIndex, 0, primitiveCount);
          }
        }
        RenderDevice.effect.Texture = (Texture2D) null;
        RenderDevice.gDevice.SetRenderTarget((RenderTarget2D) null);
        RenderDevice.screenSprite.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
        RenderDevice.screenSprite.Draw((Texture2D) RenderDevice.renderTarget, RenderDevice.screenRect, Color.White);
        RenderDevice.screenSprite.End();
      }

      public static void FlipScreenHRes()
      {
        RenderDevice.effect.Texture = RenderDevice.gfxTexture[GraphicsSystem.texPaletteNum];
        RenderDevice.effect.World = Matrix.Identity;
        RenderDevice.effect.View = Matrix.Identity;
        RenderDevice.effect.Projection = RenderDevice.projection2D;
        RenderDevice.effect.LightingEnabled = false;
        RenderDevice.effect.VertexColorEnabled = true;
        RenderDevice.gDevice.RasterizerState = RenderDevice.rasterState;
        foreach (EffectPass pass in RenderDevice.effect.CurrentTechnique.Passes)
        {
          pass.Apply();
          RenderDevice.gDevice.BlendState = BlendState.Opaque;
          RenderDevice.gDevice.SamplerStates[0] = SamplerState.LinearClamp;
          if (GraphicsSystem.gfxIndexSizeOpaque > (ushort) 0)
            RenderDevice.effect.GraphicsDevice.DrawUserIndexedPrimitives<DrawVertex>(PrimitiveType.TriangleList, GraphicsSystem.gfxPolyList, 0, (int) GraphicsSystem.gfxVertexSizeOpaque, GraphicsSystem.gfxPolyListIndex, 0, (int) GraphicsSystem.gfxIndexSizeOpaque);
          RenderDevice.gDevice.BlendState = BlendState.NonPremultiplied;
          int primitiveCount = (int) GraphicsSystem.gfxIndexSize - (int) GraphicsSystem.gfxIndexSizeOpaque;
          if (primitiveCount > 0)
            RenderDevice.effect.GraphicsDevice.DrawUserIndexedPrimitives<DrawVertex>(PrimitiveType.TriangleList, GraphicsSystem.gfxPolyList, (int) GraphicsSystem.gfxVertexSizeOpaque, (int) GraphicsSystem.gfxVertexSize - (int) GraphicsSystem.gfxVertexSizeOpaque, GraphicsSystem.gfxPolyListIndex, 0, primitiveCount);
        }
        RenderDevice.effect.Texture = (Texture2D) null;
      }
    }
}
