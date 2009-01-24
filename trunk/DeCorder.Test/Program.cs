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

namespace Decorder.Test
{
	class Program
	{
		/// <summary>
		/// This is a (h***) of a File System
		/// </summary>
		/// <param name="?"></param>
		static void PrintThFSSector(int indent, string info, Stream volume, int sector)
		{
			string indentstr = new string(' ', indent);
			byte[] buffer = new byte[512];
			volume.Position = (long)sector * 512L;
			if (volume.Read(buffer, 0, 512) != 512)
			{ Console.WriteLine(indentstr + sector + ": (" + info + ") " + "ERROR;"); return; }
			switch (BitConverter.ToUInt32(buffer, 4))
			{
				case 1179208773: // FILE
					Console.WriteLine(indentstr + sector + ": (" + info + ") " + "FILE: p=" + BitConverter.ToUInt32(buffer, 0) + ", n=" + Encoding.ASCII.GetString(buffer, 64, 448).Split('\x00')[0] + ";");
					PrintThFSSector(indent + 1, sector.ToString(), volume, BitConverter.ToInt32(buffer, 8));
					break;
				case 1414677829: // TREE
					int c = BitConverter.ToInt32(buffer, 508);
					Console.WriteLine(indentstr + sector + ": (" + info + ") " + "TREE: p=" + BitConverter.ToUInt32(buffer, 0) + ", u=" + BitConverter.ToString(buffer, 8, 8) + ", n=" + Encoding.ASCII.GetString(buffer, 496, 12).Split('\x00')[0] + ", c=" + c + ";");
					int i = 16;
					for (int t = 0; t > c; t--)
					{
						string nameid = Encoding.ASCII.GetString(buffer, i, 12).Split('\x00')[0];
						int sectorl = BitConverter.ToInt32(buffer, i + 12);
						if (nameid.Length != 0 && sectorl != 0)
							PrintThFSSector(indent + 1, nameid, volume, sectorl);
						i += 16;
					}
					break;
				default:
					Console.WriteLine(indentstr + sector + ": (" + info + ") " + "UNKNOWN: " + Encoding.ASCII.GetString(buffer, 4, 4).Split('\x00')[0]);
					break;
			}
		}

		static void Main(string[] args)
		{
			Console.BufferHeight = 2048;
			Console.WindowWidth = 128;
			Decorder.IVolume volume;
			Decorder.Device device;
			Console.WriteLine("Decorder Test\n");
		BeginSelection:
			Console.WriteLine("\nVan waar wil u een partitie bekijken?");
		Console.WriteLine("  1.  Fysische schijf");
		Console.WriteLine("  2.  Image van schijf");
		Console.WriteLine("  3.  Image van partitie");
	SelectVolumeSource:
		try
		{
			switch (int.Parse(Console.ReadLine()))
			{
				case 1:
					Console.WriteLine("\nWat is de index van de schijf? (genummerd vanaf 0, enz.)");
				SelectDeviceIndex:
					try
					{
						device = Decorder.Device.Open(
								new StreamSource(
									delegate
									{
										return new Decorder.PhysicalDriveStream(
										 int.Parse(Console.ReadLine()),
										 FileAccess.Read);
									}));
						Console.ReadLine();
						goto DeviceSelected;
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				goto SelectDeviceIndex;
				case 2:
				Console.WriteLine("\nWat is het pad naar de image?");
			SelectDevicePath:
				try
				{
					device = Decorder.Device.Open(
								new StreamSource(
									delegate
									{
										return new FileStream(
											Console.ReadLine(),
											FileMode.Open,
											FileAccess.Read,
											FileShare.ReadWrite);
									}));
					goto DeviceSelected;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			goto SelectDevicePath;
				case 3:
			Console.WriteLine("\nWat is het pad naar de image?");
		SelectVolumePath:
			try
			{
				volume = Decorder.Helper.OpenVolume(Console.ReadLine());
				goto VolumeSelected;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		goto SelectVolumePath;
				default:
		Console.WriteLine("Ongeldige keuze.");
		goto SelectVolumeSource;
			}
		}
		catch (Exception ex) { Console.WriteLine(ex.Message); }
	goto SelectVolumeSource;
DeviceSelected:
	Console.WriteLine("\nWelke partitie wil u bekijken?");
Decorder.VolumeRecord[] volumerecords = device.MasterBootRecord.VolumeRecords;
for (int i = 0; i < volumerecords.Length; i++)
{
	Console.WriteLine(string.Format("  {0}.  {1}", i, volumerecords[i].ToString()));
}
SelectVolumeIndex:
try
{
	//int temp = int.Parse(Console.ReadLine());
	//Console.WriteLine(volumerecords[temp].ToString());
	//volume = Decorder.IVolume.Open(
	//             new Decorder.LimitedStream(
	//                     device.DeviceStream,
	//                     (long)volumerecords[temp].FirstSector * 512L,
	//                     (long)volumerecords[temp].TotalSectors * 512L));
	volume = device.GetVolume(int.Parse(Console.ReadLine()));
	goto VolumeSelected;
}
catch (Exception ex)
{
	Console.WriteLine(ex.Message);
	goto SelectVolumeIndex;
}
VolumeSelected:
Console.WriteLine("\nU heeft de volgende partitie geselecteerd:");
Console.WriteLine(volume.VolumeBootRecord.ToString());
Console.WriteLine("Wenst u verder te gaan? (j/n)");
SelectContinue:
switch (Console.ReadLine().ToLowerInvariant())
{
	case "j":
	case "ja":
	case "y":
	case "yes":
		break;
	case "n":
	case "nee":
	case "no":
		goto BeginSelection;
	default:
		goto SelectContinue;
}
Console.WriteLine();
PrintThFSSector(0, ((ThfsVolumeBootRecord)volume.VolumeBootRecord).OemId, volume.VolumeStream, ((ThfsVolumeBootRecord)volume.VolumeBootRecord).RootSector);

Console.WriteLine();
Console.WriteLine("Programma is geëindigd normaalgezien.");
Console.ReadLine();
		}
	}
}
