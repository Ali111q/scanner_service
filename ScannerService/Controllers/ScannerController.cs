using Microsoft.AspNetCore.Mvc;
using ScannerApi.Services;
using System;
using System.Collections.Generic;

namespace ScannerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScannerController : ControllerBase
    {
        private readonly ScannerReadService _scannerService;

        public ScannerController(ScannerReadService scannerService)
        {
            _scannerService = scannerService;
        }

        // Endpoint to get connected scanners
        [HttpGet("connected")]
        public ActionResult<List<string>> GetConnectedScanners()
        {
            var scanners = _scannerService.GetConnectedScanners();
            if (scanners.Count == 0)
            {
                return NotFound(new { message = "No scanners found." });
            }

            return Ok(scanners);
        }

        // Endpoint to start scan and return scanned document as Base64 string
        [HttpGet("scan")]
        public IActionResult StartScanAndReturnBase64([FromQuery] string scannerName)
        {
            if (string.IsNullOrEmpty(scannerName))
            {
                return BadRequest(new { message = "Scanner name must be provided." });
            }

            try
            {
                string base64Image = _scannerService.StartScanAndGetBase64(scannerName);
                return Ok(new { scannedImage = base64Image });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}