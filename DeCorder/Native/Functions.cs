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
	static class Native
	{
		public const uint GENERIC_READ						= 0x80000000;
		public const uint GENERIC_WRITE						= 0x40000000;
		public const uint OPEN_EXISTING						= 0x00000003;
		public const uint FILE_ATTRIBUTE_NORMAL				= 0x00000080;
		public const uint IOCTL_DISK_GET_DRIVE_GEOMETRY		= 0x00070000;

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern SafeFileHandle CreateFile(
			string lpFileName,
			uint dwDesiredAccess,
			uint dwShareMode,
			IntPtr lpSecurityAttributes,
			uint dwCreationDisposition,
			uint dwFlagsAndAttributes,
			IntPtr hTemplateFile
		);

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern bool DeviceIoControl(
			Microsoft.Win32.SafeHandles.SafeFileHandle hDevice,
			uint IoControlCode,
			IntPtr InBuffer,
			uint nInBufferSize,
			ref DiskGeometry OutBuffer,
			int nOutBufferSize,
			out uint pBytesReturned,
			IntPtr Overlapped
		);
	}
}
