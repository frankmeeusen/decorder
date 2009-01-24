using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Collections;
using System.IO;

/* 
 * 
 * Decorder Test - Extracts files from a certain HDD
 * Copyright (C) 2007  Jan Boon
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * 
 */

namespace decorder_test
{
	abstract class FileSystemRecord
	{
		public static FileSystemRecord Get(Stream volume, byte[] buffer1, byte[] buffer2, int sector)
		{
			DirectoryRecord directory;
			FileRecord file;

			long pos = (long)sector * 512L;
			volume.Position = pos;
			volume.Read(buffer1, 0, 512);
			if (BitConverter.ToUInt32(buffer1, 4) != 1179208773)
			{ 
				directory = new DirectoryRecord();
				directory.Name = sector.ToString();
				Console.WriteLine("Not a FILE record, " + sector + "!"); 
				return directory;
			}

			pos = (long)BitConverter.ToInt32(buffer1, 8) * 512L;
			volume.Position = pos;
			volume.Read(buffer2, 0, 512);

			switch (BitConverter.ToUInt32(buffer2, 4))
			{
				case 1179208773: // child is a FILE, it's a dir!
					directory = new DirectoryRecord();
					FillFileSystemRecord(volume, buffer1, directory);
					int next = directory.Child;
					while (next != 0)
					{
						directory.Children.Add(next);
						volume.Position = (long)next * 512L;
						volume.Read(buffer1, 0, 512);
						next = BitConverter.ToInt32(buffer1, 60);
					}
					return directory;
				case 1414677829: // child is a TREE, it's a dir again!
					directory = new DirectoryRecord();
					FillFileSystemRecord(volume, buffer1, directory);
					FillTree(volume, buffer2, directory.Child, sector, directory.Children);
					return directory;
				case 1095520067: // child is an ALOC, it's a file!
					file = new FileRecord();
					FillFileSystemRecord(volume, buffer1, file);
					file.Allocation = ByteKiller(buffer2, 8, 496);
					file.AllocationStart = file.Child + file.Allocation[1];
					file.Allocation[1] = 0;
					file.UnusedBytes = BitConverter.ToInt32(buffer2, 504);
					file.SectorCount = BitConverter.ToInt32(buffer2, 508);
					if (file.SectorCount * 512 - file.UnusedBytes != file.Size) Console.WriteLine("File " + sector + " isn't correct size!");
					return file;
				default: // usually empty file
					file = new FileRecord();
					FillFileSystemRecord(volume, buffer1, file);
					file.Allocation = new int[0];
					file.AllocationStart = 0;
					file.UnusedBytes = 0;
					file.SectorCount = 0;
					if (file.Child != 0) Console.WriteLine("Uh, I dunno what's the problem, " + sector + ".");
					return file;
			}
		}

		public int Parent;
		public int Child;
		public DateTime Modification1;
		public DateTime Modification2;
		public DateTime Modification3;
		public int Size;
		public int Reference;
		public int Next;
		public string Name;

		private static void FillTree(Stream volume, byte[] buffer1, int sector, int parent, Collection<int> children)
		{
			byte[] buffer2 = new byte[512]; // for child trees =)
			if (BitConverter.ToInt32(buffer1, 0) != parent) Console.WriteLine("The parent is wrong, wrong, wrong!");
			int h = 16;
			int c = BitConverter.ToInt32(buffer1, 508);
			if (c > 0) throw new Exception("It's not negative!");
			for (int i = 0; i > c; i--)
			{
				string nameid = Encoding.ASCII.GetString(buffer1, h, 12).Split('\x00')[0];
				int nexthm = BitConverter.ToInt32(buffer1, h + 12);
				//if (nameid.Length != 0)
				//{
					while (nexthm != 0)
					{
						volume.Position = (long)nexthm * 512L;
						volume.Read(buffer2, 0, 512);
						if (BitConverter.ToUInt32(buffer2, 4) == 1414677829)
						{
							FillTree(volume, buffer2, nexthm, sector, children);
							nexthm = 0;
						}
						else
						{
							children.Add(nexthm);
							nexthm = BitConverter.ToInt32(buffer2, 60);
						}
					}
				//}
				h += 16;
			}
		}

