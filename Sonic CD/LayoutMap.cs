// Decompiled with JetBrains decompiler
// Type: Retro_Engine.LayoutMap
// Assembly: Sonic CD, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D35AF46A-1892-4F52-B201-E664C9200079
// Assembly location: C:\Users\koishi\Documents\REProjects\SCD-WP7-REDO\Data\Sonic CD.dll

namespace Retro_Engine
{

    public class LayoutMap
    {
      public ushort[] tileMap = new ushort[65536 /*0x010000*/];
      public byte[] lineScrollRef = new byte[32768 /*0x8000*/];
      public int parallaxFactor;
      public int scrollSpeed;
      public int scrollPosition;
      public int angle;
      public int xPos;
      public int yPos;
      public int zPos;
      public int deformationPos;
      public int deformationPosW;
      public byte type;
      public byte xSize;
      public byte ySize;
    }
}
