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
	public class ThfsFileStream : Stream
	{
		Stream baseStream;
		int[] allocation;
		int allocBlock = 0; // the allocation pair
		int allocPos = 0; // the sector
		int unused;

		byte[] cache = new byte[512]; // temp cache for stupid read requests
		int cacheOffset = 0; // position of cache
		int cacheCount = 0; // bytes left from read

		public ThfsFileStream(Stream stream, int sector, int[] allocation, int unused)
		{
			stream.Position = (long)(sector - allocation[1]) * 512L;
			this.baseStream = stream;
			this.allocation = allocation;
			this.unused = unused;
		}

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return false; } // NOT IMPLEMENTED YET //
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		public override void Flush()
		{

		}

		public override long Length
		{
			get { return 0; }// NOT IMPLEMENTED YET //
		}

		public override long Position
		{
			get
			{
				return 0;// NOT IMPLEMENTED YET //
			}
			set
			{
				throw new NotImplementedException(); // NOT IMPLEMENTED YET //
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int current = offset;
			int rest = count;

			if (cacheCount > 0)
			{
				if (cacheCount > count)
				{
					Buffer.BlockCopy(cache, cacheOffset, buffer, offset, count);
					cacheOffset += count; cacheCount -= count;
					return count;
				}
				Buffer.BlockCopy(cache, cacheOffset, buffer, current, cacheCount);
				if (cacheCount == count) { cacheOffset = 0; cacheCount = 0; return count; }
				current += cacheCount; rest -= cacheCount;
				cacheOffset = 0; cacheCount = 0;
			}

			if (allocBlock == allocation.Length) return count - rest; // EOF!

			while (rest >= 512)
			{
				int restsectors = rest / 512;
				int linkedsectors = allocation[allocBlock] - allocPos;
				if (restsectors < linkedsectors)
				{
					int read = restsectors * 512;
					if (baseStream.Read(buffer, current, read) != read) throw new Exception("Aaaarrgghh!!!");
					rest -= read; current += read;
					allocPos += restsectors;
				}
				else
				{
					int read = linkedsectors * 512;
					if (baseStream.Read(buffer, current, read) != read) throw new Exception("Whaaaargh?!"); 
					rest -= read; 
					allocPos = 0; allocBlock += 2;
					if (allocBlock == allocation.Length) return count - rest - unused; // eof!!!
					else { baseStream.Position += (long)allocation[allocBlock + 1] * 512L; current += read; }
				}
			}

			if (rest > 0)
			{
				if (baseStream.Read(cache, 0, 512) != 512) throw new Exception("Noooo!!!!!!"); allocPos++;
				Buffer.BlockCopy(cache, 0, buffer, current, rest); cacheCount = 512 - rest; cacheOffset = rest; 
				// now (rest = 0), don't use rest anymore in code
				if (allocPos == allocation[allocBlock]) // if this was the last sector of this block
				{
					allocPos = 0; allocBlock += 2;
					if (allocBlock == allocation.Length) // if this was the last sector of this file
					{
						cacheCount -= unused;
						if (cacheCount < 0) // if stuff was written into buffer that is not in this file
						{
							return count + cacheCount; // hide unneeded bytes
						}
					}// eof!!!
					else 
					{ 
						baseStream.Position += (long)allocation[allocBlock + 1] * 512L; 
					}
				}
			}

			return count;

		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return 0;// NOT IMPLEMENTED YET //
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}
	}
}
