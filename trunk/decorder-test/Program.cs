using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
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
	class Program
	{
		static void Main(string[] args)
		{
			// -1147142144
			Console.WindowWidth = 133;
			Console.BufferHeight = 8192;
			Stream volume = new FileStream(@"e:\temp\DigiCorder.part2.dump", FileMode.Open, FileAccess.Read);
			byte[] buffer = new byte[512];
			//FileStream newvol = new FileStream(@"g:\sys.thfs", FileMode.Open, FileAccess.Write);
			//FileStream exvol = new FileStream(@"i:\random\einde.bin", FileMode.Open, FileAccess.Read);
			//newvol.SetLength(1147474944 - exvol.Length);
			//newvol.Position = 1147474944 - exvol.Length;
			//byte[] buffer = new byte[exvol.Length];
			//if (exvol.Read(buffer, 0, buffer.Length) != buffer.Length) throw new Exception();
			//bitField = new BitArray((int)volume.Length / 512);
			//bitField[0] = true;
			ExportTest(volume, buffer);
			///byte[] aarg = new byte[bitField.Count / 8];
			///bitField.CopyTo(aarg, 0);
			//Console.Write(BitConverter.ToString(aarg));
			//foreach (bool b in bitField) Console.Write(b ? '1' : '0');
			//foreach (byte b in aarg) Console.Write(BitConverter.ToString(new byte[] { b }) + " ");
			//Console.WriteLine(BitConverter.ToString(BitArrayToByteArray(bitField)));

			//newvol.Write(buffer, 0, buffer.Length);
			Console.WriteLine("r");
		Whoo:
			try { PrintSector(volume, buffer, UInt32.Parse(Console.ReadLine())); }
			catch (Exception ex) { Console.WriteLine(ex.Message); }
		goto Whoo;
		}

		static void ExportTest(Stream volume, byte[] buffer)
		{
			DirectoryInfo targetDirectory = new DirectoryInfo(@"e:\temp\decorder\root\");
			targetDirectory.Create();

			volume.Position = 0;
			Decorder.ThfsVolumeBootRecord vbr = Decorder.ThfsVolumeBootRecord.Read(volume);
			int root = (int)vbr.RootSector;
			//bitField[root] = true;

			ExportTest(volume, buffer, targetDirectory, root);
		}

		static void ExportTest(Stream volume, byte[] buffer1, DirectoryInfo targetDirectory, int sector)
		{
			byte[] buffer2 = new byte[512];
			ExportTest(volume, buffer1, buffer2, targetDirectory, sector);
		}

		static void ExportTest(Stream volume, byte[] buffer1, byte[] buffer2, DirectoryInfo targetDirectory, int sector)
		{
			//bitField[sector] = true;
			FileSystemRecord fsr = FileSystemRecord.Get(volume, buffer1, buffer2, sector);
			//bitField[fsr.Child] = true;
			//bitField[fsr.Next] = true;
			//bitField[fsr.Parent] = true;
			//bitField[fsr.Reference] = true;
			if (fsr is FileRecord)
			{
				FileRecord fr = (FileRecord)fsr;
				FileStream fs = new FileStream(targetDirectory.FullName + "\\" + fr.Name, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
				if (fr.AllocationStart < 0) { Console.WriteLine("File starts below zero!"); }
				else
				{
					volume.Position = (long)fr.AllocationStart * 512L;
					//Decorder.LimitedStream ls = new Decorder.LimitedStream(volume, volume.Position, (long)fr.Size);
					//if (fr.FirstSector < 0) { Console.WriteLine("Very bad sector location in file " + sector + "!"); return; }
					//
					for (int i = 0; i < fr.Allocation.Length; i += 2)
					{
						volume.Position += (long)fr.Allocation[i + 1] * 512L;
						for (int j = 0; j < fr.Allocation[i]; j++)
						{
							//bitField[(int)(volume.Position / 512L)] = true;
							if (volume.Read(buffer1, 0, 512) != 512) throw new Exception("Can't read enough bytes!");
							fs.Write(buffer1, 0, 512);
						}
						//int c; while ((c = ls.Read(buffer1, 0, 512)) != 0) fs.Write(buffer1, 0, c);
					}
					if (fr.UnusedBytes == 512) fs.SetLength(fr.Size);
					else fs.SetLength(fs.Length - fr.UnusedBytes);
					if (fs.Length != fr.Size) Console.WriteLine("Sizes don't match!");
				}
				fs.Close();
				try
				{
					FileInfo fi = new FileInfo(targetDirectory.FullName + "\\" + fr.Name);
					fi.CreationTimeUtc = fr.Modification1;
					fi.LastAccessTimeUtc = fr.Modification2;
					fi.LastWriteTimeUtc = fr.Modification3;
				}
				catch (Exception ex) { Console.WriteLine(ex.Message); }
				Console.WriteLine(sector + ": " + targetDirectory.FullName + "\\" + fr.Name);
			}
			else if (fsr is DirectoryRecord)
			{
				DirectoryRecord dr = (DirectoryRecord)fsr;
				DirectoryInfo di = targetDirectory.CreateSubdirectory(fsr.Name);
				try
				{
					di.CreationTimeUtc = dr.Modification1;
					di.LastAccessTimeUtc = dr.Modification2;
					di.LastWriteTimeUtc = dr.Modification3;
				}
				catch (Exception ex) { Console.WriteLine(ex.Message); }
				Console.WriteLine(sector + ": " + di.FullName + "\\");
				foreach (int i in dr.Children) ExportTest(volume, buffer1, buffer2, di, i);
			}
			else Console.WriteLine("This shouldn't appear at all.");
		}

		static void PrintSector(Stream volume, byte[] buffer, uint sector)
		{
			long pos = (long)sector * 512L;
			Console.WriteLine(pos);
			if (pos > volume.Length - 512) { Console.WriteLine("Out of range!"); return; }
			volume.Position = pos;
			volume.Read(buffer, 0, 512);
			char[] cache = new char[32];

			for (int i = 0; i < 16; i++)
			{
				Console.Write("  ");
				Console.Write(BitConverter.ToString(buffer, i * 32, 32).Replace('-', ' '));
				Console.Write("  ");
				for (int j = 0; j < 32; j++) cache[j] = (char)buffer[(i * 32) + j];
				Console.WriteLine(new string(cache).Replace('\r', ' ').Replace('\n', ' ').Replace('\x07', ' ').Replace('\t', ' '));
			}

			switch (BitConverter.ToUInt32(buffer, 4))
			{
				case 1179208773: // FILE
					Console.WriteLine(" --- FILE --- ");
					//Console.WriteLine(indentstr + sector + ": (" + info + ") " + "FILE: p=" + BitConverter.ToUInt32(buffer, 0) + ", n=" + Encoding.ASCII.GetString(buffer, 64, 448).Split('\x00')[0] + ";");
					//PrintThFSSector(indent + 1, sector.ToString(), volume, BitConverter.ToUInt32(buffer, 8));
					Console.Write("Parent: "); Console.WriteLine(BitConverter.ToInt32(buffer, 0));
					Console.Write("Child: "); Console.WriteLine(BitConverter.ToInt32(buffer, 8));
					Console.WriteLine(BitConverter.ToInt32(buffer, 12));
					Console.Write("Modification: "); Console.WriteLine(new DateTime(1970, 1, 1).Add(TimeSpan.FromSeconds((double)BitConverter.ToInt32(buffer, 16))).ToString());
					Console.WriteLine(BitConverter.ToInt32(buffer, 20));
					Console.Write("Modification: "); Console.WriteLine(new DateTime(1970, 1, 1).Add(TimeSpan.FromSeconds((double)BitConverter.ToInt32(buffer, 24))).ToString());
					Console.Write("Modification: "); Console.WriteLine(new DateTime(1970, 1, 1).Add(TimeSpan.FromSeconds((double)BitConverter.ToInt32(buffer, 28))).ToString());
					for (int i = 32; i < 48; i += 4)
					{ Console.Write(BitConverter.ToInt32(buffer, i)); Console.Write(", "); } Console.WriteLine();
					Console.Write("Size: "); Console.WriteLine(BitConverter.ToInt32(buffer, 48));
					Console.Write("Reference: "); Console.WriteLine(BitConverter.ToInt32(buffer, 52));
					Console.WriteLine(BitConverter.ToInt32(buffer, 56));
					Console.Write("Next: "); Console.WriteLine(BitConverter.ToInt32(buffer, 60));
					Console.Write("Name: "); Console.WriteLine(Encoding.ASCII.GetString(buffer, 64, 448).Split('\x00')[0]);
					break;
				case 1414677829: // TREE
					Console.WriteLine(" --- TREE --- ");
					//int c = BitConverter.ToInt32(buffer, 508);
					//Console.WriteLine(indentstr + sector + ": (" + info + ") " + "TREE: p=" + BitConverter.ToUInt32(buffer, 0) + ", u=" + BitConverter.ToString(buffer, 8, 8) + ", n=" + Encoding.ASCII.GetString(buffer, 496, 12).Split('\x00')[0] + ", c=" + c + ";");
					//int i = 16;
					//for (int t = 0; t > c; t--)
					//{
					//    string nameid = Encoding.ASCII.GetString(buffer, i, 12).Split('\x00')[0];
					//    uint sectorl = BitConverter.ToUInt32(buffer, i + 12);
					//    if (nameid.Length != 0 && sectorl != 0)
					//        PrintThFSSector(indent + 1, nameid, volume, sectorl);
					//    i += 16;
					//}
					Console.Write("Parent: "); Console.WriteLine(BitConverter.ToInt32(buffer, 0));
					Console.WriteLine(BitConverter.ToInt32(buffer, 8));
					Console.WriteLine(BitConverter.ToInt32(buffer, 12));
					int h = 16;
					for (int i = 0; i > BitConverter.ToInt32(buffer, 508); i--)
					{
						string nameid = Encoding.ASCII.GetString(buffer, h, 12).Split('\x00')[0];
						uint sectorl = BitConverter.ToUInt32(buffer, h + 12);
						if (/*nameid.Length != 0 && */sectorl != 0)
						{
							Console.Write(sectorl);
							Console.Write(": ");
							Console.Write(nameid);
							Console.Write(", ");
						}
						h += 16;
					}
					Console.WriteLine();
					Console.WriteLine(BitConverter.ToInt32(buffer, 496));
					Console.WriteLine(BitConverter.ToInt32(buffer, 500));
					Console.WriteLine(BitConverter.ToInt32(buffer, 504));
					Console.Write("Negative: "); Console.WriteLine(BitConverter.ToInt32(buffer, 508));
					break;
				case 1095520067: // ALOC
					Console.WriteLine(" --- ALOC --- ");
					int[] die = FileSystemRecord.ByteKiller(buffer, 8, 496);
					for (int i = 0; i < die.Length; i++)
					{
						if (die[i] == 0) break;
						Console.Write(die[i].ToString()); Console.Write(", ");
					}
					Console.WriteLine();
					//for (int i = 8; i < 504; i++)
					//{ Console.Write(buffer[i].ToString()); Console.Write(", "); } Console.WriteLine();
					//int lol = 0;
					//for (int i = 8; i < 504; i+=2)
					//{ lol += buffer[i]; } Console.WriteLine(lol);
					//for (int i = 9; i < 504; i += 2)
					//{ lol += buffer[i]; } Console.WriteLine(lol);
					int off = die[0];
					for (int i = 2; i < die.Length; i++) off += die[i];
					Console.Write("FirstSector: "); Console.WriteLine(sector - off);
					Console.Write("UnusedBytes: "); Console.WriteLine(BitConverter.ToInt32(buffer, 504));
					Console.Write("SectorCount: "); Console.WriteLine(BitConverter.ToInt32(buffer, 508));
					break;
				case 1177118760: // VBS
					Console.WriteLine(" --- BOOT --- ");
					Console.Write("BytesPerSector: "); Console.WriteLine(BitConverter.ToUInt16(buffer, 11));
					Console.Write("SectorsPerTrack: "); Console.WriteLine(BitConverter.ToUInt16(buffer, 24));
					Console.Write("NumberOfHeads: "); Console.WriteLine(BitConverter.ToUInt16(buffer, 26));
					Console.Write("HiddenSectors: "); Console.WriteLine(BitConverter.ToUInt32(buffer, 28));
					for (int i = 32; i < 48; i += 4)
					{ Console.Write(BitConverter.ToInt32(buffer, i)); Console.Write(", "); } Console.WriteLine();
					Console.Write("RootSector: "); Console.WriteLine(BitConverter.ToInt32(buffer, 48));
					for (int i = 52; i < 512; i += 4)
					{ Console.Write(BitConverter.ToInt32(buffer, i)); Console.Write(", "); } Console.WriteLine();
					break;
				default:
					Console.WriteLine(" --- ---- --- ");
					break;
			}
			Console.WriteLine(" --- ---- --- ---- --- \n");
		}

		//static int[] ByteKiller(byte[] buffer, int offset, int length)
		//{
		//    int[] result = new int[length];
		//    byte[] temp = new byte[4];
		//    int r = 0;
		//    for (int i = offset; i < offset + length; i++)
		//    {
		//        temp[0] = 0; temp[1] = 0; temp[2] = 0; temp[3] = 0;
		//        BitArray ba = new BitArray(new byte[1] { buffer[i] });
		//        switch (ba[7])
		//        {
		//            case false: // 1 byte
		//                temp[3] = 0; temp[2] = 0; temp[1] = 0; temp[0] = buffer[i];
		//                break;
		//            case true:
		//                switch (ba[6])
		//                {
		//                    case false:
		//                        switch (ba[5])
		//                        {
		//                            case false: // 2 bytes, remove 10000000b
		//                                temp[3] = 0;
		//                                temp[2] = 0;
		//                                temp[1] = (byte)((byte)buffer[i] & (byte)0x1F);
		//                                i++; temp[0] = buffer[i];
		//                                break;
		//                            case true: // 3 bytes, remove 10100000b	
		//                                temp[3] = 0;
		//                                temp[2] = (byte)((byte)buffer[i] & (byte)0x1F);
		//                                i++; temp[1] = buffer[i];
		//                                i++; temp[0] = buffer[i];
		//                                break;
		//                        }
		//                        break;
		//                    case true:
		//                        switch (ba[5])
		//                        {
		//                            case false: // 4 bytes, remove 11000000b
		//                                temp[3] = (byte)((byte)buffer[i] & (byte)0x1F);
		//                                i++; temp[2] = buffer[i];
		//                                i++; temp[1] = buffer[i];
		//                                i++; temp[0] = buffer[i];
		//                                break;
		//                            case true:
		//                                throw new NotImplementedException();
		//                        }
		//                        break;
		//                }
		//                break;
		//        }
		//        result[r] = BitConverter.ToInt32(temp, 0);
		//        r++;
		//    }
		//    return result;
		//}

		static void TestFile(Stream dev, byte[] buffer, long volbyteidx, long sector)
		{
			long filebyteidx = volbyteidx + (sector * 512L);
			Console.WriteLine();
			Console.WriteLine("Device Byte Index of Volume: " + volbyteidx);
			Console.WriteLine("Volume Sector Index of FILE: " + sector);
			Console.WriteLine("Device Byte Index of FILE: " + filebyteidx);
			dev.Position = filebyteidx;
			dev.Read(buffer, 0, 512);
			Console.WriteLine("Volume Sector Index of FILE.Parent: " + BitConverter.ToUInt16(buffer, 0));
			Console.WriteLine("FILE.z: " + BitConverter.ToUInt16(buffer, 2));
			Console.WriteLine("FILE.MagicId: " + BitConverter.ToUInt32(buffer, 4) + " " + Encoding.ASCII.GetString(buffer, 4, 4));
			Console.WriteLine("FILE.Child: " + BitConverter.ToUInt16(buffer, 8));
			Console.WriteLine("FILE.y: " + BitConverter.ToUInt16(buffer, 10));
			Console.WriteLine("FILE.Name: " + Encoding.ASCII.GetString(buffer, 64, 448).Split('\x00')[0]);
		}

		static void TestTree(Stream dev, byte[] buffer, long volbyteidx, long sector)
		{
			long filebyteidx = volbyteidx + (sector * 512L);
			Console.WriteLine();
			Console.WriteLine("Device Byte Index of Volume: " + volbyteidx);
			Console.WriteLine("Volume Sector Index of TREE: " + sector);
			Console.WriteLine("Device Byte Index of TREE: " + filebyteidx);
			dev.Position = filebyteidx;
			dev.Read(buffer, 0, 512);
			Console.WriteLine("Volume Sector Index of TREE.Parent: " + BitConverter.ToUInt16(buffer, 0));
			Console.WriteLine("TREE.z: " + BitConverter.ToUInt16(buffer, 2));
			Console.WriteLine("TREE.MagicId: " + BitConverter.ToUInt32(buffer, 4) + " " + Encoding.ASCII.GetString(buffer, 4, 4));

			Console.WriteLine("TREE.x: " + BitConverter.ToUInt16(buffer, 8));
			Console.WriteLine("TREE.y: " + BitConverter.ToUInt16(buffer, 10));
			Console.WriteLine("TREE.a: " + BitConverter.ToUInt32(buffer, 12));

			int j = 0;
			for (int i = 16; i <= 512 - 32; i += 16)
			{
				Console.WriteLine();
				Console.WriteLine("TREE." + j + ".Name: " + Encoding.ASCII.GetString(buffer, i + 0, 12).Split('\x00')[0]);
				//Console.WriteLine("TREE." + j + ".Extension: " + Encoding.ASCII.GetString(buffer, i + 9, 3));
				uint ssector = BitConverter.ToUInt32(buffer, i + 12);
				Console.WriteLine("TREE." + j + ".Sector: " + ssector + " (byte " + (volbyteidx + (ssector * 512L)).ToString() + ")");

				j++;
			}

			Console.WriteLine();
			Console.WriteLine("TREE.b: " + Encoding.ASCII.GetString(buffer, 496, 12));
			Console.WriteLine("TREE.c: " + BitConverter.ToInt32(buffer, 508));
		}
	}
}
