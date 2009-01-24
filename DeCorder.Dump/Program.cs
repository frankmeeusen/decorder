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

namespace Decorder.Dump
{
	class Program
	{
		static void Main(string[] args)
		{
			Device device;
			int temp;
			Stream source;
			Stream target;
		//////////////////// Begin
		Begin:
			Console.WriteLine("Decorder Dump\n\n");
	//////////////////// DiskSelection
	DiskSelection:
		Console.WriteLine("\nWat is de index van de schijf? (genummerd vanaf 0, enz.)");
SelectDisk:
	try
	{
		Console.Write("> ");
		//device = new DcDevice(
		source = new PhysicalDriveStream(int.Parse(Console.ReadLine()), FileAccess.Read);
		//				 FileAccess.Read));
	}
	catch (Exception ex)
	{
		Console.WriteLine(ex.Message);
		goto SelectDisk;
	}
Console.WriteLine("\n");
//////////////////// TaskSelection
TaskSelection:
Console.WriteLine("1. Image maken van een volledige schijf");
Console.WriteLine("2. Image maken van een partitie op een schijf");
//Console.WriteLine("3. Image maken van een partitie op een image van een schijf");
SelectTask:
try
{
	Console.Write("> ");
	switch (int.Parse(Console.ReadLine()))
	{
		case 1:
			Console.WriteLine("\n");
			goto TargetSelection;
		case 2:
			Console.WriteLine("\n");
			device = Device.Open(
					new StreamSource(
						delegate
						{
							return source;
						}));
			goto VolumeSelection;
		//case 3:
		//    break;
		default:
			Console.WriteLine("Ongeldige keuze.");
			goto SelectTask;
	}
}
catch (Exception ex)
{
	Console.WriteLine(ex.Message);
	goto SelectTask;
}
//////////////////// VolumeSelection
VolumeSelection:
for (int i = 0; i < device.MasterBootRecord.VolumeRecords.Length; i++)
	Console.WriteLine(i + ". " + device.MasterBootRecord.VolumeRecords[i].ToString());
SelectVolume:
try
{
	Console.Write("> ");
	source = device.GetVolume(int.Parse(Console.ReadLine())).VolumeStream;
	source.Position = 0;
}
catch (Exception ex)
{
	Console.WriteLine(ex.Message);
	goto SelectVolume;
}
Console.WriteLine("\n");
//////////////////// TargetSelection
TargetSelection:
Console.WriteLine("Waar wil u de image bewaren?");
SelectTarget:
try
{
	Console.Write("> ");
	target = new FileStream(
				 Console.ReadLine(),
				 FileMode.CreateNew,
				 FileAccess.Write);
	byte[] buffer = new byte[512];
	int l;
	while ((l = source.Read(buffer, 0, 512)) != 0)
		target.Write(buffer, 0, l);
}
catch (Exception ex)
{
	Console.WriteLine(ex.Message);
	goto SelectTarget;
}
Console.WriteLine("\n\n\n");
source.Close();
target.Close();
goto Begin;
		}
	}
}
