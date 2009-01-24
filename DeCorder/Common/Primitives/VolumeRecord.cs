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

namespace Decorder
{
	public struct VolumeRecord
	{
		public VolumeStatus BootIndicator;
		public byte StartingHead;
		public byte StartingSector;
		public ushort StartingCylinder;
		public VolumeType VolumeType;
		public byte EndingHead;
		public byte EndingSector;
		public ushort EndingCylinder;
		public uint FirstSector;
		public uint TotalSectors;

		public VolumeRecord(byte[] buffer, int offset)
		{
			byte[] temp = new byte[2];
			BootIndicator = (VolumeStatus)buffer[offset];
			StartingHead = buffer[offset + 1];
			StartingSector = (byte)((buffer[offset + 2] & (byte)0xFC) >> 2);
			// not sure if this stuff is right, but not really used atm.
			temp[0] = (byte)((byte)((byte)buffer[offset + 2] & (byte)0x03) << 6);
			temp[1] = buffer[offset + 3];
			StartingCylinder = (ushort)(BitConverter.ToUInt16(temp, 0) >> 6);
			VolumeType = (VolumeType)buffer[offset + 4];
			EndingHead = buffer[offset + 5];
			EndingSector = (byte)((buffer[offset + 6] & (byte)0xFC) >> 2);
			temp[0] = (byte)((byte)((byte)buffer[offset + 6] & (byte)0x03) << 6);
			temp[1] = buffer[offset + 7];
			EndingCylinder = (ushort)(BitConverter.ToUInt16(temp, 0) >> 6);
			FirstSector = BitConverter.ToUInt32(buffer, offset + 8);
			TotalSectors = BitConverter.ToUInt32(buffer, offset + 12);
		}

		public void PrintTree(int indent)
		{
			Console.WriteLine(new string(' ', indent) + ToString());
		}

		public override string ToString()
		{
			return "b: " + BootIndicator.ToString()
				+ ", s: " + StartingHead.ToString()
				+ ", " + StartingSector.ToString()
				+ ", " + StartingCylinder.ToString()
				+ ", t: " + VolumeType.ToString()
				+ ", e: " + EndingHead.ToString()
				+ ", " + EndingSector.ToString()
				+ ", " + EndingCylinder.ToString()
				+ ", i: " + FirstSector.ToString()
				+ ", n: " + TotalSectors.ToString();
		}
	}
	
	public enum VolumeStatus : byte
	{
		Active = 0x80,
		Inactive = 0x00,
	}

	public enum VolumeType : byte
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
}
