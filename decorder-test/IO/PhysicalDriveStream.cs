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

namespace DeCorder
{
	public class PhysicalDriveStream : Win32Stream
	{
		public PhysicalDriveStream(int index, FileAccess access)
			: base(@"\\.\PhysicalDrive" + index, access)
		{ length = GetDriveLength(this.SafeFileHandle); }

		private long length;
		public override long Length
		{
			get
			{
				return length;
			}
		}

		public static bool GetDriveGeometry(SafeFileHandle hDevice, ref DiskGeometry pdg)
		{
			uint junk;				   // discard results
			return Native.DeviceIoControl(
				hDevice,  // device to be queried
				Native.IOCTL_DISK_GET_DRIVE_GEOMETRY,  // operation to perform
				IntPtr.Zero, 0, // no input buffer
				ref pdg, Marshal.SizeOf(pdg),	 // output buffer
				out junk,				  // # bytes returned
				IntPtr.Zero);  // synchronous I/O
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return base.Read(buffer, offset, Position + (long)count > length ? (int)(length - Position) : count);
		}

		public static long GetDriveLength(SafeFileHandle hDevice)
		{
			DiskGeometry pdg = new DiskGeometry();
			if (GetDriveGeometry(hDevice, ref pdg))
			{
				Console.WriteLine(pdg.Cylinders + " " + pdg.TracksPerCylinder + " " + pdg.SectorsPerTrack + " " + pdg.BytesPerSector);
			
				return (long)pdg.Cylinders * (long)pdg.TracksPerCylinder *
				(long)pdg.SectorsPerTrack * (long)pdg.BytesPerSector;
			}
			else
				throw new Win32Exception();
		}
	}
}
