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
	public class ThfsVolume : IVolume
	{
		ThfsVolumeBootRecord vbr;
		Stream stream;
		StreamSource source;

		public static ThfsVolume Open(StreamSource source, Stream stream) { return Open(source, stream, new byte[512]); }
		public static ThfsVolume Open(StreamSource source, Stream stream, byte[] buffer)
		{
			stream.Position = 0;
			ThfsVolumeBootRecord record = ThfsVolumeBootRecord.Read(stream, buffer);
			return new ThfsVolume(source, stream, record);
		}
		public ThfsVolume(StreamSource source, Stream stream, ThfsVolumeBootRecord record)
		{
			this.vbr = record; this.source = source; this.stream = stream;

			// Root directory instelle (partitie name enzo is allemaal de root dir!)
		}

		#region IVolume Members

		public Stream VolumeStream
		{
			get { return stream; }
		}

		VolumeBootRecord IVolume.VolumeBootRecord
		{
			get { return (VolumeBootRecord)vbr; }
		}

		public ThfsVolumeBootRecord VolumeBootRecord
		{
			get { return vbr; }
		}

		public Stream CreateStream(FileAccess access)
		{
			return source(access);
		}

		public IDirectory RootDirectory
		{
			get { throw new NotImplementedException(); }
		}

		public byte[] ReadSector(int sector) { return ReadSector(new byte[512], sector); }
		public byte[] ReadSector(byte[] buffer, int sector)
		{
			lock (stream)
			{
				stream.Position = (long)sector * 512L;
				if (stream.Read(buffer, 0, 512) != 512) 
					throw new Exception("Error while trying to read from volume.");
			}
			return buffer;
		}

		#endregion
	}
}
