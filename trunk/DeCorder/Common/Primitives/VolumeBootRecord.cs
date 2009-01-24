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
	public abstract class VolumeBootRecord
	{
		public static VolumeBootRecord Read(Stream volume, int sector) { return Read(volume, new byte[512], sector); }
		public static VolumeBootRecord Read(Stream volume) { return Read(volume, new byte[512]); }
		public static VolumeBootRecord Read(Stream volume, byte[] buffer, int sector)
		{
			volume.Position = (long)sector * 512L;
			return Read(volume, buffer);
		}
		public static VolumeBootRecord Read(Stream volume, byte[] buffer)
		{
			if (volume.Read(buffer, 0, 512) != 512)
				throw new Exception("Error while trying to read from volume.");
			return Read(buffer);
		}
		public static VolumeBootRecord Read(byte[] buffer)
		{
			if (BitConverter.ToUInt16(buffer, 510) != 43605)
				throw new Exception(
						  string.Format("The VBR doesn't have a valid Signature. The invalid Signature is {0}, but should be 55-AA.",
						  BitConverter.ToString(buffer, 510, 2)));

			if (Encoding.ASCII.GetString(buffer, 3, 8).Split('\x00')[0] == "T(h)FS  ")
				return new ThfsVolumeBootRecord(buffer);

			throw new Exception("Unknown file system.");
		}
	}
}
