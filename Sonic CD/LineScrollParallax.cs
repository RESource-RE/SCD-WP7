// Decompiled with JetBrains decompiler
// Type: Retro_Engine.LineScrollParallax
// Assembly: Sonic CD, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D35AF46A-1892-4F52-B201-E664C9200079
// Assembly location: C:\Users\koishi\Documents\REProjects\SCD-WP7-REDO\Data\Sonic CD.dll

namespace Retro_Engine
{

    public class LineScrollParallax
    {
      public int[] parallaxFactor = new int[256 /*0x0100*/];
      public int[] scrollSpeed = new int[256 /*0x0100*/];
      public int[] scrollPosition = new int[256 /*0x0100*/];
      public int[] linePos = new int[256 /*0x0100*/];
      public byte[] deformationEnabled = new byte[256 /*0x0100*/];
      public byte numEntries;
    }
}
