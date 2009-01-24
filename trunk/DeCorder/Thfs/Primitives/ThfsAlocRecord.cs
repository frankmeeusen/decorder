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

namespace Decorder
{
	public class ThfsAlocRecord
	{
		//public int Sector;

		public int[] Allocation;
		public int UnusedBytes;
		public int SectorCount;

		//public ThfsAlocRecord(Stream volume, int sector) : this(volume, new byte[512], sector) { }
		//public ThfsAlocRecord(Stream volume, byte[] buffer, int sector)
		//{
		//    volume.Position = (long)sector * 512L;
		//    if (volume.Read(buffer, 0, 512) != 512)
		//        throw new Exception("Error while trying to read from volume.");
		//    Sector = sector;
		//}
		public ThfsAlocRecord(byte[] buffer)
		{
			Allocation = ByteKiller(buffer, 8, 496);

			if (BitConverter.ToInt32(buffer, 500) != 0)
				throw new Exception("This value might be useful.");

			UnusedBytes = BitConverter.ToInt32(buffer, 504);
			SectorCount = BitConverter.ToInt32(buffer, 508);
		}

		public static int[] ByteKiller(byte[] buffer, int offset, int length)
		{
			int[] result = new int[length];
			byte[] temp = new byte[4];
			int r = 0;
			for (int i = offset; i < offset + length; i++)
			{
				temp[0] = 0; temp[1] = 0; temp[2] = 0; temp[3] = 0;
				BitArray ba = new BitArray(new byte[1] { buffer[i] });
				if (ba[7])
				{
					if (ba[6])
					{
						if (ba[5]) throw new NotImplementedException();
						else
						{
							// 4 bytes, remove 11000000b
							temp[3] = (byte)((byte)buffer[i] & (byte)0x0F);
							i++; temp[2] = buffer[i];
							i++; temp[1] = buffer[i];
							i++; temp[0] = buffer[i];
						}
					}
					else
					{
						if (ba[5])
						{
							// 3 bytes
							temp[3] = 0x00;
							temp[2] = (byte)((byte)buffer[i] & (byte)0x0F);
							i++; temp[1] = buffer[i];
							i++; temp[0] = buffer[i];
						}
						else
						{
							// 2 bytes
							temp[3] = 0x00;
							temp[2] = 0x00;
							temp[1] = (byte)((byte)buffer[i] & (byte)0x0F);
							i++; temp[0] = buffer[i];
						}
					}
					if (ba[4]) result[r] = -BitConverter.ToInt32(temp, 0);
					else result[r] = BitConverter.ToInt32(temp, 0);
				}
				else
				{
					// 1 byte
					temp[3] = 0; temp[2] = 0; temp[1] = 0; temp[0] = (byte)((byte)buffer[i] & (byte)0x3F);
					if (ba[6]) result[r] = -BitConverter.ToInt32(temp, 0);
					else result[r] = BitConverter.ToInt32(temp, 0);
				}

				r++;
			}
			return result;
		}
	}
}
