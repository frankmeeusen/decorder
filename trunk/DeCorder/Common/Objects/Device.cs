using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;

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
	public class Device
	{
		public MasterBootRecord MasterBootRecord;
		public StreamSource CreateStream;

		public static Device Open(StreamSource source) { return Open(source, source(FileAccess.Read)); }
		public static Device Open(StreamSource source, byte[] buffer) { return Open(source, source(FileAccess.Read), buffer); }
		public static Device Open(StreamSource source, Stream stream) { return Open(source, stream, new byte[512]); }
		public static Device Open(StreamSource source, Stream stream, byte[] buffer)
		{
			stream.Position = 0;
			MasterBootRecord record = MasterBootRecord.Read(stream, buffer);
			stream.Close();
			return new Device(source, record);
		}
		public Device(StreamSource source, MasterBootRecord record)
		{
			CreateStream = source;
			MasterBootRecord = record;
		}

		public IVolume GetVolume(int index)
		{
			return Helper.OpenVolume(
				new StreamSource(
					delegate(FileAccess access)
					{
						return new LimitedStream(
								this.CreateStream(access),
								(long)this.MasterBootRecord.VolumeRecords[index].FirstSector * 512L,
								(long)this.MasterBootRecord.VolumeRecords[index].TotalSectors * 512L);
					}));
		}

		public IVolume GetBootableVolume()
		{
			foreach (VolumeRecord record in MasterBootRecord.VolumeRecords) if (record.BootIndicator == VolumeStatus.Active)
					return Helper.OpenVolume(
						new StreamSource(
						delegate(FileAccess access)
						{
							return new LimitedStream(this.CreateStream(access), record.FirstSector * 512L, record.TotalSectors * 512L);
						}));
			return null;
		}
	}
}
