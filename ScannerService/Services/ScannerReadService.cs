using System;
using System.Collections.Generic;
using System.IO;
using WIA;

namespace ScannerApi.Services
{
    public class ScannerReadService
    {
        // Method to list connected scanners
        public List<string> GetConnectedScanners()
        {
            List<string> scanners = new List<string>();
            DeviceManager deviceManager = new DeviceManager();

            for (int i = 1; i <= deviceManager.DeviceInfos.Count; i++)
            {
                DeviceInfo deviceInfo = deviceManager.DeviceInfos[i];
                if (deviceInfo.Type == WiaDeviceType.ScannerDeviceType)
                {
                    scanners.Add(deviceInfo.Properties["Name"].get_Value().ToString());
                }
            }

            return scanners;
        }

        // Method to start scan and return scanned document as Base64 string
        public string StartScanAndGetBase64(string scannerName)
        {
            try
            {
                DeviceManager deviceManager = new DeviceManager();
                DeviceInfo selectedDeviceInfo = null;

                // Locate the scanner with the specified name
                for (int i = 1; i <= deviceManager.DeviceInfos.Count; i++)
                {
                    DeviceInfo deviceInfo = deviceManager.DeviceInfos[i];
                    if (deviceInfo.Type == WiaDeviceType.ScannerDeviceType &&
                        deviceInfo.Properties["Name"].get_Value().ToString().Equals(scannerName, StringComparison.OrdinalIgnoreCase))
                    {
                        selectedDeviceInfo = deviceInfo;
                        break;
                    }
                }

                if (selectedDeviceInfo == null)
                {
                    throw new Exception($"Scanner '{scannerName}' not found.");
                }

                // Connect to the selected scanner
                Device scannerDevice = selectedDeviceInfo.Connect();

                // Open the scanner’s native interface to allow manual scanning
                CommonDialog wiaDialog = new CommonDialog();
                ImageFile scannedImage = wiaDialog.ShowAcquireImage(
                    WiaDeviceType.ScannerDeviceType,
                    WiaImageIntent.ColorIntent,
                    WiaImageBias.MaximizeQuality,
                    FormatID.wiaFormatJPEG,
                    true, // Display native UI
                    true, // Allow preview
                    false // Don't close the UI automatically after scanning
                );

                if (scannedImage != null)
                {
                    Console.WriteLine("Scan completed successfully.");
                    // Convert scanned image to Base64
                    byte[] imageBytes = (byte[])scannedImage.FileData.get_BinaryData();
                    string base64Image = Convert.ToBase64String(imageBytes);
                    return base64Image;
                }
                else
                {
                    throw new Exception("Scan canceled or failed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while scanning: " + ex.Message);
                throw;
            }
        }
    }
}
