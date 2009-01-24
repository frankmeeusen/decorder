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
	struct MasterBootRecord
	{
		public MasterBootRecord(Stream disk)
		{
			Console.WriteLine(disk.Length);

			byte[] buffer = new byte[512];
			disk.Read(buffer, 0, 512);

			CodeArea = new byte[446];
			//disk.Read(CodeArea, 0, 446);
			Buffer.BlockCopy(buffer, 0, CodeArea, 0, 446);

			OptionalDiskSignature = new byte[4];
			//disk.Read(OptionalDiskSignature, 0, 4);
			Buffer.BlockCopy(buffer, 440, OptionalDiskSignature, 0, 4);
			//OptionalDiskSignature[0] = CodeArea[440];
			//OptionalDiskSignature[1] = CodeArea[441];
			//OptionalDiskSignature[2] = CodeArea[442];
			//OptionalDiskSignature[3] = CodeArea[443];

			UsuallyNulls = new byte[2];
			//disk.Read(UsuallyNulls, 0, 2);
			Buffer.BlockCopy(buffer, 444, UsuallyNulls, 0, 2);
			//UsuallyNulls[0] = CodeArea[444];
			//UsuallyNulls[1] = CodeArea[445];
			if (UsuallyNulls[0] != 0x00) Console.WriteLine("UsuallyNulls[0] != 0x00");
			if (UsuallyNulls[1] != 0x00) Console.WriteLine("UsuallyNulls[1] != 0x00");

			TableOfPrimaryVolumes = new VolumeRecord[4];
			TableOfPrimaryVolumes[0] = new VolumeRecord(disk, buffer, 446);
			TableOfPrimaryVolumes[1] = new VolumeRecord(disk, buffer, 462);
			TableOfPrimaryVolumes[2] = new VolumeRecord(disk, buffer, 478);
			TableOfPrimaryVolumes[3] = new VolumeRecord(disk, buffer, 494);

			MbrSignature = new byte[2];
			//disk.Read(MbrSignature, 0, 2);
			Buffer.BlockCopy(buffer, 510, MbrSignature, 0, 2);
			if (MbrSignature[0] != 0x55) Console.WriteLine("MbrSignature[0] != 0x55");
			if (MbrSignature[1] != 0xAA) Console.WriteLine("MbrSignature[1] != 0xAA");
		}

		public void Debug()
		{
			Console.WriteLine("\n  >  Begin MasterBootRecord\n");

			Console.Write("  CodeArea: ");
			Console.WriteLine(BitConverter.ToString(CodeArea));

			Console.Write("  OptionalDiskSignature: ");
			Console.WriteLine(BitConverter.ToString(OptionalDiskSignature));

			Console.Write("  UsuallyNulls: ");
			Console.WriteLine(BitConverter.ToString(UsuallyNulls));

			Console.WriteLine("  TableOfPrimaryVolumes: ");
			foreach (VolumeRecord thing in TableOfPrimaryVolumes) thing.Debug();

			Console.Write("  MbrSignature: ");
			Console.WriteLine(BitConverter.ToString(MbrSignature));

			// According to wikipedia, but it seems to be wrong...
			//if (CodeArea[437] > 0 && CodeArea[438] > 0 && CodeArea[439] > 0)
			//{
			//	int idx = 300;
			//	int len;
			//	Console.Write("  Error Messages: ");
			//	len = CodeArea[437];
			//	Console.Write(Encoding.ASCII.GetString(CodeArea, idx, len));
			//	Console.Write(", ");
			//	idx += len;
			//	len = CodeArea[438];
			//	Console.Write(Encoding.ASCII.GetString(CodeArea, idx, len));
			//	Console.Write(", ");
			//	idx += len;
			//	len = CodeArea[439];
			//	Console.WriteLine(Encoding.ASCII.GetString(CodeArea, idx, len));
			//}

			Console.WriteLine("\n  >  End MasterBootRecord\n");
		}
		
		public byte[] CodeArea;
		public byte[] OptionalDiskSignature;
		public byte[] UsuallyNulls;
		public VolumeRecord[] TableOfPrimaryVolumes;
		public byte[] MbrSignature;
	}
}
