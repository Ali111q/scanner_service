using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScannerApi.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScannerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ScannerReadService _scannerService;

        public Worker(ILogger<Worker> logger, ScannerReadService scannerService)
        {
            _logger = logger;
            _scannerService = scannerService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Service is running at: {time}", DateTimeOffset.Now);

                // Example: Log connected scanners periodically
                var scanners = _scannerService.GetConnectedScanners();
                if (scanners.Count > 0)
                {
                    _logger.LogInformation("Connected scanners: {scanners}", string.Join(", ", scanners));
                }
                else
                {
                    _logger.LogInformation("No scanners found.");
                }

                await Task.Delay(10000, stoppingToken); // Delay for 10 seconds
            }
        }
    }
}