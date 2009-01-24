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

namespace DeCorder
{
	/// <summary>
	/// MEDIA_TYPE
	/// </summary>
	public enum MediaType
	{
		Unknown,
		F5_1Pt2_512,
		F3_1Pt44_512,
		F3_2Pt88_512,
		F3_20Pt8_512,
		F3_720_512,
		F5_360_512,
		F5_320_512,
		F5_320_1024,
		F5_180_512,
		F5_160_512,
		RemovableMedia,
		FixedMedia,
		F3_120M_512,
		F3_640_512,
		F5_640_512,
		F5_720_512,
		F3_1Pt2_512,
		F3_1Pt23_1024,
		F5_1Pt23_1024,
		F3_128Mb_512,
		F3_230Mb_512,
		F8_256_128,
		F3_200Mb_512,
		F3_240M_512,
		F3_32M_512
	}
}
