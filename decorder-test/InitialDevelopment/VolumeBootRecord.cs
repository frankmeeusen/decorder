using System;
using System.Collections.Generic;
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

namespace InitialDevelopment
{
	class VolumeBootRecord
	{
		public VolumeBootRecord(Stream disk)
		{
			byte[] sector = new byte[512];
			disk.Read(sector, 0, 512);

			try
			{
				//long beginposition = 0;
				JumpInstruction = new byte[3];
				Buffer.BlockCopy(sector, 0, JumpInstruction, 0, 3);
				//disk.Read(JumpInstruction, 0, 3);
				int codepos = JumpInstruction[1];
				if (codepos < 64) codepos = 64; // hack against crashes!
				// short jump = 0xEB
				// no op = 0x90

				//byte[] idtemp = new byte[49];
				//disk.Read(idtemp, 0, 49);
				OemId = Encoding.ASCII.GetString(sector, 3, 8);

				//BiosParameterBlock = new byte[53];
				//disk.Read(BiosParameterBlock, 0, 53); 

				BytesPerSector = BitConverter.ToUInt16(sector, 11);
				SectorsPerCluster = sector[13];
				ReservedSectors = BitConverter.ToUInt16(sector, 14);
				NumberOfFATs = sector[16];
				RootEntries16 = BitConverter.ToUInt16(sector, 17);
				SmallSectors16 = BitConverter.ToUInt16(sector, 19);
				MediaDescriptor = sector[21];
				SectorsPerFat16 = BitConverter.ToUInt16(sector, 22);
				SectorsPerTrack = BitConverter.ToUInt16(sector, 24);
				NumberOfHeads = BitConverter.ToUInt16(sector, 26);
				HiddenSectors = BitConverter.ToUInt32(sector, 28);
				LargeSectors = BitConverter.ToUInt32(sector, 32);
				SectorsPerFat32 = BitConverter.ToUInt32(sector, 36);
				ExtendedFlags32 = BitConverter.ToUInt16(sector, 40);
				FileSystemVersion32 = BitConverter.ToUInt16(sector, 42);
				RootClusterNumber32 = BitConverter.ToUInt32(sector, 44);
				FileSystemInformationSectorNumber32 = BitConverter.ToUInt16(sector, 48);
				BackupBootSector32 = BitConverter.ToUInt16(sector, 50);

				Reserved32 = new byte[12];
				//disk.Read(Reserved32, 0, 12);
				Buffer.BlockCopy(sector, 52, Reserved32, 0, 12);

				int ebpblen = codepos - 64;
				ExtendedBiosParameterBlock = new byte[ebpblen];
				//disk.Read(ExtendedBiosParameterBlock, 0, ebpblen);
				Buffer.BlockCopy(sector, 64, ExtendedBiosParameterBlock, 0, ebpblen);

				int codelen = 508 - codepos;
				BootstrapCode = new byte[codelen];
				//disk.Read(BootstrapCode, 0, codelen);
				Buffer.BlockCopy(sector, codepos, BootstrapCode, 0, codelen);

				VbrSignature = new byte[2];
				//disk.Read(VbrSignature, 0, 2);
				Buffer.BlockCopy(sector, 510, VbrSignature, 0, 2);
				if (VbrSignature[0] != 0x55) Console.WriteLine("VbrSignature[0] != 0x55");
				if (VbrSignature[1] != 0xAA) Console.WriteLine("VbrSignature[1] != 0xAA");
			}
			catch (Exception ex) { Console.WriteLine(ex.Message); }
		}

		public void Debug()
		{
			try
			{
				Console.WriteLine("\n  >  Begin VolumeBootRecord\n");


				Console.Write("  JumpInstruction: ");
				Console.WriteLine(BitConverter.ToString(JumpInstruction));

				Console.Write("  OemId: ");
				Console.WriteLine(OemId);

				//Console.Write("  BiosParameterBlock: ");
				//Console.WriteLine(BitConverter.ToString(BiosParameterBlock));

				Console.Write("  BytesPerSector: ");
				Console.WriteLine(BytesPerSector);

				Console.Write("  SectorsPerCluster: ");
				Console.WriteLine(SectorsPerCluster);

				Console.Write("  ReservedSectors: ");
				Console.WriteLine(ReservedSectors);

				Console.Write("  NumberOfFATs: ");
				Console.WriteLine(NumberOfFATs);

				Console.Write("  RootEntries16: ");
				Console.WriteLine(RootEntries16);

				Console.Write("  SmallSectors16: ");
				Console.WriteLine(SmallSectors16);

				Console.Write("  MediaDescriptor: ");
				Console.WriteLine(MediaDescriptor);

				Console.Write("  SectorsPerFat16: ");
				Console.WriteLine(SectorsPerFat16);

				Console.Write("  SectorsPerTrack: ");
				Console.WriteLine(SectorsPerTrack);

				Console.Write("  NumberOfHeads: ");
				Console.WriteLine(NumberOfHeads);

				Console.Write("  HiddenSectors: ");
				Console.WriteLine(HiddenSectors);

				Console.Write("  LargeSectors: ");
				Console.WriteLine(LargeSectors);

				Console.Write("  SectorsPerFat32: ");
				Console.WriteLine(SectorsPerFat32);

				Console.Write("  ExtendedFlags32: ");
				Console.WriteLine(ExtendedFlags32);

				Console.Write("  FileSystemVersion32: ");
				Console.WriteLine(FileSystemVersion32);

				Console.Write("  RootClusterNumber32: ");
				Console.WriteLine(RootClusterNumber32);

				Console.Write("  FileSystemInformationSectorNumber32: ");
				Console.WriteLine(FileSystemInformationSectorNumber32);

				Console.Write("  BackupBootSector32: ");
				Console.WriteLine(BackupBootSector32);

				Console.Write("  Reserved32: ");
				Console.WriteLine(BitConverter.ToString(Reserved32));

				Console.Write("  ExtendedBiosParameterBlock: ");
				Console.WriteLine(BitConverter.ToString(ExtendedBiosParameterBlock));

				Console.Write("  BootstrapCode: ");
				Console.WriteLine(BitConverter.ToString(BootstrapCode));

				Console.Write("  VbrSignature: ");
				Console.WriteLine(BitConverter.ToString(VbrSignature));


				Console.WriteLine("\n  >  End VolumeBootRecord\n");
			}
			catch (Exception ex) { Console.WriteLine(ex.Message); }
		}

		public byte[] JumpInstruction;
		public string OemId;
		//public byte[] BiosParameterBlock;
		public ushort BytesPerSector;
		public byte SectorsPerCluster;
		public ushort ReservedSectors;
		public byte NumberOfFATs;
		public ushort RootEntries16;
		public ushort SmallSectors16;
		public byte MediaDescriptor;
		public ushort SectorsPerFat16;
		public ushort SectorsPerTrack;
		public ushort NumberOfHeads;
		public uint HiddenSectors;
		public uint LargeSectors;
		public uint SectorsPerFat32;
		public ushort ExtendedFlags32;
		public ushort FileSystemVersion32;
		public uint RootClusterNumber32;
		public ushort FileSystemInformationSectorNumber32;
		public ushort BackupBootSector32;
		public byte[] Reserved32;
		public byte[] ExtendedBiosParameterBlock;
		public byte[] BootstrapCode;
		public byte[] VbrSignature;
	}
}
