// Decompiled with JetBrains decompiler
// Type: Retro_Engine.CollisionMask16x16
// Assembly: Sonic CD, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D35AF46A-1892-4F52-B201-E664C9200079
// Assembly location: C:\Users\koishi\Documents\REProjects\SCD-WP7-REDO\Data\Sonic CD.dll

namespace Retro_Engine
{

    public class CollisionMask16x16
    {
      public sbyte[] floorMask = new sbyte[16384 /*0x4000*/];
      public sbyte[] leftWallMask = new sbyte[16384 /*0x4000*/];
      public sbyte[] rightWallMask = new sbyte[16384 /*0x4000*/];
      public sbyte[] roofMask = new sbyte[16384 /*0x4000*/];
      public uint[] angle = new uint[1024 /*0x0400*/];
      public byte[] flags = new byte[1024 /*0x0400*/];
    }
}
