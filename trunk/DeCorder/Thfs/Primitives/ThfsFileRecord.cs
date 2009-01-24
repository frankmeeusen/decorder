using System;
using System.Collections.Generic;
using System.Text;

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
	struct ThfsFileRecord
	{
		public int Parent;
		public int Child;
		public DateTime Modification1;
		public DateTime Modification2;
		public DateTime Modification3;
		public int Size;
		public int Reference;
		public int Next;
		public string Name;
		public ThfsFileRecord(byte[] buffer)
		{
			if (BitConverter.ToUInt32(buffer, 4) != 1179208773)
				throw new Exception("Invalid FILE record.");

			Parent = BitConverter.ToInt32(buffer, 0);
			Child = BitConverter.ToInt32(buffer, 8);
			Modification1 = new DateTime(1970, 1, 1).Add(TimeSpan.FromSeconds((double)BitConverter.ToInt32(buffer, 16)));
			Modification2 = new DateTime(1970, 1, 1).Add(TimeSpan.FromSeconds((double)BitConverter.ToInt32(buffer, 24)));
			Modification3 = new DateTime(1970, 1, 1).Add(TimeSpan.FromSeconds((double)BitConverter.ToInt32(buffer, 28)));
			Size = BitConverter.ToInt32(buffer, 48);
			Reference = BitConverter.ToInt32(buffer, 52);
			Next = BitConverter.ToInt32(buffer, 60);
			Name = Encoding.ASCII.GetString(buffer, 64, 448).Split('\x00')[0];
		}
	}
}
