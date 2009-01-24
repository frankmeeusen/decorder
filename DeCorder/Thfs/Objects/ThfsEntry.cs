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
	public abstract class ThfsEntry : IEntry
	{
		internal ThfsFileRecord fileRecord;

		#region IEntry Members

		public virtual void Delete()
		{
			throw new NotImplementedException();
		}

		public virtual void Move(IDirectory newparent, string newname)
		{
			throw new NotImplementedException();
		}

		public IDirectory Parent
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string Name
		{
			get
			{
				return fileRecord.Name;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public DateTime Created
		{
			get
			{
				return fileRecord.Modification1;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public DateTime Modified
		{
			get
			{
				return fileRecord.Modification2;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public DateTime LastAccess
		{
			get
			{
				return fileRecord.Modification3;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		internal IVolume volume;
		public IVolume Volume
		{
			get { return volume; }
		}

		#endregion
	}
}
