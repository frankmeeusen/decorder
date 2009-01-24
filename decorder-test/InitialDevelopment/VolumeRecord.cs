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
	enum VolumeStatus : byte
	{
		Bootable = 0x80,
		NonBootable = 0x00,
	}
	enum VolumeTypes : byte
	{
		Undefined = 0x00,
		Fat12PrimaryVolumeOrLogicalDrive = 0x01,
		Fat16VolumeOrLogicalDrive = 0x04,
		ExtendedVolume = 0x05,
		BigdosFat16VolumeOrLogicalDrive = 0x06,
		InstallableFileSystemNtfsVolumeOrLogicalDrive = 0x07,
		Fat32VolumeOrLogicalDrive = 0x0B,
		Fat32VolumeOrLogicalDriveUsingBiosInt13hExtensions = 0x0C,
		BigdosFat16VolumeOrLogicalDriveUsingBiosInt13hExtensions = 0x0E,
		ExtendedVolumeUsingBiosInt13hExtensions = 0x0F,
		EisaVolume = 0x12,
		DynamicDiskVolume = 0x42,
		LegacyFtFat16Disk = 0x86,
		LegacyFtNtfsDisk = 0x87,
		LegacyFtVolumeFormattedWithFat32 = 0x8B,
		LegacyFtVolumeUsingBiosInt16hExtensionsFormattedWithFat32 = 0x8C,
	}
	struct VolumeRecord
	{
		public VolumeRecord(Stream device, byte[] buffer, int index)
		{
			//byte[] temp = new byte[4];

			//disk.Read(temp, 0, 1);
			Status = (VolumeStatus)buffer[index];

			Starting = new byte[3];
			//disk.Read(Starting, 0, 3);
			Buffer.BlockCopy(buffer, index + 1, Starting, 0, 3);

			//disk.Read(temp, 0, 1);
			VolumeType = (VolumeTypes)buffer[index + 4];

			Ending = new byte[3];
			//disk.Read(Ending, 0, 3);
			Buffer.BlockCopy(buffer, index + 5, Ending, 0, 3);

			//disk.Read(temp, 0, 4);
			RelativeSectors = BitConverter.ToUInt32(buffer, index + 8);
			RelativeBytes = (long)RelativeSectors * 512L;

			//disk.Read(temp, 0, 4);
			TotalSectors = BitConverter.ToUInt32(buffer, index + 12);
			TotalBytes = (long)TotalSectors * 512L;

			if (TotalBytes > 0 && RelativeBytes < device.Length)
			{
				long curpos = device.Position;
				device.Position = RelativeBytes;
				VolumeBootRecord = new VolumeBootRecord(device);
				device.Position = curpos;
			}
			else
			{
				VolumeBootRecord = null;
			}
		}

		public void Debug()
		{
			Console.WriteLine("\n  >  Begin VolumeRecord\n");


			Console.Write("  Status: ");
			Console.WriteLine(Status.ToString());

			Console.Write("  Starting: ");
			Console.WriteLine(BitConverter.ToString(Starting));

			Console.Write("  VolumeType: ");
			Console.WriteLine(VolumeType.ToString());

			Console.Write("  Ending: ");
			Console.WriteLine(BitConverter.ToString(Ending));

			Console.Write("  RelativeSectors: ");
			Console.WriteLine(RelativeSectors);

			Console.Write("  TotalSectors: ");
			Console.WriteLine(TotalSectors);

			Console.Write("  RelativeBytes: ");
			Console.WriteLine(RelativeBytes);

			Console.Write("  TotalBytes: ");
			Console.WriteLine(TotalBytes);

			Console.Write("  Size: ");
			if (TotalBytes >= 10995116277760)
			{
				Console.Write((double)TotalBytes / 1099511627776.0);
				Console.WriteLine(" TiB");
			}
			else if (TotalBytes >= 10737418240)
			{
				Console.Write((double)TotalBytes / 1073741824.0);
				Console.WriteLine(" GiB");
			}
			else if (TotalBytes >= 10485760)
			{
				Console.Write((double)TotalBytes / 1048576.0);
				Console.WriteLine(" MiB");
			}
			else if (TotalBytes >= 10240)
			{
				Console.Write((double)TotalBytes / 1024.0);
				Console.WriteLine(" KiB");
			}
			else
			{
				Console.Write(TotalBytes);
				Console.WriteLine(" Bytes");
			}

			if (VolumeBootRecord == null) Console.WriteLine("  VolumeBootRecord: null");
			else
			{
				Console.WriteLine("  VolumeBootRecord: ");
				VolumeBootRecord.Debug();
			}

			Console.WriteLine("\n  >  End VolumeRecord\n");
		}

		public VolumeStatus Status;
		public byte[] Starting;
		public VolumeTypes VolumeType;
		public byte[] Ending;
		public uint RelativeSectors;
		public uint TotalSectors;

		public long RelativeBytes;
		public long TotalBytes;

		public VolumeBootRecord VolumeBootRecord;
	}
}