		private static void FillFileSystemRecord(Stream volume, byte[] buffer,  FileSystemRecord record)
		{
			record.Parent = BitConverter.ToInt32(buffer, 0);
			record.Child = BitConverter.ToInt32(buffer, 8);
			record.Modification1 = new DateTime(1970, 1, 1).Add(TimeSpan.FromSeconds((double)BitConverter.ToInt32(buffer, 16)));
			record.Modification2 = new DateTime(1970, 1, 1).Add(TimeSpan.FromSeconds((double)BitConverter.ToInt32(buffer, 24)));
			record.Modification3 = new DateTime(1970, 1, 1).Add(TimeSpan.FromSeconds((double)BitConverter.ToInt32(buffer, 28)));
			record.Size = BitConverter.ToInt32(buffer, 48);
			record.Reference = BitConverter.ToInt32(buffer, 52);
			record.Next = BitConverter.ToInt32(buffer, 60);
			record.Name = Encoding.ASCII.GetString(buffer, 64, 448).Split('\x00')[0];
		}

		public static int[] ByteKiller(byte[] buffer, int offset, int length)
		{
			int[] result = new int[length];
			byte[] temp = new byte[4];
			int r = 0;
			for (int i = offset; i < offset + length; i++)
			{
				temp[0] = 0; temp[1] = 0; temp[2] = 0; temp[3] = 0;
				BitArray ba = new BitArray(new byte[1] { buffer[i] });
				switch (ba[7])
				{
					case false: // 1 byte
						temp[3] = 0; temp[2] = 0; temp[1] = 0; temp[0] = (byte)((byte)buffer[i] & (byte)0x3F);
						if (ba[6]) result[r] = -BitConverter.ToInt32(temp, 0);
						else result[r] = BitConverter.ToInt32(temp, 0);
						break;
					case true:
						switch (ba[6])
						{
							case false:
								switch (ba[5])
								{
									case false: // 2 bytes, remove 10000000b
										temp[3] = 0x00;
										temp[2] = 0x00;
										temp[1] = (byte)((byte)buffer[i] & (byte)0x0F);
										i++; temp[0] = buffer[i];
										break;
									case true: // 3 bytes, remove 10100000b	
										temp[3] = 0x00;
										temp[2] = (byte)((byte)buffer[i] & (byte)0x0F);
										i++; temp[1] = buffer[i];
										i++; temp[0] = buffer[i];
										break;
								}
								break;
							case true: // 000 11111 10100111 01100111
								switch (ba[5])
								{
									case false: // 4 bytes, remove 11000000b
										temp[3] = (byte)((byte)buffer[i] & (byte)0x0F);
										i++; temp[2] = buffer[i];
										i++; temp[1] = buffer[i];
										i++; temp[0] = buffer[i];
										break;
									case true:
										throw new NotImplementedException();
								}
								break;
						}
						if (ba[4]) result[r] = -BitConverter.ToInt32(temp, 0);
						else result[r] = BitConverter.ToInt32(temp, 0);
						break;
				}
				r++;
			}
			return result;
		}
	}
	//enum FileSystemRecordMagicId : int
	//{
	//    File = 1179208773,
	//    Tree = 1414677829,
	//    Aloc = 1095520067
	//}

	//class FileSystemRecord
	//{
	//    public ushort IdSector;
	//    public long DevicePosition;

	//    public ushort Parent;
	//    public ushort _1;
	//    public FileSystemRecordMagicId MagicId; // FILE: 1179208773, TREE: 1414677829, ALOC: 1095520067

	//    public static FileSystemRecord Get(Stream device, byte[] buffer, long volume, ushort sector)
	//    {
	//        long devpos = volume + ((long)sector * 512L);
	//        device.Position = devpos;
	//        device.Read(buffer, 0, 512);
	//        FileSystemRecordMagicId mid = (FileSystemRecordMagicId)BitConverter.ToInt32(buffer, 4);
	//        switch (mid)
	//        {
	//            case FileSystemRecordMagicId.File:
	//                return new FileRecord();
	//            case FileSystemRecordMagicId.Tree:
	//                return new TreeRecord();
	//            case FileSystemRecordMagicId.Aloc:
	//                return new AlocRecord();
	//        }
	//        return new FileSystemRecord();
	//    }

	//    public static FileSystemRecord Get(Stream device, long volume, ushort sector)
	//    {
	//        return Get(device, new byte[512], volume, sector);
	//    }
	//}
}
