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
	public class MasterBootRecord
	{
		public VolumeRecord[] VolumeRecords;
		public uint DiskSignature;

		public static MasterBootRecord Read(byte[] buffer) { return new MasterBootRecord(buffer); }
		public static MasterBootRecord Read(Stream device, int sector) { return Read(device, new byte[512], sector); }
		public static MasterBootRecord Read(Stream device) { return Read(device, new byte[512]); }
		public static MasterBootRecord Read(Stream device, byte[] buffer, int sector)
		{
			device.Position = (long)sector * 512L;
			return Read(device, buffer);
		}
		public static MasterBootRecord Read(Stream device, byte[] buffer)
		{
			if (device.Read(buffer, 0, 512) != 512)
				throw new Exception("Error while trying to read from device.");
			return new MasterBootRecord(buffer);
		}
		public MasterBootRecord(byte[] buffer)
		{
			if (BitConverter.ToUInt16(buffer, 510) != 43605)
				throw new Exception(
						  string.Format("The MBR doesn't have a valid Signature. The invalid Signature is {0}, but should be 55-AA.",
						  BitConverter.ToString(buffer, 510, 2)));

			this.VolumeRecords = new VolumeRecord[4];
			this.VolumeRecords[0] = new VolumeRecord(buffer, 446);
			this.VolumeRecords[1] = new VolumeRecord(buffer, 462);
			this.VolumeRecords[2] = new VolumeRecord(buffer, 478);
			this.VolumeRecords[3] = new VolumeRecord(buffer, 494);

			this.DiskSignature = BitConverter.ToUInt32(buffer, 440);
		}
	}
}
