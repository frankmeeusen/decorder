using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;

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
	public class Win32Stream : FileStream
	{
		public Win32Stream(string path, FileAccess access)
			: base(GetStreamHandle(path, access), access)
		{ }

		private static SafeFileHandle GetStreamHandle(string path, FileAccess access)
		{
			SafeFileHandle sfh = Native.CreateFile(
				path,
				access == FileAccess.Read
					? Native.GENERIC_READ
					: access == FileAccess.ReadWrite
						? Native.GENERIC_READ | Native.GENERIC_WRITE
						: access == FileAccess.Write
							? Native.GENERIC_WRITE
							: 0,
				3,
				IntPtr.Zero,
				Native.OPEN_EXISTING,
				Native.FILE_ATTRIBUTE_NORMAL,
				IntPtr.Zero);
			if (sfh.IsInvalid)
				throw new System.ComponentModel.Win32Exception(
					System.Runtime.InteropServices.Marshal.GetLastWin32Error());
			return sfh;
		}
	}
}
