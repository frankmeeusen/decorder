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
	public static class Helper
	{
		public static IVolume OpenVolume(string image)
		{ return OpenVolume(new StreamSource(delegate { return new FileStream(image, FileMode.Open, FileAccess.Read, FileShare.ReadWrite); })); }
		public static IVolume OpenVolume(StreamSource source) { return OpenVolume(source, new byte[512]); }
		public static IVolume OpenVolume(StreamSource source, byte[] buffer)
		{
			Stream stream = source(FileAccess.ReadWrite);
			stream.Position = 0; VolumeBootRecord record = VolumeBootRecord.Read(stream, buffer);
			if (record is ThfsVolumeBootRecord) return new ThfsVolume(source, stream, (ThfsVolumeBootRecord)record);
			throw new Exception("Unknown volume boot record class!");
		}
	}

	public delegate Stream StreamSource(FileAccess access);
}
