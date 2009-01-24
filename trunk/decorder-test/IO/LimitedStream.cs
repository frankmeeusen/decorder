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

namespace DeCorder
{
	public class LimitedStream : Stream
	{
		public Stream BaseStream;
		private long begin;
		private long current;
		private long length;
		private long end;

		public LimitedStream(Stream stream, long offset, long count)
		{
			this.BaseStream = stream;

			this.begin = offset;
			this.length = offset + count > stream.Length
				? stream.Length - offset
				: count;

			this.current = offset;
			this.end = offset + this.length;
		}

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return true; }
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		public override void Flush()
		{
			BaseStream.Flush();
		}

		public override long Length
		{
			get { return length; }
		}

		public override long Position
		{
			get
			{
				return current - begin;
			}
			set
			{
				if (value > length) current = end;
				else if (value < 0) current = begin;
				else current = begin + value;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int l;
			//lock (BaseStream)
			//{
			if (BaseStream.Position != current)
				BaseStream.Position = current;
			l = BaseStream.Read(buffer, offset,
				current + count > end ? (int)(end - current) : count);
			//}
			current += l;
			return l;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			switch (origin)
			{
				default:
				case SeekOrigin.Begin:
					return (current = begin + offset) - begin;
				case SeekOrigin.End:
					return (current = end + offset) - begin;
				case SeekOrigin.Current:
					return (current = current + offset) - begin;
			}
		}

		public override void SetLength(long value)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			int l = current + count > end ? (int)(end - current) : count;
			//lock (BaseStream)
			//{
			BaseStream.Position = current;
			BaseStream.Write(buffer, offset, l);
			//}
			current += l;
		}
	}
}
