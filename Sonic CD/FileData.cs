// Decompiled with JetBrains decompiler
// Type: Retro_Engine.FileData
// Assembly: Sonic CD, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D35AF46A-1892-4F52-B201-E664C9200079
// Assembly location: C:\Users\koishi\Documents\REProjects\SCD-WP7-REDO\Data\Sonic CD.dll

namespace Retro_Engine
{

    public class FileData
    {
      public char[] fileName = new char[64 /*0x40*/];
      public uint fileSize;
      public uint position;
      public uint bufferPos;
      public uint virtualFileOffset;
      public byte eStringPosA;
      public byte eStringPosB;
      public byte eStringNo;
      public bool eNybbleSwap;
    }
}
